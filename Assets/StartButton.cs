using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    //Button used to start the game
    private Button start;

    void Start()
    {
        start = GetComponent<Button>();
        start.onClick.AddListener(startGame);
    }

    void startGame()
    {
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
    }
}
