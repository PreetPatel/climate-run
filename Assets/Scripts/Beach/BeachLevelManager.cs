﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeachLevelManager : MonoBehaviour
{

    public static BeachLevelManager Instance { set; get; }

    private bool isGameStarted = false;
    private bool startedShaking = false;
    private PlayerMotor playerMotor;
    private CameraMotor cameraMotor;
    private CompanionMotor compMotor;
    private bool garbageCollected = false;
    private float timeSinceGarbageCollected = 0.0f;

    // UI and the UI fields
    public Text scoreText;
    public Text garbageText;
    public Text informationText;
    public Text livesText;
    public Slider pollutionSlide;
    private float score = 0;
    private float garbage = 0;
    private float modifier = 1.0f;

    //Death menu
    public Animator deathMenuAnim;
    public Text deadScoreText, deadGarbageText;

    private void Awake()
    {
        Instance = this;
        pollutionSlide.value = GarbageSpawner.garbageMultiplier;
        informationText.text = "Press any key to start";
        playerMotor = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMotor>();
        cameraMotor = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraMotor>();
        compMotor = GameObject.FindGameObjectWithTag("Companion").GetComponent<CompanionMotor>();
        scoreText.text = "Score : " + score.ToString("0");
        garbageText.text = "Garbage : " + garbage.ToString();
        livesText.text = "Lives Remaining : 3";
        
        //modifierText.text = "Modifer : x" + modifier.ToString("0.0");

    }

    private void Update()
    {
        if (Input.anyKey && !isGameStarted)
        {
            isGameStarted = true;
            playerMotor.StartRunning();
            cameraMotor.StartFollowing();
            compMotor.StartRunning();
            informationText.text = "";
        }

        if (isGameStarted)
        {
            if (!garbageCollected)
            {
                float garbMulti = GarbageSpawner.garbageMultiplier;
                GarbageSpawner.garbageMultiplier = Mathf.Clamp(garbMulti += 0.001f, 0.0f, 1.0f);
                pollutionSlide.value = GarbageSpawner.garbageMultiplier;
            }
            score += (Time.deltaTime * modifier);
            scoreText.text = "Score : " + score.ToString("0");

            /*            if (score > 60)
                        {
                            SceneManager.LoadScene("Antarctica_EndingCutscene");
                        }*/
            if(score > 25)
            {
                informationText.text = "";
            }
            else if (score > 20)
            {
                informationText.text = "Good luck";
            }
            else if (score > 15)
            {
                informationText.text = "More obstacles will appear on your track as the pollution meter rises";
            }
            else if (score > 10)
            {
                informationText.text = "The pollution meter rises whenever you don't collect a garbage";
            }
            else if (score > 5)
            {
                informationText.text = "Garbage will pile up if you leave them unhandled";
            }
            else if (score > 0)
            {
                informationText.text = "Flippy, you have to collect the garbage people throw on the beach!";
            }

            timeSinceGarbageCollected += Time.deltaTime;
            Debug.Log(timeSinceGarbageCollected);
            if(timeSinceGarbageCollected > 5.0f)
            {
                garbageCollected = false;
                timeSinceGarbageCollected = 0.0f;
            }

        }

    }

    public void getGarbage()
    {
        garbage++;
        garbageText.text = "Garbage : " + garbage.ToString();
        garbageCollected = true;
        float garbMulti = GarbageSpawner.garbageMultiplier;
        GarbageSpawner.garbageMultiplier = Mathf.Clamp(garbMulti -= 0.25f, 0.0f, 1.0f);
        pollutionSlide.value = GarbageSpawner.garbageMultiplier;
    }
    
    public void updateLives(float livesAmount)
    {
        livesText.text = "Lives Remaining : " + livesAmount.ToString("0");
    }

    // public void updatemodifer( float modifieramount)
    // {
    //     modifier = 1.0f + modifieramount;
    //     modifiertext.text = "modifer : x" + modifier.tostring("0.0");
    // }

    public void OnDeath()
    {
        deadScoreText.text = "Score: " + score.ToString("0");
        deadGarbageText.text = "Garbage Collected: " + garbage.ToString("0");
        deathMenuAnim.SetTrigger("Dead");
        //completely pause the game
        //Time.timeScale = 0;
    }

    public void OnRetryButton()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene("Beach");
	}

    public void OnExitButtonPress()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
