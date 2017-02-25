using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum Weapon { Sword, Axe, Shield, None }

[DisallowMultipleComponent]
public class Forge : MonoBehaviour {

	[Header("Parameters")]
	[SerializeField] int maxMetalAmount = 5;
	[SerializeField] float maxForgeTime = 3;

	[Header("References")]
	[SerializeField] GameObject forgingBar;
	[SerializeField] Image metalBarFill;
	[SerializeField] Image forgingBarFill;
	[SerializeField] GameObject forgedSwordPrefab;
	[SerializeField] GameObject forgedAxePrefab;
	[SerializeField] GameObject forgedShieldPrefab;

	int metalAmount = 0;
	GameObject forgingPlayer;
	Weapon weaponForging;
	float forgeTimer = 0;


	void Awake () {

		metalBarFill.fillAmount = 0;
		ResetForgingBar ();
	}

	public void UpdateMetalAmount (int amount) {

		metalAmount += amount;
		metalBarFill.fillAmount = ((float)metalAmount / maxMetalAmount);
	}

	public void SetForgingPlayer (GameObject player, Weapon weapon) {

		forgingPlayer = player;
		weaponForging = weapon;
	}

	public void RemoveForgingPlayer () {

		forgingPlayer = null;
	}

	public void ForgeWeapon () {

		if (forgingPlayer && metalAmount > 0) {

			// Initialization
			if (forgeTimer == 0) {

				forgingBar.SetActive (true);
				forgingBarFill.fillAmount = 0;
			}

			if (forgeTimer < maxForgeTime) {
				
				forgeTimer += Time.deltaTime;
				forgingBarFill.fillAmount = (forgeTimer / maxForgeTime);
			}
			else if (forgeTimer >= maxForgeTime) {

				ResetForgingBar ();
				UpdateMetalAmount (-1);

				GameObject newWeapon = null;

				switch (weaponForging) {
				case Weapon.Sword:
					newWeapon = Instantiate (forgedSwordPrefab, Vector3.zero, Quaternion.identity) as GameObject;
					break;
				case Weapon.Axe:
					newWeapon = Instantiate (forgedAxePrefab, Vector3.zero, Quaternion.identity) as GameObject;
					break;
				case Weapon.Shield:
					newWeapon = Instantiate (forgedShieldPrefab, Vector3.zero, Quaternion.identity) as GameObject;
					break;
				}

				forgingPlayer.GetComponent<PlayerController> ().ReceiveItem (newWeapon);
			}
		}
	}

	public void ResetForgingBar () {

		forgeTimer = 0;
		forgingBarFill.fillAmount = 0;
		forgingBar.SetActive (false);
	}
}
