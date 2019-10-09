﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BeachPlayerMotor : MonoBehaviour
{
    private const float LANE_DISTANCE = 2.5f;
    private const float TURN_SPEED = 0.05F;

    // Animation
    private Animator anim;

    // Wait for player
    private bool isRunning = false;

    // Movements
    private CharacterController controller;
    private float jumpForce = 6.0f;
    private float gravity = 12.0f;
    private float verticalVelocity;


    // speed modifier
    private float speed = 7.0f;

    // 0 is left, 1 is middle, 2 is right
    private int lane = 1;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!isRunning)
        {
            return;
        }
        // Check which lane we should be
        if(Input.GetKeyDown(KeyCode.A))
        {
            MoveLane(false);
        }
        if(Input.GetKeyDown(KeyCode.D))
        {
            MoveLane(true);
        }

        // Calculate where we will be in the future
        Vector3 targetPosition = transform.position.z * Vector3.forward;
        if(lane == 0)
        {
            targetPosition += Vector3.left * LANE_DISTANCE;
        } else if (lane == 2)
        {
            targetPosition += Vector3.right * LANE_DISTANCE;
        }

        // Calculating our move vector
        Vector3 moveVector = Vector3.zero;

        // where we should be - where we are to get a normalised vector. 
        moveVector.x = (targetPosition - transform.position).normalized.x * speed;

        bool isGrounded = IsGrounded();
        anim.SetBool("Grounded", isGrounded);

        // Calculate Y
        if (isGrounded) // if grounded
        {
            verticalVelocity = -0.1f;

            if (Input.GetKeyDown(KeyCode.W))
            {
                // Jump
                anim.SetTrigger("Jump");
                verticalVelocity = jumpForce;
            }
            else if  (Input.GetKeyDown(KeyCode.S))
            {
                // Slide
                StartSliding();
                Invoke("StopSliding", 1.0f);
            }
        }
        else // fast fall
        {
            verticalVelocity -= (gravity * Time.deltaTime);

            if (Input.GetKeyDown(KeyCode.S))
            {
                verticalVelocity = -jumpForce;
            }
        }

        moveVector.y = verticalVelocity;
        moveVector.z = speed;

        // Move the actual character
        controller.Move(moveVector * Time.deltaTime);

        // Rotate the character while moving
        Vector3 dir = controller.velocity;
        if (dir != Vector3.zero)
        {
            dir.y = 0;
            transform.forward = Vector3.Lerp(transform.forward, dir, TURN_SPEED);
        }

    }

    private void MoveLane(bool goRight)
    {

        // Switches lanes, and is clamped between 0 and 2
        lane += (goRight) ? 1 : -1;
        lane = Mathf.Clamp(lane, 0, 2);
    }

    private bool IsGrounded()
    {
        Ray groundRay = new Ray(
            new Vector3(
                controller.bounds.center.x,
                (controller.bounds.center.y - controller.bounds.extents.y) + 0.2f,
                controller.bounds.center.z),
                Vector3.down
        );

        return Physics.Raycast(groundRay, 0.2f + 0.1f);
    }

    public void StartRunning()
    {
        isRunning = true;
        anim.SetTrigger("StartRunning");
    }

    public void StartSliding()
    {
        anim.SetBool("Sliding", true);
        controller.height /= 2;
        controller.center = new Vector3(controller.center.x, controller.center.y / 2, controller.center.z);
    }

    public void StopSliding()
    {
        anim.SetBool("Sliding", false);
        controller.height *= 2;
        controller.center = new Vector3(controller.center.x, controller.center.y * 2, controller.center.z);
    }

    private void Crash()
    {
        anim.SetTrigger("Death");
        isRunning = false;
        LevelManagerBeach.IsDead = true;
        //SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        switch (hit.gameObject.tag)
        {
            case "Obstacle":
                Crash();
                break;
        }
    }
}