using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class EndScreen : MonoBehaviour {

    [SerializeField] Button mainMenuButton;
    [SerializeField] Text score;
    [SerializeField] Text distanceTraveled;
    [SerializeField] Text destructoidsDestroyed;
    [SerializeField] Text timeEndured;
    [SerializeField] Text averageAttention;
    [SerializeField] Text averageMeditation;
    [SerializeField] Image averageEEGBackground;
    [SerializeField] Text averageEEGText;

    // Make the button uninteractable
    private void Awake() {
        mainMenuButton.interactable = false;
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

    public void UpdateStats() {
        score.text = GameManager.instance.points.ToString();
        distanceTraveled.text = GameManager.instance.playerController.distanceTraveled.ToString();
        destructoidsDestroyed.text = GameManager.instance.playerController.destructoidsDestroyed.ToString();
        timeEndured.text = ((int)GameManager.instance.timeEndured).ToString();
        if (GameManager.instance.playMode == 0) {
            averageAttention.enabled = false;
            averageMeditation.enabled = false;
            averageEEGText.enabled = false;
            averageEEGBackground.enabled = false;
        }
        // get the list of all the EEG values
        List<int> eegList = GameManager.instance.averageEEG;

        if (GameManager.instance.playMode == 1) {
            averageMeditation.enabled = false;
            averageEEGText.text = ((int)eegList.Average()).ToString() + "%";
        }
        if (GameManager.instance.playMode == 2) {
            averageAttention.enabled = false;
            averageEEGText.text = ((int)eegList.Average()).ToString() + "%";
        }
    }

    // Coroutine which fades this panel in
    IEnumerator FadeMeIn() {
        Text[] backgroundTexts = this.gameObject.GetComponentsInChildren<Text>();
        Image[] backgroundImages = this.gameObject.GetComponentsInChildren<Image>();
        Color antiLerpedColor;
        // fade all text and image objects in after a defined time
        yield return new WaitForSeconds(2.5f);
        while (backgroundTexts[0].color != Color.white) {
            antiLerpedColor = Color.Lerp(backgroundImages[0].color, Color.white, (Time.deltaTime * 4f));
            foreach (Text text in backgroundTexts) {
                text.color = antiLerpedColor;
            }
            foreach (Image image in backgroundImages) {
                image.color = antiLerpedColor;
            }
            yield return null;
        }
    }
}
