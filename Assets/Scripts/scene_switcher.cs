using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class scene_switcher : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //will switch to the scene bearing that name
    public void switch_scene(string scene_name)
    {
        SceneManager.LoadScene(scene_name);
    }
}
