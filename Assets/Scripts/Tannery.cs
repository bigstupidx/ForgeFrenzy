using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[DisallowMultipleComponent]
public class Tannery : MonoBehaviour {

	[Header("Parameters")]
	[SerializeField] float maxTanningTime = 7;
	[SerializeField] float tanningSpeed = 1;

	[Header("References")]
	[SerializeField] GameObject tanningBar;
	[SerializeField] Image tanningBarFill;
	[SerializeField] GameObject tannedLeatherPrefab;

	// Privates
	GameObject leatherTanning;
	float tanningTimer = 0;


	void Awake () {

		ResetTanning ();
	}

	void Update () {

		if (leatherTanning) {

			tanningTimer += tanningSpeed * Time.deltaTime;
		}
	}

	public void PlaceLeather () {

		if (leatherTanning == null) {

			tanningBar.SetActive (true);
		}
	}

	public void ResetTanning () {

		tanningTimer = 0;
		tanningBarFill.fillAmount = 0;
		tanningBar.SetActive (false);
	}
}
