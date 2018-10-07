using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    
    GameObject player;
    Rigidbody playerRigidbody;
    PlayerController playerController;

    public float baseDistance = 7.5f;
    public float distanceModifier = 4.5f;
    public float desiredDistance;

    // damping applied to the rotation
    public float rotationDamping = 3.0f;
    public float lerpSpeed = 1.75f;

    private bool flipped = false;

    // Use this for initialization
    void Awake () {
        player = GameObject.Find("Player");
        playerRigidbody = player.GetComponent<Rigidbody>();
        playerController = player.GetComponent<PlayerController>();

        transform.position = new Vector3(playerRigidbody.position.x, playerRigidbody.position.y, playerRigidbody.position.z - CalculateDistance());
    }

    // Method called later to update the camera's position behind the player
    private void LateUpdate() {

        // where the camera "wants" to be
        Vector3 targetPosition = playerRigidbody.position - (playerRigidbody.transform.forward * CalculateDistance());

        // move to the target position from the current position at a predefined speed
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * lerpSpeed);

        if (!flipped) {
            // look at the player
            transform.LookAt(playerRigidbody.transform, playerRigidbody.transform.up);
        } else {
            transform.LookAt(playerRigidbody.position - (playerRigidbody.transform.forward * 50));
        }
    }
    
    // Calculate the distance the camera should be from the player depending on its current speed
    private float CalculateDistance() {
        return baseDistance + (distanceModifier * playerController.modifier);
    }

    public void FlipCamera() {
        flipped = !flipped;
        transform.rotation *= Quaternion.Euler(0, 90, 0);
    }
}
