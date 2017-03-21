using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
public class BrokenWeapon : MonoBehaviour {

	[SerializeField] float startFade = 5;
	[SerializeField] float fadeSpeed = 1;
	[SerializeField] Sprite brokenSword;
	[SerializeField] Sprite brokenAxe;

	SpriteRenderer spriteRenderer;
	bool fading = false;

	void Awake () {

		spriteRenderer = this.GetComponent<SpriteRenderer> ();
		spriteRenderer.sprite = (Random.value < 0.5) ? brokenSword : brokenAxe;
	}

	void OnEnable () {

		StartCoroutine (DelayFadeStart ());
	}

	void Update () {

		if(fading && spriteRenderer.color.a > 0) {

			float newAlpha = spriteRenderer.color.a - fadeSpeed * Time.deltaTime;

			if (newAlpha <= 0) {

				Destroy (this.gameObject);
			}
			else {

				spriteRenderer.color = new Color (1, 1, 1, newAlpha);
			}
		}
	}

	IEnumerator DelayFadeStart () {

		yield return new WaitForSeconds (startFade);

		spriteRenderer.color = Color.white;
		fading = true;
	}

	void OnDisable () {

		spriteRenderer.color = Color.white;
	}
}
