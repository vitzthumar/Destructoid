using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class InputController : MonoBehaviour {

    PlayerController playerController;
    CameraController cameraController;
    private bool incrementOrDecrementAxisInUse = false;

	// Get the player controller so that it can be used
	void Awake () {
        playerController = GetComponent<PlayerController>();
        cameraController = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();
	}
	
	// Update is called once per frame
	void Update () {
        if (GameManager.instance.gameRunning) {

            /* 
             * There will be two forms of control: keyboard and Xbox controller
             * 
             * Keyboard will be controlled by the following:
             * WASD and arrow keys control rotation
             * Spacebar will cycle through the available colors
             * Enter will flip the camera
             * 
             * Xbox controller will be controlled by the following:
             * Left and right analog sticks control rotation
             * Left and right triggers will cycle through the available colors
             * A will change to green, B to red, Y to yellow, X to blue
             * Clicking either analog stick will flip the camera
             */

            // rotation
            Vector2 rotationAxis = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            // normalize an input's magnitude if it is greater than 1
            if (rotationAxis.magnitude > 1)
            {
                rotationAxis.Normalize();
            }
            // pass the axis
            playerController.rotationAxis = rotationAxis;

            // color
            // green/A = 0, red/B = 1, yellow/Y = 2, blue/X = 3
            if (Input.GetButtonDown("IncrementColor")) {
                if (playerController.color == 3) {
                    playerController.color = 0;
                }
                else {
                    playerController.color++;
                }
                // set the new color
                playerController.SetColor();
            }

            if (Input.GetAxis("IncrementOrDecrementColor") != 0) {
                if (!incrementOrDecrementAxisInUse) {
                    incrementOrDecrementAxisInUse = true;
                    if (Input.GetAxis("IncrementOrDecrementColor") < 0) {
                        if (playerController.color == 3) {
                            playerController.color = 0;
                        }
                        else {
                            playerController.color++;
                        }
                    }
                    else {
                        if (playerController.color == 0) {
                            playerController.color = 3;
                        }
                        else {
                            playerController.color--;
                        }
                    }
                    playerController.SetColor();
                }
            }
            else {
                incrementOrDecrementAxisInUse = false;
            }

            // check for input on each of the four buttons
            if (Input.GetButtonDown("GreenSelect")) {
                playerController.color = 0;
                playerController.SetColor();
            }
            if (Input.GetButtonDown("RedSelect")) {
                playerController.color = 1;
                playerController.SetColor();
            }
            if (Input.GetButtonDown("YellowSelect")) {
                playerController.color = 2;
                playerController.SetColor();
            }
            if (Input.GetButtonDown("BlueSelect")) {
                playerController.color = 3;
                playerController.SetColor();
            }

            // check for input to flip the camera
            if (Input.GetButtonDown("FlipCamera")) {
                cameraController.FlipCamera();
            }

            // pause game
            if (Input.GetButtonDown("Pause")) {
                GameManager.instance.PauseGame();
            }
        }
    }
}
