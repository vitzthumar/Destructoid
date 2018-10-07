using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartPanelFade : MonoBehaviour {

    [SerializeField] Text[] backgroundTexts;
    Image startPanelBackground;
    Color fadeOutColor;
    Color fadeInColor;

    public void FadeOut() {
        startPanelBackground = this.gameObject.GetComponent<Image>();
        StartCoroutine("FadeMeOut");
    }

    IEnumerator FadeMeOut() {
        while (startPanelBackground.color != Color.clear) {
            fadeOutColor = Color.Lerp(startPanelBackground.color, Color.clear, (Time.deltaTime * 5f));
            startPanelBackground.color = fadeOutColor;
            if (GameManager.instance.playMode == 0) {
                backgroundTexts[0].color = fadeOutColor;
                backgroundTexts[1].color = Color.clear;
                backgroundTexts[2].color = Color.clear;
            }
            else {
                foreach (Text text in backgroundTexts) {
                    text.color = fadeOutColor;
                }
            }
            yield return null;
        }
        // disable the error panel
        GameObject.Find("EndGamePanelError").SetActive(false);
        // let's play some Destructoid!
        GameManager.instance.StartGame();
    }
}
