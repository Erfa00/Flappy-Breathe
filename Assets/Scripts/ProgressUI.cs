using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class ProgressUI : MonoBehaviour
{
    public TMP_Text usernameText;
    public Button backButton;

    private List<string> dateLabels = new List<string>();
    private List<int> scores = new List<int>();

    // Start is called before the first frame update
    void Start()
    {
        string currentUser = PlayerPrefs.GetString("CurrentUser", "Guest");
        usernameText.text = currentUser + "'s Progress";

        List<DataManager.GameSessionData> gameSessions = DataManager.GetUserGameSessions(currentUser);
       
        if (gameSessions.Count > 0)
        {
            foreach (var session in gameSessions)
            {
                dateLabels.Add(session.dateTime); 
                scores.Add(session.score);       
            }

            Debug.Log("Loaded " + gameSessions.Count + " sessions.");
         }

        else
        {
            Debug.LogWarning("No game data found for user: " + currentUser);
        }
            backButton.onClick.AddListener(GoBack);
       
    }

    void GoBack()
    {
        Debug.Log("Go Back");
        SceneManager.LoadScene("Main Menu");
    }

}
