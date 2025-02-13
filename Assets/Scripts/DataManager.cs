using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;
using System.IO.Ports;

public class DataManager : MonoBehaviour
{
    public TMP_Text playerNameText;
    
    //Input field
    public TMP_InputField inhalationInput;
    public TMP_InputField timeInput;
    
    //Buttons in Calibration Scene
    public Button saveButton;
    public Button backButton;
    public Button startButton;
    public Button calibrateButton;

    //public TMP_Text maxInhaled;
    //public TMP_Text inhaleTime;

    //Data Manager
    private string currentUser;
    private UserData userData;
    private static string SavePath => Application.persistentDataPath + "userData.json";
    

    //Serial Comm
    private SerialReader serialReader;
    public string receivedData;

    
    // Start is called before the first frame update
    void Start()
    {
        serialReader = FindObjectOfType<SerialReader>();
       

        currentUser = PlayerPrefs.GetString("CurrentUser", "Guest");
        playerNameText.text = $" Hi {currentUser}, please calibrate your maximum inhalation volume";

        

        userData = LoadUserData(currentUser);
        //inhalationInput.text = userData.maxInhalationVolume.ToString();
        //timeInput.text = userData.timeInhale.ToString();

        saveButton.onClick.AddListener(SaveUserData);
        startButton.onClick.AddListener(StartGame);
        backButton.onClick.AddListener(GoBack);
        calibrateButton.onClick.AddListener(Calibrate);

    }

    public void Calibrate()
    {
        serialReader.port.Write("3");

        try
        {
            if (serialReader.port.IsOpen)
            {
                while (serialReader.port.BytesToRead == 0)
                {
                }

                receivedData = serialReader.port.ReadLine();
            }
        }
        catch (System.Exception ex)
        {
            ex = new System.Exception();
        }

        ProcessReceivedData(receivedData);
    }
  
    void ProcessReceivedData(string data)
    {
        string[] values = data.Split(',');

        if(values.Length == 2)
        {
            float maxInhaledValue;
            float inhaleTimeValue;

            if (float.TryParse(values[0], out maxInhaledValue) && float.TryParse(values[1], out inhaleTimeValue))
            {
                // Assign to UI Texts
                inhalationInput.text = maxInhaledValue.ToString();
                timeInput.text = inhaleTimeValue.ToString();

                Debug.Log($"Parsed: Max Inhaled = {maxInhaledValue} mL, Inhalation Time = {inhaleTimeValue} s");
            }
            else
            {
                Debug.LogError("Parsing failed: Invalid numerical values received.");
            }
        }
        else
        {
            Debug.LogError("Invalid data format received from Arduino: " + data);
        }

    }



    public void SaveUserData()
    {
        if (float.TryParse(inhalationInput.text, out float inhalationValue) &&
            float.TryParse(timeInput.text, out float timeValue))
        {
            userData.maxInhalationVolume = inhalationValue;
            userData.timeInhale = timeValue;

            SaveToJson(userData);

            Debug.Log("Breathing limits saved for:" + currentUser);

        }
        else
        {
            Debug.LogWarning("Invalid input");
        }

     }

    private UserData LoadUserData(string username)
    {
        if (File.Exists(SavePath))
        {
            string json = File.ReadAllText(SavePath);
            UserDataCollection collection = JsonUtility.FromJson<UserDataCollection>(json);

            foreach (var data in collection.users)
            {
                if(data.username == username)
                {
                    return data;
                }
            }
        }

        return new UserData { username = username, maxInhalationVolume = 0, timeInhale = 0 };
    }

    private void SaveToJson(UserData newData)
    {
        UserDataCollection collection = new UserDataCollection();

        if (File.Exists(SavePath))
        {
            string json = File.ReadAllText(SavePath);
            collection = JsonUtility.FromJson<UserDataCollection>(json);

        }

        bool found = false;
        for(int i = 0; i < collection.users.Count; i++)
        {
            if (collection.users[i].username == newData.username)
            {
                collection.users[i] = newData;
                found = true;
                break;
            }

        }

        if (!found)
        {
            collection.users.Add(newData);
        }

        string updatedJson = JsonUtility.ToJson(collection, true);
        File.WriteAllText(SavePath, updatedJson);
    }

    public void GoBack()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Flappy Bird");
    }

    

    public static UserDataCollection LoadUserData()
    {
        if (File.Exists(SavePath))
        {
            string json = File.ReadAllText(SavePath);
            return JsonUtility.FromJson<UserDataCollection>(json);
        }
        return new UserDataCollection();
    }

    public static void SaveUserData(UserDataCollection collection)
    {
        string updatedJson = JsonUtility.ToJson(collection, true);
        File.WriteAllText(SavePath, updatedJson);
        Debug.Log("User data saved successfully.");

    }

    public static List<GameSessionData> GetUserGameSessions(string username)
    {
        UserDataCollection collection = LoadUserData();

        UserData user = collection.users.Find(u => u.username == username);
        if (user != null)
        {
            return user.gameSessions;
        }

        return new List<GameSessionData>();
    }

    [System.Serializable]
    public class UserData
    {
        public string username;
        public float maxInhalationVolume;
        public float timeInhale;
        public List<GameSessionData> gameSessions = new List<GameSessionData>();
    }

    [System.Serializable]
    public class GameSessionData
    {
        public string dateTime;
        public int score;
        public float gameDuration;
        public float maxVolumeInhale;
        public float avgFlow;
        public float forceMul;
        public List<Vector2> drawnPath;
        public List<Vector2> coinPath;
    }

    [System.Serializable]
    public class UserDataCollection
    {
        public List<UserData> users = new List<UserData>();
    }
}
