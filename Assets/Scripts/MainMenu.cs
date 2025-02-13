using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class MainMenu : MonoBehaviour
{
    public Button start;
    public Button progress;
    public Button tutorial;
    public Button back;
    public TMP_Text username;

    // Start is called before the first frame update
    void Start()
    {
        string currentUser = PlayerPrefs.GetString("CurrentUser", "Guest");
        username.text = "Hi " + currentUser;

        start.onClick.AddListener(StartGame);
        progress.onClick.AddListener(progressScene);
        back.onClick.AddListener(backTo);
        tutorial.onClick.AddListener(goTutorial);
    }

    private void goTutorial()
    {
        SceneManager.LoadScene("How To Play");
    }

    private void StartGame()
    {
        SceneManager.LoadScene("Calibration");
    }

    private void progressScene()
    {
        SceneManager.LoadScene("Progress");
    }

    private void backTo()
    {
        SceneManager.LoadScene("First Scene");
    }
}
