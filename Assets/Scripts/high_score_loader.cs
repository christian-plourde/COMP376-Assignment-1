using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Xml;
using System;

public class high_score_loader : MonoBehaviour
{
    public Text normal_mode_level;
    public Text variant_mode_level;
    public Text normal_mode_gold;
    public Text variant_mode_gold;

    // Start is called before the first frame update
    void Start()
    {
        //when this loads we should load in the high scores and display them
        try
        {
            XmlDocument xml = new XmlDocument();
            xml.Load("Assets/Scripts/high_scores.xml"); //load the high scores file
            normal_mode_gold.text = xml.SelectNodes("root/normal_mode_gold")[0].InnerXml;
            normal_mode_level.text = xml.SelectNodes("root/normal_mode_level")[0].InnerXml;
            variant_mode_gold.text = xml.SelectNodes("root/variant_mode_gold")[0].InnerXml;
            variant_mode_level.text = xml.SelectNodes("root/variant_mode_level")[0].InnerXml;
        }

        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
