using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO.Ports;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class Game : MonoBehaviour
{
    public int score;
    
    public TMP_Text scoreText;
    public GameObject scoringText;
    public GameObject playButton;
    public GameObject backButton;
    public GameObject player1;
    public Button backB;

    public Player player;

    //GAME OVER POPUP
    public GameObject gameOverPopup;
    public GameObject gameOver;
    public GameObject wellDone;
    public TMP_Text FinalscoreText;
    public TMP_Text maxVolume;
    public TMP_Text averageFlow;
    public Button restartButton;
    public Button exitButton;

    public float playedDuration;

    string data;
    public float maxVolValue;
    public float avgFlowValue;
    

    private SerialReader serialReader;

    private int finalscore;
    

    private string SavePath => Application.persistentDataPath + "/userData.json";

    void Start()
    {
        serialReader = FindObjectOfType<SerialReader>();

        gameOverPopup.SetActive(false);

        restartButton.onClick.AddListener(RestartGame);
        backB.onClick.AddListener(Back);
        exitButton.onClick.AddListener(ExitGame);

        Debug.Log("Persistent Data Path: " + Application.persistentDataPath);
    }

    void Awake()
    {
        Application.targetFrameRate = 60;
        Pause();
    }

    public void Play()
    {
        serialReader.port.Write("2");
        Debug.Log("Sent 2 to Arduino");

        score = 0;
        scoreText.text = score.ToString();

        playButton.SetActive(false);
        backButton.SetActive(false);

        Time.timeScale = 1f;
        player.enabled = true;
       

        Pipes[] pipes = FindObjectsOfType<Pipes>();

        for(int i = 0; i<pipes.Length; i++)
        {
            Destroy(pipes[i].gameObject);
        }

    }

    void Pause()
    {
        Time.timeScale = 0f;
        player.enabled = false;
    }

    
    public void GameOver()
    {
        Pause();

        playedDuration = player.GetElapsedTime();
        Debug.Log("Total Time Played: " + playedDuration + " seconds");

        GetMaxAverage();
        wellDone.SetActive(false);
        scoringText.SetActive(false);
        finalscore = score;

        PathDrawer pathDrawer = FindObjectOfType<PathDrawer>();
        List<Vector2> drawnPath = pathDrawer != null ? pathDrawer.GetPath() : new List<Vector2>();

        gameOverPopup.SetActive(true);
        FinalscoreText.text = "Score: " + finalscore;
        maxVolume.text = "Highest deep inhalation: " + maxVolValue;
        averageFlow.text = "Average airFlow: " + avgFlowValue;

        SaveGameSession(drawnPath);

    }

    public void EndGame()
    {
        Pause();

        playedDuration = player.GetElapsedTime();
        Debug.Log("Total Time Played: " + playedDuration + " seconds");

        GetMaxAverage();
        gameOverPopup.SetActive(true);
        scoringText.SetActive(false);
        gameOver.SetActive(false);
        wellDone.SetActive(true);
        finalscore = score;

        PathDrawer pathDrawer = FindObjectOfType<PathDrawer>();
        List<Vector2> drawnPath = pathDrawer != null ? pathDrawer.GetPath() : new List<Vector2>();

        FinalscoreText.text = "Score: " + finalscore;
        maxVolume.text = "Highest deep inhalation: " +maxVolValue;
        averageFlow.text = "Average airFlow: " +avgFlowValue;


        SaveGameSession(drawnPath);
    }

    public void GetMaxAverage()
    {
        serialReader.port.Write("4");  

        try
        {
            if (serialReader.port.IsOpen)
            {
                data = serialReader.port.ReadLine();
            }
        }
        catch (System.Exception ex)
        {
            ex = new System.Exception();
        }

        string[] values = data.Split(',');

        float.TryParse(values[0], out maxVolValue);
        float.TryParse(values[1], out avgFlowValue);
    }

    private void SaveGameSession(List<Vector2> drawnPath)
    {
        string currentUser = PlayerPrefs.GetString("CurrentUser", "Guest");
        DataManager.UserDataCollection collection = DataManager.LoadUserData();

        DataManager.UserData user = collection.users.Find(u => u.username == currentUser);
        if (user == null)
        {
            user = new DataManager.UserData { username = currentUser };
            collection.users.Add(user);
        }

        List<Vector2> coinPath = CoinPathTracker.Instance != null ? CoinPathTracker.Instance.GetCoinPath() : new List<Vector2>();

        DataManager.GameSessionData newSession = new DataManager.GameSessionData
        {
            dateTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            score = finalscore,
            gameDuration = playedDuration,
            maxVolumeInhale = maxVolValue,
            avgFlow = avgFlowValue,
            forceMul = player.forceMultiplier,
            drawnPath = new List<Vector2>(drawnPath),
            coinPath = new List<Vector2>(coinPath)
        };

        user.gameSessions.Add(newSession);

        DataManager.SaveUserData(collection);
        Debug.Log("Game session with path saved for: " + currentUser);

    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Back()
    {
        SceneManager.LoadScene("Calibration");
        serialReader.port.Write("1");

    }

    public void ExitGame()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void IncreaseScore()
    {
        score++;
        scoreText.text = score.ToString();
    }
}
