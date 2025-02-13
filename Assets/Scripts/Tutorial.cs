using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class Tutorial : MonoBehaviour
{

    public GameObject Panel;
    public GameObject calibrationImage;
    public GameObject gameImage;
    public GameObject loseHeartImage;
    public GameObject topImage;
    public GameObject groundImage;

    //boolean
    public bool calibrate;
    public bool game;
    public bool loseHeart;
    public bool top;
    public bool ground;

    //buttons
    public Button next;
    public Button back;
    public Button exit;


    // Start is called before the first frame update
    void Start()
    {
        Panel.SetActive(true);
        calibrationImage.SetActive(true);
        gameImage.SetActive(false);
        loseHeartImage.SetActive(false);
        topImage.SetActive(false);
        groundImage.SetActive(false);

        //boolean
        calibrate = true;
        game = false;
        loseHeart = false;
        top = false;
        ground = false;


        back.gameObject.SetActive(false);
        next.gameObject.SetActive(true);

        back.onClick.AddListener(goBack);
        next.onClick.AddListener(goNext);
        exit.onClick.AddListener(goExit);
    }

    private void goExit()
    {
        SceneManager.LoadScene("Main Menu");
    }

    private void goBack()
    {
        if (ground)
        {
            next.gameObject.SetActive(true);
            back.gameObject.SetActive(true);

            groundImage.SetActive(false);
            topImage.SetActive(true);

            ground = false;
            top = true;
            return;
        }

        if (top)
        {
            next.gameObject.SetActive(true);
            back.gameObject.SetActive(true);

            topImage.SetActive(false);
            loseHeartImage.SetActive(true);

            top = false;
            loseHeart = true;
            return;
        }

        if (loseHeart)
        {
            next.gameObject.SetActive(true);
            back.gameObject.SetActive(true);

            loseHeartImage.SetActive(false);
            gameImage.SetActive(true);

            loseHeart = false;
            game = true;
            return;
        }

        if (game)
        {
            next.gameObject.SetActive(true);
            back.gameObject.SetActive(false);

            gameImage.SetActive(false);
            calibrationImage.SetActive(true);

            game = true;
            calibrate = true;
            return;
        }

    }

    private void goNext()
    {
        if (calibrate) //go to gameImage
        {
            back.gameObject.SetActive(true);
            
            calibrationImage.SetActive(false);
            gameImage.SetActive(true);

            calibrate = false;
            game = true;
            return;
        }

        if (game) //go to loseHeartImage
        {
            back.gameObject.SetActive(true);

            gameImage.SetActive(false);
            loseHeartImage.SetActive(true);

            game = false;
            loseHeart = true;
            return;

        }

        if (loseHeart) //go to topImage
        {
            back.gameObject.SetActive(true);

            loseHeartImage.SetActive(false);
            topImage.SetActive(true);

            loseHeart = false;
            top = true;
            return;

        }

        if (top) //go to groundImage
        {
            back.gameObject.SetActive(true);
            next.gameObject.SetActive(false);

            topImage.SetActive(false);
            groundImage.SetActive(true);

            top = false;
            ground = true;
            return;
        }


    }
    
}
