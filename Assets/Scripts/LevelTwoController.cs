using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

enum GameState { Intro, InProgress, Lost, Won };

[DisallowMultipleComponent]
public class LevelTwoController : MonoBehaviour {

	[SerializeField] Text timerText;
	[SerializeField] Text endGameText;
	[SerializeField] Image cameraOverlay;
	[SerializeField] EnemyLine enemyLine;
	[SerializeField] float winTime = 180;

	GameState gameState = GameState.Intro;
	float timer = 0;
	bool endConversationFinished = false;

	void Start () {

		ConversationManager.Instance.StartConversation(this.GetComponent<ConversationComponent>().Conversations[0]);
		if(timerText) { timerText.text = "3:00"; }
	}

	void Update () {

		UpdateDialogue();
		if(gameState == GameState.InProgress) { CheckEndGameConditions(); }
		if(endConversationFinished) { FadeScreenToBlack(); }
	}

	void UpdateDialogue () {

		if(ConversationManager.Instance._IsTalking()) {

			if(Input.anyKeyDown) {

				ConversationManager.Instance.stepSpeed = 0f;
			}

			if(Input.anyKeyDown && ConversationManager.Instance._GetStepCoroutine() == false) {

				ConversationManager.Instance._SetNextLine(true);
			}
		}

		// Check if start convo is finished
		if(ConversationManager.Instance._IsTalking() == false && gameState == GameState.Intro) {

			gameState = GameState.InProgress;
		}
		// Check if win convo is finished
		else if(ConversationManager.Instance._IsTalking() == false && gameState != GameState.InProgress) {

			endConversationFinished = true;
		}
	}

	void CheckEndGameConditions () {

		if(timer < winTime) {

			timer += Time.deltaTime;
			float remainingTime = winTime - timer;
			if(timerText) { timerText.text = ((int) remainingTime / 60).ToString("0") + ":" + ((int) remainingTime % 60).ToString("00"); }
		}
		else {

			enemyLine.StopAllCoroutines();
			enemyLine.StopMovement();
			ParticleSystem[] fightClouds = enemyLine.GetComponentsInChildren<ParticleSystem>();
			foreach(ParticleSystem fightcloud in fightClouds) { fightcloud.Stop(); }
			AudioSource[] enemyLineAudio = enemyLine.GetComponentsInChildren<AudioSource> ();
			foreach (AudioSource audioSource in enemyLineAudio) { audioSource.Stop(); }
			gameState = GameState.Won;
			ConversationManager.Instance.StartConversation(this.GetComponent<ConversationComponent>().Conversations[1]);
		}
	}

	void FadeScreenToBlack () {

		float newAlpha = cameraOverlay.color.a + Time.deltaTime;
		if(newAlpha < 1) {

			cameraOverlay.color = new Color(0, 0, 0, newAlpha);
		}
		else {

			if(gameState == GameState.Won) {

				SceneManager.LoadScene(0);
			}
			else {

				SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
			}
		}
	}

	public void LostGame () {

		if(gameState != GameState.Lost) {
			
			endGameText.gameObject.SetActive(true);
			cameraOverlay.color = Color.clear;
			gameState = GameState.Lost;
			ConversationManager.Instance.StartConversation(this.GetComponent<ConversationComponent>().Conversations[2]);
		}
	}
}
