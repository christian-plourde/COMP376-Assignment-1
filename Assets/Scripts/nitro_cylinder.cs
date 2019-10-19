using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class nitro_cylinder : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //the nitro cylinder should move down
        this.transform.Translate(new Vector3(0.0f, -0.9f, 0.0f)*Time.deltaTime);

        //if it is below ground level destroy it
        if (this.transform.position.y < -4.0f)
            Destroy(this.gameObject);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        //when the player collides with this it should be destroyed
        Destroy(this.gameObject);
    }
}
