using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Connection : MonoBehaviour {

    [SerializeField] Image connectionBackground;
    [SerializeField] Image connectionIcon;
    [SerializeField] Sprite[] connectionIcons;

    void Awake() {
        if (GameManager.instance.playMode == 0) {
            connectionIcon.enabled = false;
        }
        connectionBackground.enabled = false;
    }

    public void UpdateBackground() {
        if (GameManager.instance.playMode != 0) {
            connectionBackground.enabled = true;
        }
    }

    // Set the connection icon to the current status of the MindWaveMobile
    public void UpdateSignalIcons(int signalIcon) {
        connectionIcon.sprite = connectionIcons[signalIcon];
    }

    // Fade this UI out
    public void FadeOut() {
        StartCoroutine("FadeIt");
    }
    IEnumerator FadeIt() {
        Color lerpedColor;
        while (connectionBackground.color != Color.clear) {
            lerpedColor = Color.Lerp(connectionBackground.color, Color.clear, Mathf.PingPong(Time.time, .035f));
            connectionBackground.color = lerpedColor;
            connectionIcon.color = lerpedColor;
            yield return null;
        }
    }

}
