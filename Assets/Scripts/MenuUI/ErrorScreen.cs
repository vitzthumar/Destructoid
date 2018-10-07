using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ErrorScreen : MonoBehaviour {

    [SerializeField] Button mainMenuButton;
    [SerializeField] Image backgroundImage;

    // Make the button uninteractable
    private void Awake() {
        if (GameManager.instance.playMode == 0) {
            mainMenuButton.interactable = false;
        }
    }

    // Return to main menu
    public void ReturnMainMenu() {
        GameManager.instance.LoadScene("MainMenu");
    }

    // Fade this screen in
    public void FadeIn() {
        StartCoroutine("FadeMeIn");
        // make the button accessible
        mainMenuButton.interactable = true;
    }

    // Coroutine which fades this panel in
    IEnumerator FadeMeIn() {
        Text[] backgroundTexts = this.gameObject.GetComponentsInChildren<Text>();
        Color antiLerpedColor;
        // fade all text and image objects in
        while (backgroundImage.color != Color.white) {
            antiLerpedColor = Color.Lerp(backgroundImage.color, Color.white, (Time.deltaTime * 4f));
            backgroundImage.color = antiLerpedColor;
            foreach (Text text in backgroundTexts) {
                text.color = antiLerpedColor;
            }
            yield return null;
        }
    }
}
