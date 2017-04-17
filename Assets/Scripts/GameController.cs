using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class GameController : MonoBehaviour {

	Text gameOverText;
	bool gameRunning = false;

	void Awake () {

		//if(gameOverText == null) { gameOverText = GameObject.Find("Game Over Text").GetComponent<Text>(); }
		//gameOverText.gameObject.SetActive(false);
		gameRunning = true;
	}

	void Update () {

		if(!gameRunning && Input.anyKeyDown) {

			SceneManager.LoadScene(0);
		}
	}

	public void PlayersWin () {

		gameOverText.text = "Army Repelled!";
		gameOverText.gameObject.SetActive(true);
		gameRunning = false;
	}

	public void PlayersLose () {

		gameOverText.text = "Game Over!";
		gameOverText.gameObject.SetActive(true);
		gameRunning = false;
	}
}
