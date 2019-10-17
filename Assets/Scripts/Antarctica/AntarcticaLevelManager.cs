﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AntarcticaLevelManager : MonoBehaviour
{
    public static AntarcticaLevelManager Instance {set; get;}

    private bool isGameStarted = false;
    private PlayerMotor playerMotor;
    private CameraMotor cameraMotor;

    // UI and the UI fields
    public Text scoreText;
    public Text informationText;
    private float score = 0;

    //Death menu
    public Animator deathMenuAnim;
    public Text deadScoreText;
    public Button pauseButton;

    private void Awake()
    {
        Instance = this;
        scoreText.text = score.ToString();
        informationText.text = "Touch to start";
        playerMotor = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMotor>();
        cameraMotor = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraMotor>();
    }

    private void Update()
    {
        if (Input.anyKey && !isGameStarted)
        {
            isGameStarted = true;
            playerMotor.StartRunning();
            cameraMotor.StartFollowing();    
        }

        if (isGameStarted)
        {
            score += Time.deltaTime;
            scoreText.text = score.ToString("0");

            // refactor later
            if (score > 60)
            {
               // SceneManager.LoadScene("Antarctica_EndingCutscene");
            }
            else if (score > 15)
            {
                informationText.text = "The ice is melting!";
            }
            else if (score > 10)
            {
                informationText.text = "Swipe up to jump";
            }
            else if (score > 5)
            {
                informationText.text = "Swipe down to slide";
            }
            else if (score > 0)
            {
                informationText.text = "Swipe to move";
            }

        

        }
    }

    public void OnDeath()
    {
        deadScoreText.text = "Score: " + score.ToString("0");
        deathMenuAnim.SetTrigger("Dead");
        isGameStarted = false;
        scoreText.gameObject.SetActive(false);
        informationText.gameObject.SetActive(false);
        pauseButton.gameObject.SetActive(false);
    }

    public void OnRetryButton()
    {
        Debug.Log("Retry");
        SceneManager.LoadScene("Antarctica");
    }

    public void OnExitButtonPress()
    {
        Debug.Log("Exit");
        SceneManager.LoadScene("MainMenu");
    }
}