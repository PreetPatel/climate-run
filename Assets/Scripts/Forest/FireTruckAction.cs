﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Author: Marc Burgess
 */
public class FireTruckAction : MonoBehaviour
{
    private bool triggeredAlready = false;
    private bool transitioning = false;
    private bool growingBushes = false;
    private int framesSinceTrigger = 0;
    private int framesSinceExtinguish = 0;
    private int framesSinceBushGrow = 0;
    private Dictionary<SplashFireCombo, bool> extinguishableFireParticles;
    private Dictionary<GrowingBush, bool> grownBushes;

    internal void doWaterSpray()
    {
        if (!triggeredAlready)
        {
            triggeredAlready = true;
            Time.timeScale = 0.30f;
            transitioning = true;

            extinguishableFireParticles = new Dictionary<SplashFireCombo, bool>();
            SplashFireCombo[] fireParticles = GetComponentsInChildren<SplashFireCombo>();
            for (int i = 0; i < fireParticles.Length; i++)
            {
                extinguishableFireParticles.Add(fireParticles[i], false);
            }

            grownBushes = new Dictionary<GrowingBush, bool>();
            GrowingBush[] bushes = GetComponentsInChildren<GrowingBush>();
            for (int i = 0; i < bushes.Length; i++)
            {
                grownBushes.Add(bushes[i], false);
            }
        }
    }

    void Update()
    {
        if(transitioning)
        {
            if (framesSinceTrigger > 170)
            {
                transitioning = false;
                Time.timeScale = 1.0f;
                return;
            }

            if (framesSinceTrigger > 130 && !growingBushes)
            {
                growingBushes = true;
            }

            framesSinceExtinguish++;

            if (framesSinceExtinguish >= 5)
            {
                framesSinceExtinguish = 0;

                foreach (KeyValuePair<SplashFireCombo, bool> particleHasDespawned in extinguishableFireParticles)
                {
                    SplashFireCombo particle = particleHasDespawned.Key;
                    if (!particleHasDespawned.Value)
                    {
                        extinguishableFireParticles[particle] = true;
                        particle.Trigger();
                        break;
                    }
                }
            }

            framesSinceTrigger++;
        }

        if (growingBushes)
        {
            bool allBushesGrown = false;

            framesSinceBushGrow++;

            if (framesSinceBushGrow >= 3)
            {
                allBushesGrown = true;
                foreach (KeyValuePair<GrowingBush, bool> bushGrown in grownBushes)
                {
                    GrowingBush bush = bushGrown.Key;
                    if (!bushGrown.Value)
                    {
                        grownBushes[bush] = true;
                        bush.Trigger();
                        allBushesGrown = false;
                        break;
                    }
                }

                framesSinceBushGrow = 0;
            }

            if (allBushesGrown)
            {
                growingBushes = false;
            }
        }
    }
}
