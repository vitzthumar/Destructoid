using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    // follow the singleton pattern so that this GameManager may be accessed at all times from anywhere
    public static GameManager instance = null;
    GameObject player;
    public PlayerController playerController;

    // UI stuff
    GameObject startPanel;
    GameObject endPanel;
    GameObject menuPanels;

    // EEG stuff
    MindWaveMobile mwmController;
    public int poorSignal;
    public int attention;
    public int meditation;
    private float oldEEG;
    private float newEEG;
    public float updatedEEG;
    public float eegDuration = 1f;
    private float updateTime;
    // play mode, 0 = no EEG, 1 = attention, 2 = meditation
    public int playMode;
    Slider sliderUI;
    public List<int> averageEEG;

    // helper stuff and points
    [HideInInspector] public bool gameRunning = false;
    public bool paused = false;
    public int points;
    public int stackingPointsPerDestructoid = 100;
    Score scoreUI;
    Connection connectionUI;
    public float timeEndured;

    // destructoid stuff
    [HideInInspector] public int destructoidCount = 0;
    public GameObject destructoid;
    private IEnumerator spawnCoroutine;
    private IEnumerator loadCoroutine;
    private float spawnTimer = 4f;

    // Use this for initialization
    void Awake() {
        
        DontDestroyOnLoad(this.gameObject);
        // make sure there is only ever one game manager
        if (instance == null) {
            instance = this;
        }
        else if (instance != this) {
            Destroy(this.gameObject);
        }
        // get the MindWaveMobile controller
        mwmController = GetComponent<MindWaveMobile>();
    }

    // Start the game!
    public void StartGame() {
        gameRunning = true;
        //reset the points
        points = 0;
        // begin spawning destructoids
        StartDestructoidSpawn();
    }

    // Lerp the EEG value so it doesn't change drastically
    void Update() {
        if (gameRunning) {
            if (playMode != 0) {
                // update the eeg and the slider smoothly if in the appropriate play mode
                updatedEEG = Mathf.SmoothStep(oldEEG, newEEG, (Time.time - updateTime) / 1f);
                sliderUI.UpdateSlider(updatedEEG);
            }
            // update the speed for the player 
            playerController.UpdateSpeed(updatedEEG);
        }
    }

    // Used to log the player's points
    void LateUpdate() {
        if (gameRunning) {
            points = (int)playerController.distanceTraveled + (playerController.destructoidsDestroyed * stackingPointsPerDestructoid);
            scoreUI.UpdateScore(points);
            timeEndured += Time.deltaTime;
        }
    }

    // End the game
    public void EndGame() {
        // stop spawning destructoids
        gameRunning = false;

        // fade out all in-game UI and disconnect the EEG device
        scoreUI.FadeOut();
        if (playMode != 0) {
            mwmController.Disconnect();
            connectionUI.FadeOut();
            sliderUI.FadeOut();
        }

        // stop all the destructoids from moving
        GameObject[] destructoids = GameObject.FindGameObjectsWithTag("Destructoid");
        foreach (GameObject destructoid in destructoids) {
            destructoid.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
        // return back to the main menu, the game ended naturally
        GameObject endGamePanel = GameObject.Find("EndGamePanelNoError");
        endGamePanel.GetComponent<EndScreen>().UpdateStats();
        endGamePanel.GetComponent<EndScreen>().FadeIn();
    }

    // Method to start the spawn coroutine
    private void StartDestructoidSpawn() {
        spawnCoroutine = StartSpawn();
        StartCoroutine(spawnCoroutine);
    }

    // Coroutine that spawns destructoids every X seconds
    IEnumerator StartSpawn() {
        while (gameRunning) {
            yield return new WaitForSeconds(spawnTimer);
            if (gameRunning) {
                InstantiateDesctructoid();
            }
        }
    }
    
    // Method used to pause and unpause the game while it's running
    public void PauseGame() {
        // make sure the game is running
        if (gameRunning) {
            if (paused) {
                // do something if the game is already paused
            }
            else {
                // do something if the game is NOT already paused
            }
        }
    }

    // Method which attempts to spawn a destructoid with various sizes
    public void InstantiateDesctructoid() {

        for (int tries = 0; tries < 5; tries++) {
            // set the random size for this destructoid
            float destructoidSize = Random.Range(7.5f, 35f);
            if (TryDestructoidSpawn(destructoidSize)) {
                break;
            }
        }
    }

    // Method to actually spawn the destructoid
    private bool TryDestructoidSpawn(float destructoidSize) {

        // get information on the player's current position
        Vector3 playerPosition = player.transform.position;
        Vector3 playerDirection = player.transform.forward;
        Quaternion playerRotation = player.transform.rotation;

        // create a position variable where the destructoid could be spawned
        Vector3 possiblePosition;

        // whether or not we can spawn in this position
        bool validPosition = false;
        bool hitEdge = false;

        // try to spawn a destructoid 10 times
        for (int spawnAttempts = 0; spawnAttempts < 10; spawnAttempts++) {

            validPosition = true;

            // get the next possible position
            possiblePosition = playerPosition - (playerDirection * ((spawnAttempts + 2.5f) * destructoidSize));

            // collect all colliders within the radius of the destructoid
            Collider[] colliders = Physics.OverlapSphere(possiblePosition, destructoidSize);

            // go through each collider collected
            foreach (Collider other in colliders) {

                if (other.gameObject.tag == "BoundingIco") {
                    // hit the edge
                    validPosition = false;
                    hitEdge = true;
                }
                if (other.gameObject.tag == "Destructoid" || other.gameObject.tag == "Player") {
                    validPosition = false;
                }
            }

            // hit the icosphere, break out of the for loop and don't spawn anything
            if (hitEdge) {
                return false;
            }
            // the position is valid, spawn then break
            if (validPosition) {
                // spawn the destructiod
                GameObject destructoidInstance = Instantiate(destructoid, possiblePosition, Quaternion.identity);
                // change the size of the destructoid
                destructoidInstance.transform.localScale = new Vector3(destructoidSize, destructoidSize, destructoidSize);
                destructoidInstance.GetComponent<DestructoidController>().destructoidSize = destructoidSize;
                // only spawn one destructoid
                return true;
            }
        }
        return false;
    }
    
    // EEG methods
    void OnUpdatePoorSignal(int value) {
        poorSignal = value;
        if (value < 25) {
            connectionUI.UpdateSignalIcons(0);
        }
        else if (value >= 25 && value < 51) {
            connectionUI.UpdateSignalIcons(4);
        }
        else if (value >= 51 && value < 78) {
            connectionUI.UpdateSignalIcons(3);
        }
        else if (value >= 78 && value < 107) {
            connectionUI.UpdateSignalIcons(2);
        }
        else if (value >= 107) {
            connectionUI.UpdateSignalIcons(1);
        }
    }
    void OnUpdateAttention(int value) {
        if (playMode == 1) {
            oldEEG = newEEG;
            newEEG = value;
            updateTime = Time.time;
            averageEEG.Add(value);
        }
    }
    void OnUpdateMeditation(int value) {
        if (playMode == 2) {
            oldEEG = newEEG;
            newEEG = value;
            updateTime = Time.time;
            averageEEG.Add(value);
        }
        
    }

    // Quit the game
    public void QuitGame() {
        Application.Quit();
    }
    // Start a normal game without EEG feedback
    public void StartNormal() {
        playMode = 0;
        SetupGame();
    }
    // Start a game with attention feedback
    public void StartAttention() {
        playMode = 1;
        SetupGame();
    }
    // Start a game with meditation feedback
    public void StartMeditation() {
        playMode = 2;
        SetupGame();
    }
    // Loads the scene in which the game is being played and checks the EEG device if necessary
    private void SetupGame() {
        GameObject.Find("BackButton").SetActive(false);
        // fade out the MenuPanel's objects and fade the background with text in
        GameObject menuPanels = GameObject.Find("MenuPanels");
        menuPanels.GetComponent<MenuPanelFade>().FadeOut();
        GameObject background = GameObject.Find("Background");
        background.GetComponent<BackgroundFade>().FadeIn();
    }

    // Load the game once in the play scene
    private void LoadGame() {
        // last step before the game can run, gather EEG feedback
        // find the player for later use
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
        sliderUI = GameObject.FindGameObjectWithTag("Slider").GetComponent<Slider>();
        sliderUI.UpdateSlider();
        scoreUI = GameObject.Find("Score").GetComponent<Score>();
        connectionUI = GameObject.Find("Connection").GetComponent<Connection>();
        connectionUI.UpdateBackground();

        if (playMode == 0) {
            updatedEEG = 50f;
        }
        if (playMode == 1) {
            mwmController.Connect();
            mwmController.UpdatePoorSignalEvent += OnUpdatePoorSignal;
            mwmController.UpdateAttentionEvent += OnUpdateAttention;
        }
        if (playMode == 2) {
            mwmController.Connect();
            mwmController.UpdatePoorSignalEvent += OnUpdatePoorSignal;
            mwmController.UpdateMeditationEvent += OnUpdateMeditation;
        }

        IEnumerator checkEEG = CheckEEG();
        StartCoroutine(checkEEG);
    }
    
    // Check to see if the EEG is working and receiving signal
    IEnumerator CheckEEG() { 
        // wait to start until EEG feedback is received
        bool eegError = false;
        if (playMode != 0) {
            float waitTimer = 7.5f;
            while (newEEG == 0) {
                waitTimer -= Time.deltaTime;
                if (waitTimer < 0) {
                    mwmController.Disconnect();
                    eegError = true;
                    GameObject.Find("EndGamePanelError").GetComponent<ErrorScreen>().FadeIn();
                    break;
                }
                yield return null;
            }
        }
        // no problem loading the EEG
        if (!eegError) {
            // fade into the PlayScene then start if no error occurs with EEG feedback
            startPanel = GameObject.Find("StartPanel");
            startPanel.GetComponent<StartPanelFade>().FadeOut();
        }
    }

    // Loads the scene passed as an argument
    public void LoadScene(string sceneName) {
        StopAllCoroutines();
        IEnumerator loadScene = LoadAScene(sceneName);
        StartCoroutine(loadScene);        
    }

    // Coroutine to load the main menu
    IEnumerator LoadAScene(string sceneName) {
        mwmController.Disconnect();
        // load the level
        AsyncOperation asyncLoadLevel = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        while (!asyncLoadLevel.isDone) {
            yield return null;
        }
        // start the next fade if the play scene was loaded
        if (sceneName == "PlayScene") {
            LoadGame();
        }
    }
}
