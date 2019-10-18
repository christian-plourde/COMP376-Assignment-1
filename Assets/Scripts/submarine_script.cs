using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System;

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
    private float init_move_speed;
    public Transform sub_transform; //this is the transform associated to the submarine (the player character)
    public boat_script boat;
    int gold_bar_count;
    int oxygen_cylinders;
    public Text life_count;
    bool immune; //important to set this to true for a few seconds after player loses a life
    DateTime immune_start;
   

    void Start()
    {
        //when the game starts, place the submarine in the middle of the screen
        init_move_speed = move_speed;
        sub_transform.position = new Vector3(0, 0, -1);
        direction = 1;
        facing_right = true;
        oxygen_cylinders = 2;
        life_count.text = "Oxygen Cylinders: " + oxygen_cylinders;
        immune_start = DateTime.Now;
        immune = false;
    }

    // Update is called once per frame
    void Update()
    {

        check_inputs();

        //next we need to check if we are close to any gold
        
        Gold[] gold_objects = boat.gold_objects.ToArray<Gold>();
        for(int i = 0; i < boat.gold_objects.Count; i++)
        {

            if ((gold_objects[i].GoldObject.transform.position - sub_transform.position).magnitude < 0.5)
            {
                boat.removeGoldBar(gold_objects[i]);
                gold_bar_count+= gold_objects[i].Value;
                //set the move speed as a function of the gold_bar_count
                if(move_speed > 0.1)
                    move_speed -= 0.1f*gold_objects[i].Value;
            }
        }

        //now we should check if we are close to the boat
        //if we are we should drop the gold into it
        if((sub_transform.position - boat.boat_transform.position).magnitude < 1)
        {
            boat.increase_total_gold(gold_bar_count);
            //we also need to reset the speed of the submarine
            move_speed = init_move_speed;
            gold_bar_count = 0;
        }

        //finally we need to check if the boat is close to one of the fish
        //if it is then the player should lose a life

        //first let's check if the character should keep his immunity (if he has been immune for less than 2 seconds he should stay immune)
        if ((DateTime.Now - immune_start).TotalSeconds >= 2)
        {
            immune = false;
            this.GetComponent<SpriteRenderer>().color = Color.white;
        }
            

        else if(immune)
        {
            //if we are immune make the sprite red
            this.GetComponent<SpriteRenderer>().color = Color.red;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(!immune)
        {
            lose_life();
            immune = true;
            immune_start = DateTime.Now;
        }
    }

    private void lose_life()
    {
        oxygen_cylinders--;
        life_count.text = "Oxygen Cylinders: " + oxygen_cylinders;

        //once the life has been lost check if the player has 0 lives
        //if that is the case place him back at the boat and restore his cylinders. Also make his score 0
        if(oxygen_cylinders <= 0)
        {
            boat.total_gold = 0;
            oxygen_cylinders = 2;
            life_count.text = "Oxygen Cylinders: " + oxygen_cylinders;
            gold_bar_count = 0;
            sub_transform.position = boat.transform.position;

            //finally we need to reset the level to level 1.
            boat.level = 1;
            boat.level_indicator_text.text = "Level: " + boat.level;
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
            sub_transform.Translate(Vector2.up * move_speed * Time.deltaTime);
        }

        if(Input.GetAxis("Vertical") < 0)
        {
            sub_transform.Translate(Vector2.down * move_speed * Time.deltaTime);
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
                sub_transform.position = new Vector2(right_border, sub_transform.position.y);
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
                sub_transform.position = new Vector2(left_border, sub_transform.position.y);
            else
                sub_transform.Translate(direction * Vector2.right * move_speed * Time.deltaTime);
        }

    }
}
