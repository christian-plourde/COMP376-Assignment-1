using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading;

//An enum to define the size of the gold produced
public enum Size{SMALL, MEDIUM, LARGE}

//a class that contains a gold game object and its corresponding size
public class Gold
{

    GameObject gold_object; //the gold game object instantiated from a prefab
    Size gold_size; //the size of the gold being generated
    
    //returns the game object associated with the class
    public GameObject GoldObject
    {
        get { return gold_object; } 
    }

    //returns the value based on the size of the gold object
    //small gives 1 point, medium gives 2, large gives 10
    public int Value
    {
        get { switch (gold_size)
                {
                case Size.SMALL: return 1;
                case Size.MEDIUM: return 2;
                case Size.LARGE: return 10;
                default: return 1;
                }
            }
    }

    //creates a new gold object from a gold prefab with a given size
    public Gold(GameObject gold, Size gold_size)
    {
        gold_object = gold;
        this.gold_size = gold_size;
    }

}

//a class that contains information about an octopus object
public class Octopus
{
    GameObject octopus_object; //the octopus object spawned from the prefab
    float move_speed; //the movement speed of the octopus (randomized)
    float scale; //the scale of the octopus (randomized)
    bool spawned; //indicates if the octopus has spawned yet or not
    float spawn_time; //the spawn time of the octopus in the level (randomized)
    int direction; //the direction that the octopus is facing (important for translation every frame)


    //returns the direction the octopus is facing (1 or -1)
    public int Direction
    {
        get { return direction; }
        set { direction = value; }
    }

    //returns the spawn time in the level that the octopus should spawn at
    public float SpawnTime
    {
        get { return spawn_time; }
    }

    //indicates if the octopus has spawned yet or not
    public bool Spawned
    {
        get { return spawned; }
    }

    //indicates the scale factor of the octopus
    public float Scale
    {
        get { return scale; }
    }

    //allows setting or retrieving of the current move speed of the octopus
    public float MoveSpeed
    {
        get { return move_speed; }
        set { move_speed = value; }
    }

    //returns the actual game object prefab associated with an octopus
    public GameObject Oct
    {
        get { return octopus_object; }
    }

    //will create a new octopus from the prefab with randomized move speed and scale and set it moving
    //in the positive x direction
    public Octopus(GameObject oct)
    {
        this.octopus_object = oct;
        move_speed = UnityEngine.Random.Range(0.02f, 0.08f);
        scale = UnityEngine.Random.Range(1.0f, 2.0f);
        octopus_object.transform.localScale = new Vector3(scale, scale, scale);
        spawned = true;
        direction = 1;
    }

    //will create a new octopus that has not spawned and create its spawn time based on the length of
    //the level to ensure it spawns before the level is over
    public Octopus(float level_time)
    {
        spawned = false;
        spawn_time = UnityEngine.Random.Range(0, level_time);
    }
}

//a class to represent the angry fish enemy class
public class AngryFish
{
    GameObject fish_object; //the actual prefab object this is instantiated from
    float move_speed; //the move speed of the fish (randomized)
    float scale; //the scale of the fish (randomized)

    //returns the scale of the fish
    public float Scale
    {
        get { return scale; }
    }

    //returns or sets the move speed of the fish
    public float MoveSpeed
    {
        get { return move_speed; }
        set { move_speed = value; }
    }

    //returns the actual prefab associated with the class
    public GameObject Fish
    {
        get { return fish_object; }
    }

    //creates an angryfish object with random scale and move speed
    public AngryFish(GameObject fish)
    {
        this.fish_object = fish;
        move_speed = UnityEngine.Random.Range(0.02f, 0.08f);
        scale = UnityEngine.Random.Range(1.0f, 2.0f);
        fish.transform.localScale = new Vector3(scale, scale, scale);
    }
}

//this class is tied to the boat object and contains many of the game
//functionalities since it is a static game objects
public class boat_script : MonoBehaviour
{
    DateTime last_gold_add; //the last time a gold bar was spawned
    public GameObject angry_fish; //a reference to the angry fish prefab so it can be instantiated
    public GameObject octopus; //a reference to the octopus prefab so it can be instantiated
    public GameObject gold_object; //a reference to the octopus prefab so it can be instantiated
    public GameObject nitro_cylinder; //a reference to the nitro cylinder prefab so it can be instantiated
    public LinkedList<Gold> gold_objects; //a list of all the gold objects currently in the scene
    public LinkedList<AngryFish> angry_fish_list; //a list of all the fish currently in the scene
    public float gold_generation_interval; //the interval at which gold should be generated in the level
    public int total_gold; //the total gold the player has collected (score)
    public Transform boat_transform; //the transform of the boat object (this class)
    public Text text; //the text that indicates how much gold the player has collected
    public Text level_indicator_text; //the text that contains the indication of which level the player has reached
    public string main_menu_scene_name; //the name of the main menu scene (for returning to the menu on esc)
    public float time_in_level; //this is the total time spent in this level
    public float level_time; //the time that a player should spend in a level
    public int level; //the level we are on.
    Octopus[] octopus_list; //an array of octopus (size 2 since 2 octopus spawn in each level)
    public bool variant_mode; //indicates if we are playing in variant mode or not

    void Start()
    {
        //set variant mode to false
        variant_mode = false;
        //check if we are in variant mode
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "VariantMode")
            variant_mode = true; //if we are set it to true

        gold_objects = new LinkedList<Gold>(); //instantiate list of gold objects
        angry_fish_list = new LinkedList<AngryFish>(); //instantiate list of fish
        octopus_list = new Octopus[2]; //instantiate array of octopus
        for(int i = 0; i < octopus_list.Length; i++)
        {
            octopus_list[i] = new Octopus(level_time); //create two new empty octopus
            //they wont show up but they will have a spawn time based on how long the level is
        }
        last_gold_add = DateTime.Now; //set last gold add to now
        total_gold = 0; //the total gold the player has collected should be 0 to start

        //we need to instantiate 5 angry fish at different depths
        for (int i = 0; i < 5; i++)
        {
            //create a new angry fish at random depth
            AngryFish new_fish = new AngryFish(Instantiate(angry_fish, new Vector3(-15.0f, UnityEngine.Random.Range(-3.1f, 1.1f), -1), transform.rotation));
            angry_fish_list.AddLast(new_fish); //add it to the list
        }

        //set the time in level to 0 to start
        time_in_level = 0;

        //also set level to 1
        level = 1;
        
    }

    // Update is called once per frame
    void Update()
    {
        check_level_update(); //checks if the level should be changed (based on how long we have been in
        //the level and how long each level lasts)

        //check if the player wants to return to the main menu
        if(Input.GetButton("Cancel"))
        {
            scene_switcher switcher = new scene_switcher(); //if so switch to main menu
            switcher.switch_scene(main_menu_scene_name); //custom class and method
        }

        //if the time since the last gold was added is larger that the interval between gold generation
        //add gold
        if(DateTime.Now > last_gold_add.AddSeconds(gold_generation_interval))
        {
            generate_gold(); //adds a gold bar to the scene
            last_gold_add = DateTime.Now; //reset the last gold added time to now
        }

        //we also need to move the angry fish
        foreach(AngryFish fish in angry_fish_list)
        {
            fish.Fish.transform.Translate(new Vector3(fish.MoveSpeed, 0, 0)); //move the fish in the positive x direction

            //if the fish has gone past the right side of the screen reset the x position to its intial spot
            if(fish.Fish.transform.position.x > 15)
                fish.Fish.transform.position = new Vector3(-15.0f, UnityEngine.Random.Range(-3.1f, 1.1f), -1);
        }

        //check if an octopus should spawn
        for(int i = 0; i < octopus_list.Length; i++)
        {
            //if the slot in the octopus array has not been spawned and the time since the level started
            //is larger than the spawntime of the octopus then the octopus should be spawned
            if (!octopus_list[i].Spawned && time_in_level >= octopus_list[i].SpawnTime)
            {
                //create new octopus in scene
                octopus_list[i] = new Octopus(Instantiate(octopus, new Vector3(-10.1f, UnityEngine.Random.Range(-3.1f, 1.1f), -1), transform.rotation));
            }
        }

        //finally move all the spawned octopus
        foreach (Octopus octopus in octopus_list)
        {

            //check if null
            //we dont want to move an octopus if it doesnt exist
            if (octopus.Oct == null)
                continue;

            //if the octopus has passed the right side make it start moving in the negative x direction
            if(octopus.Oct.transform.position.x > 10)
            {
                octopus.Direction = -1;
            }

            //if the octopus has passed the left side make it start moving in the positive x direction
            else if (octopus.Oct.transform.position.x <= -10)
            {
                octopus.Direction = 1;
            }

            //do the movement of the octopus after its move direction has been set
            octopus.Oct.transform.Translate(new Vector3(octopus.Direction * octopus.MoveSpeed, 0, 0));

        }

        //SPECIAL CODE FOR VARIANT MODE
        if(variant_mode)
        {
            //if we are in variant mode then every frame we have a chance of spawning a nitro cylinder
            //generate a random number
            float spawn_nitro = UnityEngine.Random.Range(0.0f, 1.0f);

            //if the generated number is less than this, we create a new nitro cylinder
            if(spawn_nitro < 0.002)
            {
                Instantiate(nitro_cylinder, new Vector3(UnityEngine.Random.Range(-9, 9), 1.21f, -1.0f), transform.rotation);
            }
        }

    }

    //this function is called in update and checks if the level should be updated
    private void check_level_update()
    {
        //this function checks if it is time to update the level and should be called once every frame
        //to update the  time the player has spent in the level and if applicable increase the level and
        //the speed of the fish
        time_in_level += Time.deltaTime;
        if(time_in_level > level_time)
        {
            //first reset time in level to 0
            time_in_level = 0;
            //then we need to increase the move_speed of the fish
            foreach(AngryFish fish in angry_fish_list)
            {
                fish.MoveSpeed = fish.MoveSpeed * 1.1f; //increase fish speed by 10%
            }
            //finally we need to increase the level indicator in the UI.
            level_indicator_text.text = "Level: " + ++level;

            //reset the octopus since the level starts with none
            for (int i = 0; i < octopus_list.Length; i++)
            {
                Destroy(octopus_list[i].Oct);
                octopus_list[i] = new Octopus(level_time);
            }

        }
    }

    //this will add a gold bar to the scene
    public void addGoldBar()
    {
        //generate random number to determine where on the ground it should spawn
        System.Random random = new System.Random();

        GameObject new_gold = Instantiate(gold_object, new Vector3(random.Next(-9, 9), -3.344f, -1), transform.rotation);

        //genrate a random size for the new gold piece
        Size new_gold_size;
        //small gold
        if ((int)random.Next(0, 2) == 0)
        {
            //scale based on the size of the gold
            new_gold.transform.localScale = new Vector3(0.3f, 0.3f, 1.0f);
            new_gold_size = Size.SMALL;
        }
        
        //medium gold
        else if ((int)random.Next(0, 2) == 1)
        {
            new_gold.transform.localScale = new Vector3(0.6f, 0.6f, 1.0f);
            new_gold_size = Size.MEDIUM;
        }
        
        //large gold
        else
        {
            new_gold.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            new_gold_size = Size.LARGE;
        }
            
        //add the new gold to the list
        gold_objects.AddLast(new Gold(new_gold, new_gold_size));
    }

    //called when a player collects a gold bar
    public void removeGoldBar(Gold gold_bar)
    {
        gold_objects.Remove(gold_bar); //remove bar from list
        Destroy(gold_bar.GoldObject); //destroy game object
    }

    //will generate new gold
    public void generate_gold()
    {
        //if there are less than 5 gold bars, let's create a new one at a random position in the
        //bottom of the sea

        if (gold_objects.Count < 5)
        {
            addGoldBar();
        }
    }

    //called when the player successfully drops gold off at the boat
    public void increase_total_gold(int count)
    {
        total_gold += count; //gold countis increased
        text.text = "Total Gold: " + total_gold; //UI is updated to show new gold count
    }
}
