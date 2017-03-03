using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
public class CameraEffects : MonoBehaviour {

	[Header("Parameters")]
	[SerializeField, Range(0,0.5f)] float maxXAxisShake = 0.1f;
	[SerializeField, Range(0,0.5f)] float maxYAxisShake = 0.1f;
	[SerializeField, Range(0,0.5f)] float shakeDuration = 0.25f;

	float shakeTimer = 0;
	Vector3 startPosition;
	Transform target;

	void Awake () {

		startPosition = this.transform.position;
	}

	void Update () {

		if (shakeTimer > 0) {

			this.transform.position = new Vector3 (this.transform.position.x + Random.Range (-maxXAxisShake, maxXAxisShake), 
				this.transform.position.y + Random.Range (-maxYAxisShake, maxYAxisShake), 
				this.transform.position.z);

			shakeTimer -= Time.deltaTime;

			// Check if need to reset camera position
			if(shakeTimer <= 0) { this.transform.position = startPosition; }
		}

		if (target) {

			this.transform.position = Vector3.MoveTowards (this.transform.position, target.position - 5f * Vector3.forward, 50 * Time.deltaTime);

			if (Vector3.Distance (this.transform.position, target.position) < 1) {
			
				RestoreTimeScale ();
			}
		}
	}

	public void ShakeCamera () {

		shakeTimer = shakeDuration;
	}

	public void ShakeCamera (float duration) {

		shakeTimer = duration;
	}

	public void SetZoomTarget (Transform newTarget) {
		Debug.Log ("HI");
		// Get that noice slow mo going
		Time.timeScale = 0.1f;
		this.GetComponent<Camera> ().orthographic = false;
		target = newTarget;
	}

	void RestoreTimeScale () {

		Time.timeScale = 1f;
	}

	void OnDestroy () {

		RestoreTimeScale ();
	}
}
