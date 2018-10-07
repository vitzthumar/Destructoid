using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuitButtonLoader : MonoBehaviour {

    [SerializeField] Button quitButton;

	void Awake () {
        UnityEngine.Events.UnityAction quitGame = () => { GameManager.instance.QuitGame(); };
        quitButton.onClick.AddListener(quitGame);
    }
}
