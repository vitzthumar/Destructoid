using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slider : MonoBehaviour {

    [SerializeField] Image sliderBackground;
    [SerializeField] Image redFill;
    [SerializeField] Image blueFill;


    // Use this for initialization
    public void UpdateSlider () {
        sliderBackground = GetComponent<Image>();
        sliderBackground.enabled = false;
        if (GameManager.instance.playMode == 0) {
            sliderBackground.enabled = false;
            redFill.enabled = false;
            blueFill.enabled = false;
        }
        if (GameManager.instance.playMode == 1) {
            sliderBackground.enabled = true;
            redFill.enabled = true;
            blueFill.enabled = false;
        }
        if (GameManager.instance.playMode == 2) {
            sliderBackground.enabled = true;
            redFill.enabled = false;
            blueFill.enabled = true;
        }
    }

    public void UpdateSlider(float eegValue) {
        redFill.fillAmount = eegValue / (float)100;
        blueFill.fillAmount = eegValue / (float)100;
    }

    // Fade this UI out
    public void FadeOut() {
        StartCoroutine("FadeIt");
    }
    IEnumerator FadeIt() {
        Color lerpedColor;
        while (sliderBackground.color != Color.clear) {
            lerpedColor = Color.Lerp(sliderBackground.color, Color.clear, Mathf.PingPong(Time.time, .035f));
            sliderBackground.color = lerpedColor;
            redFill.color = lerpedColor;
            blueFill.color = lerpedColor;
            yield return null;
        }
    }
}
