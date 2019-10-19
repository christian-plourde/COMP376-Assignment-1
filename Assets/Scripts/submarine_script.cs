using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System;
using System.Xml;
using System.IO;

public class submarine_script : MonoBehaviour
{
    // Start is called before the first frame update
    bool facing_right; //indicates if the sub should be rotated
    float direction; //this is the modifier for the direction the sub is facing in the x direction
    public float left_border; //the left border of the screen (if we are to the left of this we wrap around)
    public float right_border; //the right border of the screen (if we exceed this we wrap around)
    public float ground_level; //this is the level of the ground that cannot be passed
    public float water_level; //this is the level of the water (which cannot be exceeded when moving the sub up)
    public float move_speed; //the speed at which the sub will move around
    private float init_move_speed; //this is the initial move speed of the submarine
    public Transform sub_transform; //this is the transform associated to the submarine (the player character)
    public boat_script boat; //a reference to the boat script since it has functions to add gold to our count
    int gold_bar_count; //the number of gold bars the  player is holding
    int oxygen_cylinders; //the number of lives the player has left
    public Text life_count; //the text in the UI that shows how many lives the player has left
    bool immune; //important to set this to true for a few seconds after player loses a life
    DateTime immune_start; //the start time of the players immune cycle
    bool nitro_cylinder_active; //indicates if the nitro boost is currently active
    bool nitro_cylinder_available; //indicates if nitro cylinder boost is available (i.e. player currently has at least one nitro cylinder)
    DateTime nitro_cylinder_start; //the start time of the nitro cylinders effect
    int nitro_cylinder_count; //the number of nitro cylinders the player has collected
    int nitro_cylinder_duration; //the duration of the effect of 1 nitro cylinder
    int nitro_cylinder_speed_multiplier; //the speed multiplier when using a nitro cylinder

    void Start()
    {
        nitro_cylinder_duration = 3; //nitro cylinder effect lasts for 3 seconds
        nitro_cylinder_speed_multiplier = 3; //speed multiplier for nitro cylinder is 3
        nitro_cylinder_count = 0; //count of nitro cylinders is initially 0
        nitro_cylinder_available = false; //nitro cylinder is initially unavailable
        init_move_speed = move_speed; //the initial move speed is set and remembered
        nitro_cylinder_active = false; //nitro cylinder is initially inactive
        sub_transform.position = new Vector3(0, 0, -1); //when the game starts, place the submarine in the middle of the screen 
        direction = 1; //the player is initially moving in the positive x direction
        facing_right = true; //indicates if the player is facing right (starts as true)
        oxygen_cylinders = 2; //player starts with two lives
        life_count.text = "Oxygen Cylinders: " + oxygen_cylinders; //write to the UI the number of lives the player has
        immune_start = DateTime.Now; //set the immune start time to now (not necessary)
        immune = false; //player is not immune to start off
    }

    // Update is called once per frame
    void Update()
    {
        //if the nitro_cylinder is active we need to increase our movement speed if the effect is still active (i.e. its time has not expired)
        if (nitro_cylinder_active && (DateTime.Now - nitro_cylinder_start).TotalSeconds < nitro_cylinder_count*nitro_cylinder_duration)
        {
            move_speed = init_move_speed * nitro_cylinder_speed_multiplier;
        }

        //if the nitro cylinder is active but exxpired we need to disable it and reset the nitro cylinder count and our move speed
        else if (nitro_cylinder_active)
        {
            move_speed = init_move_speed;
            nitro_cylinder_active = false;
            nitro_cylinder_count = 0;
        }

        //input handler
        check_inputs();

        //next we need to check if we are close to any gold
        //place the gold objects in an array for easy processing (tied to the boat)
        Gold[] gold_objects = boat.gold_objects.ToArray<Gold>();
        for(int i = 0; i < boat.gold_objects.Count; i++)
        {
            //if we are close to any gold
            if ((gold_objects[i].GoldObject.transform.position - sub_transform.position).magnitude < 0.5)
            {
                boat.removeGoldBar(gold_objects[i]); //pick up the gold while removing it from the scene
                gold_bar_count+= gold_objects[i].Value; //increase our gold count (in the sub)
                //set the move speed as a function of the gold_bar_count higher it is slower we go
                if(move_speed > 0.1)
                    move_speed -= 0.1f*gold_objects[i].Value;
            }
        }

        //now we should check if we are close to the boat
        //if we are we should drop the gold into it
        if((sub_transform.position - boat.boat_transform.position).magnitude < 1)
        {
            boat.increase_total_gold(gold_bar_count); //increase our score by the amount of gold we are carrying
            //we also need to reset the speed of the submarine
            move_speed = init_move_speed; //reset our speed
            gold_bar_count = 0; //and set the gold we are carrying to 0
        }

        //finally we need to check if the boat is close to one of the fish
        //if it is then the player should lose a life

        //first let's check if the character should keep his immunity (if he has been immune for less than 2 seconds he should stay immune)
        if ((DateTime.Now - immune_start).TotalSeconds >= 2)
        {
            immune = false; //if we are no longer immune set the flag to false
            this.GetComponent<SpriteRenderer>().color = Color.white; //remove the red coloring on the sub
        }

        else if(immune)
        {
            //if we are immune make the sprite red
            this.GetComponent<SpriteRenderer>().color = Color.red;
        }

        //if we have a nitro_cylinder available we should be green
        if(nitro_cylinder_available)
            this.GetComponent<SpriteRenderer>().color = Color.green;
    }

    //this function determines what occurs when there is a collision
    void OnTriggerEnter2D(Collider2D col)
    {
        //if we are not immune and we collide with a fish or an octopus
        if(!immune && (col.gameObject.CompareTag("angry_fish") || col.gameObject.CompareTag("octopus")))
        {
            lose_life(); //we should lose a life
            immune = true; //and become immune
            immune_start = DateTime.Now; //set the immunity start time to now so we can determine when it should end
        }

        //only collect nitro cylinders if we are not currently using them
        if(col.gameObject.CompareTag("nitro_cylinder") && !nitro_cylinder_active)
        {
            //this happens when we collide with a nitro cylinder and we are not currently using one
            nitro_cylinder_available = true; //make the nitro cylinder available for use
            nitro_cylinder_count++; //increase our count of nitro cylinders
        }
    }

    //this is called when we should lose a life
    private void lose_life()
    {
        oxygen_cylinders--; //reduce the number of lives we have
        life_count.text = "Oxygen Cylinders: " + oxygen_cylinders; //update the UI indicator of lives left

        //once the life has been lost check if the player has 0 lives
        //if that is the case place him back at the boat and restore his cylinders. Also make his score 0
        if(oxygen_cylinders <= 0)
        {
            //we load the current score into an xml file that is read in the menu to show the current high scores
            try
            {
                XmlDocument xml = new XmlDocument();
                xml.Load("Assets/Scripts/high_scores.xml"); //load the high scores file
                if (!boat.variant_mode) //if we are in normal mode
                {
                    //if the level is higher than what was previously recorded in the high scores update it
                    if(boat.level > Int16.Parse(xml.SelectNodes("root/normal_mode_level")[0].InnerXml))
                        xml.SelectNodes("root/normal_mode_level")[0].InnerXml = boat.level.ToString();
                    //if the gold is higher than what was previously recorded in the high scores update it
                    if (boat.total_gold > Int16.Parse(xml.SelectNodes("root/normal_mode_gold")[0].InnerXml))
                        xml.SelectNodes("root/normal_mode_gold")[0].InnerXml = boat.total_gold.ToString();
                }

                else
                {
                    //if the level is higher than what was previously recorded in the high scores update it
                    if (boat.level > Int16.Parse(xml.SelectNodes("root/variant_mode_level")[0].InnerXml))
                        xml.SelectNodes("root/variant_mode_level")[0].InnerXml = boat.level.ToString();
                    //if the gold is higher than what was previously recorded in the high scores update it
                    if (boat.total_gold > Int16.Parse(xml.SelectNodes("root/variant_mode_gold")[0].InnerXml))
                        xml.SelectNodes("root/variant_mode_gold")[0].InnerXml = boat.total_gold.ToString();
                }

                //save the file
                xml.Save("Assets/Scripts/high_scores.xml");
            }

            catch (Exception e)
            {
                Debug.Log(e.Message);
            }

            boat.total_gold = 0; //reset the total gold
            oxygen_cylinders = 2; //reset the lives
            life_count.text = "Oxygen Cylinders: " + oxygen_cylinders; //reset the life count in the UI
            gold_bar_count = 0; //reset the number of gold bars the sub is carrying
            sub_transform.position = boat.transform.position; //place the sub at the boat

            //finally we need to reset the level to level 1.
            boat.level = 1; //reset level to 1
            boat.level_indicator_text.text = "Level: " + boat.level; //set UI text for level
            //and decrease the speed of the angry fish
            foreach(AngryFish fish in boat.angry_fish_list)
            {
                fish.MoveSpeed = UnityEngine.Random.Range(0.02f, 0.08f);
            }
            
        }
    }

    /// <summary>
    /// This function will check if there have been inputs and apply the correct transformations
    /// </summary>
    void check_inputs()
    {
        //this is the key to move the submarine down
        //we need to check to make sure that we can move down as well (make sure we stay in frame)

        //the sub should move down slightly every time
        sub_transform.Translate(Vector2.down * move_speed * 0.5f * Time.deltaTime);

        //this is the key to move up
        if(Input.GetAxis("Vertical") > 0 && sub_transform.position.y < water_level)
        {
            sub_transform.Translate(Vector2.up * move_speed * Time.deltaTime); //move sub up
        }

        if(Input.GetAxis("Vertical") < 0)
        {
            sub_transform.Translate(Vector2.down * move_speed * Time.deltaTime); //move sub down
        }

        //make sure we dont go out of the water
        if (sub_transform.position.y >= water_level)
        {
            sub_transform.position = new Vector3(sub_transform.position.x, water_level, sub_transform.position.z);
        }

        //or below the bottom
        if (sub_transform.position.y <= ground_level)
            sub_transform.position = new Vector3(sub_transform.position.x, ground_level, sub_transform.position.z);

        //this is the key to move left
        if (Input.GetAxis("Horizontal") < 0)
        {
            //if we move left we have to swap the direction modifier to -1 to flip the keys
            direction = -1;

            //we also need to rotate the sub 180 degrees about the y axis
            if(facing_right)
            {
                sub_transform.Rotate(Vector3.up, -180);
                facing_right = false;
            }

            //if we have passed the right border, we need to bring the sub back to the left
            if (sub_transform.position.x < left_border)
                sub_transform.position = new Vector3(right_border, sub_transform.position.y, sub_transform.position.z);
            else
                sub_transform.Translate(direction * Vector2.left * move_speed * Time.deltaTime);
        }

        //this is the key to move right
        if (Input.GetAxis("Horizontal") > 0)
        {
            //if we go right we need to again flip the direction modifier to be 1
            direction = 1;

            //then we rotate the sub image
            if(!facing_right)
            {
                sub_transform.Rotate(Vector3.up, -180);
                facing_right = true;
            }
                

            //if we have passed the right border, we need to bring the sub back to the left
            if (sub_transform.position.x > right_border)
                sub_transform.position = new Vector3(left_border, sub_transform.position.y, sub_transform.position.z);
            else
                sub_transform.Translate(direction * Vector2.right * move_speed * Time.deltaTime);
        }

        //activate nitro cylinders
        if(Input.GetKeyDown(KeyCode.E))
        {
            //if there is a nitro cylinder available activate it
            //this will make nitro cylinder available equal to false
            nitro_cylinder_available = false;
            nitro_cylinder_active = true;
            nitro_cylinder_start = DateTime.Now;
        }

    }
}
