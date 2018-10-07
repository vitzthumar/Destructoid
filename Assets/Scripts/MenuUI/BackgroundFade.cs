using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundFade : MonoBehaviour {

    [SerializeField] Text[] backgroundTexts;
    Color fadeInColor;
    IEnumerator fadeMeIn;

    // Accessible method to fade this UI in
    public void FadeIn() {
        foreach (Text text in backgroundTexts) {
            text.color = Color.clear;
        }
        fadeMeIn = FadeMeIn();
        StartCoroutine(fadeMeIn);
    }

    // Coroutine to do the job
    IEnumerator FadeMeIn() {
        // fade the background text in depending on the game mode
        while (backgroundTexts[0].color != Color.white) {
            fadeInColor = Color.Lerp(backgroundTexts[0].color, Color.white, (Time.deltaTime * 4.5f));
            if (GameManager.instance.playMode == 0) {
                backgroundTexts[0].color = fadeInColor;
            }
            else {
                foreach (Text text in backgroundTexts) {
                    text.color = fadeInColor;
                }
            }
            yield return null;
        }
        // everything is ready to go, load the play scene!
        GameManager.instance.LoadScene("PlayScene");
    }
}
