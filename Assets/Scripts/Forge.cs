using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum Weapon { Sword, Axe, Shield, None }

[DisallowMultipleComponent]
public class Forge : MonoBehaviour {

	[Header("Parameters")]
	[SerializeField] int maxMetalAmount = 5;

	[Header("References")]
	[SerializeField] Image metalBarFill;
	[SerializeField] GameObject forgedIngotPrefab;

	int metalAmount = 0;
	Weapon weaponForging;


	void Awake () {

		metalBarFill.fillAmount = 0;
	}

	public void UpdateMetalAmount (int amount) {

		metalAmount += amount;
		metalBarFill.fillAmount = ((float)metalAmount / maxMetalAmount);
	}

	public void RetrieveIngot (PlayerController player) {

		if (metalAmount > 0) {

			GameObject newIngot = Instantiate (forgedIngotPrefab, Vector3.zero, Quaternion.identity) as GameObject;
			player.ReceiveItem (newIngot);
			metalAmount--;
			metalBarFill.fillAmount = ((float)metalAmount / maxMetalAmount);
		}
	}
}
