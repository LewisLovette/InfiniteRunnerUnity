using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveData : MonoBehaviour
{

    private float highscore = 0;


    // Start is called before the first frame update
    void Awake()
    {
        //Dont destroy on scene load
        DontDestroyOnLoad(this.gameObject);
    }

    public void setHighScore(float score)
    {
        if(score > highscore)
        {
            highscore = score;
        }
    }

    public float getHighScore()
    {
        return highscore;
    }

}
