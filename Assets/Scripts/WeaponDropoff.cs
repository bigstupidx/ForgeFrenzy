using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class WeaponDropoff : MonoBehaviour {

	[Header("Fill")]
	[SerializeField] Image fillImage;
	[SerializeField] float destroyTime;
	float dropoffTimer = 0;

	Weapon weaponRequired;


	void Update () {

		UpdateTimer();
	}

	void UpdateTimer () {

		dropoffTimer += Time.deltaTime;

		if(dropoffTimer >= destroyTime) {

			Destroy(this.gameObject);
		}
		else {

			fillImage.fillAmount = dropoffTimer / destroyTime;
		}
	}

	public void SetWeaponRequired (Weapon weaponType) {

		weaponRequired = weaponType;
	}

	public void SetDestroyTime(float newTime) {

		destroyTime = newTime;
	}

	public void DropOffWeapon (GameObject weaponDroppingOff) {
		
		Weapon weaponType = Weapon.None;

		if(weaponDroppingOff.name.Contains("Finished Axe")) {

			weaponType = Weapon.Axe;
		}
		else if(weaponDroppingOff.name.Contains("Finished Sword")) {

			weaponType = Weapon.Sword;
		}
		else if(weaponDroppingOff.name.Contains("Finished Shield")) {

			weaponType = Weapon.Shield;
		}

		if(weaponRequired == weaponType) {

			// Reward players
			// Play any feedback (audio, particle effects)

			// Increase weapon count
			LevelOneController levelOneController = GameObject.FindGameObjectWithTag("GameController").GetComponent<LevelOneController>();
			if(levelOneController) {

				switch (weaponType) {
				case Weapon.Shield:
					levelOneController.IncreaseShieldCount();
					break;
				case Weapon.Sword:
					levelOneController.IncreaseSwordCount();
					break;
				case Weapon.Axe:
					levelOneController.IncreaseAxeCount();
					break;
				}
			}

			Destroy(weaponDroppingOff);
			Destroy(this.gameObject);
		}
	}
}
