﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuData : MonoBehaviour
{

    private SaveData data;
    private Text score;

    // Start is called before the first frame update
    void Start()
    {
        data = GameObject.Find("Data").GetComponent<SaveData>();
        score = GetComponent<Text>();

        score.text = "High-Score: "+data.getHighScore().ToString("F0");
    }

}
