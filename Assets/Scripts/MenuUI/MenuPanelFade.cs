using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuPanelFade : MonoBehaviour {

    Text[] menuPanelTexts;
    Image[] menuPanelImages;
    Color fadeOutColor;

    public void FadeOut() {
        menuPanelTexts = this.gameObject.GetComponentsInChildren<Text>();
        menuPanelImages = this.gameObject.GetComponentsInChildren<Image>();
        foreach (Text text in menuPanelTexts) {
            text.color = Color.white;
        }
        foreach (Image image in menuPanelImages) {
            image.color = Color.white;
        }
        StartCoroutine("FadeMeOut");
    }

    IEnumerator FadeMeOut() {
        // fade all text and image objects out
        while (menuPanelTexts[0].color != Color.clear) {
            fadeOutColor = Color.Lerp(menuPanelTexts[0].color, Color.clear, (Time.deltaTime * 4.5f));
            foreach (Text text in menuPanelTexts) {
                text.color = fadeOutColor;
            }
            foreach (Image image in menuPanelImages) {
                image.color = fadeOutColor;
            }
            yield return null;
        }
    }
}
