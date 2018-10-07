using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour {

    [SerializeField] Image scoreBackground;
    [SerializeField] Text scoreText;

    public void UpdateScore(int points) {
        scoreText.text = points.ToString();
    }

    // Fade this UI out
    public void FadeOut() {
        StartCoroutine("FadeIt");
    }
    IEnumerator FadeIt() {
        Color lerpedColor;
        while (scoreBackground.color != Color.clear) {
            lerpedColor = Color.Lerp(scoreBackground.color, Color.clear, Mathf.PingPong(Time.time, .035f));
            scoreBackground.color = lerpedColor;
            scoreText.color = lerpedColor;
            yield return null;
        }
    }
}
