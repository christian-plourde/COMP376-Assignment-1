using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;

public class boat_script : MonoBehaviour
{
    // Start is called before the first frame update
    DateTime last_gold_add;
    public GameObject gold_object;
    public LinkedList<GameObject> gold_objects;
    public float gold_generation_interval;
    public int total_gold;
    public Transform boat_transform;
    public Text text;

    void Start()
    {
        gold_objects = new LinkedList<GameObject>();
        last_gold_add = DateTime.Now;
        total_gold = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(DateTime.Now > last_gold_add.AddSeconds(gold_generation_interval))
        {
            generate_gold();
            last_gold_add = DateTime.Now;
        }
    }

    public void addGoldBar()
    {
        System.Random random = new System.Random();
        gold_objects.AddLast(Instantiate(gold_object, new Vector3(random.Next(-9, 9), -3.344f, -1), transform.rotation));
    }

    public void removeGoldBar(GameObject gold_bar)
    {
        gold_objects.Remove(gold_bar);
        Destroy(gold_bar);
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
