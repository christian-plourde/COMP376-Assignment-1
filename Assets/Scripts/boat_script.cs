using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading;

public enum Size{SMALL, MEDIUM, LARGE}

public class Gold
{

    GameObject gold_object;
    Size gold_size;
    
    public GameObject GoldObject
    {
        get { return gold_object; }
    }

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

    public Gold(GameObject gold, Size gold_size)
    {
        gold_object = gold;
        this.gold_size = gold_size;
    }

}

public class AngryFish
{
    GameObject fish_object;
    float move_speed;
    float scale;

    public float Scale
    {
        get { return scale; }
    }

    public float MoveSpeed
    {
        get { return move_speed; }
        set { move_speed = value; }
    }

    public GameObject Fish
    {
        get { return fish_object; }
    }

    public AngryFish(GameObject fish)
    {
        this.fish_object = fish;
        move_speed = UnityEngine.Random.Range(0.02f, 0.08f);
        scale = UnityEngine.Random.Range(1.0f, 2.0f);
        fish.transform.localScale = new Vector3(scale, scale, scale);
    }
}

public class boat_script : MonoBehaviour
{
    // Start is called before the first frame update
    DateTime last_gold_add;
    public GameObject angry_fish;
    public GameObject gold_object;
    public LinkedList<Gold> gold_objects;
    public LinkedList<AngryFish> angry_fish_list;
    public float gold_generation_interval;
    public int total_gold;
    public Transform boat_transform;
    public Text text;
    public Text level_indicator_text;
    public string main_menu_scene_name;
    public float time_in_level; //this is the total time spent in this level
    public float level_time; //the time that a player should spend in a level
    public int level; //the level we are on.

    void Start()
    {
        
        gold_objects = new LinkedList<Gold>();
        angry_fish_list = new LinkedList<AngryFish>();
        last_gold_add = DateTime.Now;
        total_gold = 0;

        //we need to instantiate 5 angry fish at different depths
        for (int i = 0; i < 5; i++)
        {
            //create a new angry fish at random depth
            AngryFish new_fish = new AngryFish(Instantiate(angry_fish, new Vector3(-15.0f, UnityEngine.Random.Range(-3.1f, 1.1f), -1), transform.rotation));
            angry_fish_list.AddLast(new_fish);
        }

        //set the time in level to 0 to start
        time_in_level = 0;

        //also set level to 1
        level = 1;

    }

    // Update is called once per frame
    void Update()
    {
        check_level_update();

        //check if the player wants to return to the main menu
        if(Input.GetButton("Cancel"))
        {
            scene_switcher switcher = new scene_switcher();
            switcher.switch_scene(main_menu_scene_name);
        }

        if(DateTime.Now > last_gold_add.AddSeconds(gold_generation_interval))
        {
            generate_gold();
            last_gold_add = DateTime.Now;
        }

        //we also need to move the angry fish
        foreach(AngryFish fish in angry_fish_list)
        {
            fish.Fish.transform.Translate(new Vector3(fish.MoveSpeed, 0, 0));

            //if the fish has gone past the right side of the screen reset the x position to its intial spot
            if(fish.Fish.transform.position.x > 15)
                fish.Fish.transform.position = new Vector3(-15.0f, UnityEngine.Random.Range(-3.1f, 1.1f), -1);
        }
    }

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
        }
    }

    public void addGoldBar()
    {
        System.Random random = new System.Random();

        GameObject new_gold = Instantiate(gold_object, new Vector3(random.Next(-9, 9), -3.344f, -1), transform.rotation);

        Size new_gold_size;
        if ((int)random.Next(0, 2) == 0)
        {
            new_gold.transform.localScale = new Vector3(0.3f, 0.3f, 1.0f);
            new_gold_size = Size.SMALL;
        }
            
        else if ((int)random.Next(0, 2) == 1)
        {
            new_gold.transform.localScale = new Vector3(0.6f, 0.6f, 1.0f);
            new_gold_size = Size.MEDIUM;
        }
            
        else
        {
            new_gold.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            new_gold_size = Size.LARGE;
        }
            

        gold_objects.AddLast(new Gold(new_gold, new_gold_size));
    }

    public void removeGoldBar(Gold gold_bar)
    {
        gold_objects.Remove(gold_bar);
        Destroy(gold_bar.GoldObject);
    }

    public void generate_gold()
    {
        //if there are less than 5 gold bars, let's create a new one at a random position in the
        //bottom of the sea

        if (gold_objects.Count < 5)
        {
            addGoldBar();
        }
    }

    public void increase_total_gold(int count)
    {
        total_gold += count;
        text.text = "Total Gold: " + total_gold;
    }
}
