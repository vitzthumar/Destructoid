using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonLoader : MonoBehaviour {

    private void Awake() {

        // load the functions for each of the game-starting buttons
        Button attentionButton = GameObject.Find("LeftButton").GetComponent<Button>();
        UnityEngine.Events.UnityAction startA = () => { GameManager.instance.StartAttention(); };
        attentionButton.onClick.AddListener(startA);

        Button normalButton = GameObject.Find("MiddleButton").GetComponent<Button>();
        UnityEngine.Events.UnityAction startN = () => { GameManager.instance.StartNormal(); };
        normalButton.onClick.AddListener(startN);

        Button meditationButton = GameObject.Find("RightButton").GetComponent<Button>();
        UnityEngine.Events.UnityAction startM = () => { GameManager.instance.StartMeditation(); };
        meditationButton.onClick.AddListener(startM);
    }
}
