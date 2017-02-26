using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class MainMenuController : MonoBehaviour {

	[SerializeField] float fadeSpeed = 1;
	[SerializeField] Image cameraOverlay;

	bool fading = false;


	void Awake () {

		cameraOverlay.color = Color.clear;
	}

	void Update () {
	
		if(Input.anyKeyDown) {

			fading = true;
		}

		if(fading) {

			float newAlpha = fadeSpeed * Time.deltaTime + cameraOverlay.color.a;
			cameraOverlay.color = new Color(0, 0, 0, newAlpha);

			if(newAlpha >= 1f) {
				
				SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
			}
		}
	}


}
