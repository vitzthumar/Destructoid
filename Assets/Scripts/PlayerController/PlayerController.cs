using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour {

    Rigidbody playerRigidbody;

    // inputs
    [HideInInspector] public Vector2 rotationAxis;
    [HideInInspector] public bool incrementColor;
    [HideInInspector] public int color;
    [HideInInspector] Material playerMaterial;

    // speed
    public float baseSpeed = 22f;
    public float speedMinMaxRange = 6f;
    public float currentSpeed;
    [HideInInspector] public int speedControl;
    [HideInInspector] public float modifier;

    // rotation
    public float rotationSpeed = 105f;

    // distance and points
    private Vector3 lastPosition;
    public float distanceTraveled;
    public int destructoidsDestroyed;

    // Use this for initialization
    void Awake () {
        playerRigidbody = gameObject.GetComponent<Rigidbody>();
        playerMaterial = GetComponent<Renderer>().material;

        // randomize the player's initial color and set it
        color = Random.Range(0, 4);
        SetColor();

        // initialize speed to be the base speed
        currentSpeed = baseSpeed;
	}

    // Keeps track of the distance traveled by the player
    void LateUpdate() {
        distanceTraveled += Vector3.Distance(playerRigidbody.transform.position, lastPosition);
        lastPosition = playerRigidbody.transform.position;
    }
    
    // Method used to update the player's rotation and position
    void FixedUpdate() {
        if (GameManager.instance.gameRunning) {
            // move the object forward in space at a rate dependent on EEG feedback
            playerRigidbody.transform.position = (playerRigidbody.transform.position + playerRigidbody.transform.forward * Time.deltaTime * currentSpeed);

            // update the rotation if the input permits
            if (rotationAxis.magnitude != 0) {
                UpdateRotation();
            }
        }
    }

    // Method used to update the player's speed based on the current appropriate EEG value
    public void UpdateSpeed(float eegValue) {
        // make this also available for the camera to use
        modifier = (eegValue - 50) / 50f;
        // adjust the current speed
        currentSpeed = baseSpeed + (speedMinMaxRange * modifier);        
    }

    // Method used for the rotation of the player as well as restricting rotational freedoms, keeping the player facing one direction on the z axis
    private void UpdateRotation() {
        // rotate the player about the x and y axes 
        playerRigidbody.transform.Rotate(-rotationAxis.y * rotationSpeed * Time.deltaTime, 0f, 0f, Space.Self);
        playerRigidbody.transform.Rotate(0f, rotationAxis.x * rotationSpeed * Time.deltaTime, 0f, Space.Self);
    }

    // Method used to change the material of the player
    public void SetColor() {
        if (color == 0) {
            playerMaterial.color = Color.green;
        }
        if (color == 1) {
            playerMaterial.color = Color.red;
        }
        if (color == 2) {
            playerMaterial.color = Color.yellow;
        }
        if (color == 3) {
            playerMaterial.color = Color.blue;
        }
    }

    // Will be called whenever the player collides with another object
    private void OnTriggerEnter(Collider other) {
        
        // the player hit the bounding area wall
        if (other.gameObject.CompareTag("BoundingIco")) {
            // hit the edge, end game
            StartCoroutine("FadeToGray");
            GameManager.instance.EndGame();
        }

        if (other.gameObject.CompareTag("Destructoid")) {
            if (other.gameObject.GetComponent<DestructoidController>().color == color) {
                // player and destructoid are the same color
                destructoidsDestroyed++;
                other.GetComponent<DestructoidController>().DestroyDestructoid();
                // spawn a new destructoid
                GameManager.instance.InstantiateDesctructoid();
            }
            else {
                // player and destructoid are not the same color, end game
                StartCoroutine("FadeToGray");
                GameManager.instance.EndGame();
            }
        }
    }

    IEnumerator FadeToGray() {
        Color lerpedColor;
        while (playerMaterial.color != Color.gray) {
            lerpedColor = Color.Lerp(playerMaterial.color, Color.gray, Mathf.PingPong(Time.time, .03f));
            playerMaterial.color = lerpedColor;
            yield return null;
        }   
    }
}
