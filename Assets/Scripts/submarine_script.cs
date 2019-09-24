using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public Transform sub_transform; //this is the transform associated to the submarine (the player character)
    void Start()
    {
        //when the game starts, place the submarine in the middle of the screen
        sub_transform.position = new Vector3(0, 0, 0);
        direction = 1;
        facing_right = true;
    }

    // Update is called once per frame
    void Update()
    {
        check_inputs();
    }

    /// <summary>
    /// This function will check if there have been inputs and apply the correct transformations
    /// </summary>
    void check_inputs()
    {
        //this is the key to move the submarine down
        //we need to check to make sure that we can move down as well (make sure we stay in frame)
        if (Input.GetAxis("Vertical") < 0 && sub_transform.position.y > ground_level)
        {
            sub_transform.Translate(Vector2.down*move_speed*Time.deltaTime);
        }

        //this is the key to move up
        if(Input.GetAxis("Vertical") > 0 && sub_transform.position.y < water_level)
        {
            sub_transform.Translate(Vector2.up * move_speed * Time.deltaTime);
        }

        //this is the key to move left
        if(Input.GetAxis("Horizontal") < 0)
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
