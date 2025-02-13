using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UserManager : MonoBehaviour
{
    public TMP_Dropdown userDropdown;
    public TMP_InputField nameInput;
    public Button addUserButton;
    public Button deleteUserButton;
    public Button startGameButton;
    public Button exit;

    private List<string> users = new List<string> ();
    private string selectedUser = "";

    // Start is called before the first frame update
    void Start()
    {
        nameInput.text = "";
        nameInput.interactable = true;

        LoadUsers();
        UpdateDropdown();

        nameInput.onValueChanged.AddListener(OnTextChanged);
        nameInput.onEndEdit.AddListener(OnEndEdit);

        addUserButton.onClick.AddListener(AddUser);
        deleteUserButton.onClick.AddListener(DeleteUser);
        startGameButton.onClick.AddListener(StartGame);
        userDropdown.onValueChanged.AddListener(SelectUser);
        exit.onClick.AddListener(exitGame);
    }

    private void exitGame()
    {
        Application.Quit(); // Exits the application

        // If running in Unity Editor, stop the play mode
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    void OnTextChanged(string text)
    {
        Debug.Log("Typing: " + text);
    }

    void OnEndEdit(string text)
    {
        Debug.Log("Final Input: " + text);
    }

    public void AddUser()
    {
        string newUser = nameInput.text.Trim();

        if (!string.IsNullOrEmpty(newUser) && !users.Contains(newUser))
        {
            users.Add(newUser);
            SaveUsers();
            UpdateDropdown();
            nameInput.text = "";
        }
    }

    public void SelectUser(int index)
    {
        if (users.Count > 0 && index >= 0 && index < users.Count)
        {
            selectedUser = users[index];
            PlayerPrefs.SetString("CurrentUser", selectedUser);
            PlayerPrefs.Save();
        }

        else
        {
            selectedUser = "";
        }
    }

    public void DeleteUser()
    {
        if (!string.IsNullOrEmpty(selectedUser) && users.Contains(selectedUser))
        {
            int selectedIndex = userDropdown.value;
            users.RemoveAt(selectedIndex);
            SaveUsers();
            UpdateDropdown();

            userDropdown.onValueChanged.RemoveAllListeners();


            if (users.Count > 0)
            {
                selectedIndex = Mathf.Clamp(selectedIndex, 0, users.Count - 1);
                userDropdown.value = selectedIndex;
                SelectUser(selectedIndex);
            }

            else
            {
                selectedUser = "";
                userDropdown.value = 0;
            }

            userDropdown.onValueChanged.AddListener(SelectUser);
        }
    }

    void SaveUsers()
    {
        PlayerPrefs.SetString("UserList", string.Join(",", users));
        PlayerPrefs.Save();

    }

    void LoadUsers()
    {
        string savedUsers = PlayerPrefs.GetString("UserList", "");
        if (!string.IsNullOrEmpty(savedUsers))
        {
            users = new List<string>(savedUsers.Split(','));
        }
    }

    void UpdateDropdown()
    {
        userDropdown.ClearOptions();
        userDropdown.AddOptions(users);

        if (userDropdown.options.Count > 0)
        {
            userDropdown.value = 0;
            SelectUser(0);
        }
    }

    public void StartGame()
    {
        if(!string.IsNullOrEmpty(selectedUser))
        {
            Debug.Log("Hi:" + selectedUser);
            SceneManager.LoadScene("Main Menu");
        }
    }
}

