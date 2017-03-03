using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class HUDGears : MonoBehaviour {

	[Header("Parameters")]
	[SerializeField] float angleForShield = 300;
	[SerializeField] float angleForSword = 60;
	[SerializeField] float angleForAxe = 180;
	[SerializeField] float smallGearTurnSpeed = 3;
	[SerializeField] float mediumGearTurnSpeed = 2;
	[SerializeField] float bigGearTurnSpeed = 1;

	[Header("References")]
	[SerializeField] RectTransform bigGearTransform;
	[SerializeField] RectTransform mediumGearTransform;
	[SerializeField] RectTransform smallGearTransform;
	[SerializeField] Image shieldImage;
	[SerializeField] Image swordImage;
	[SerializeField] Image axeImage;

	// Private variables
	float targetAngle = 0;

	void Update () {

		float currentAngle = bigGearTransform.rotation.eulerAngles.z;

		if (Mathf.Abs(currentAngle - targetAngle) > 1) {

			bigGearTransform.Rotate (0, 0, bigGearTurnSpeed);
			mediumGearTransform.Rotate (0, 0, mediumGearTurnSpeed);
			smallGearTransform.Rotate (0, 0, smallGearTurnSpeed);
		}

		if (Input.GetKeyDown (KeyCode.Alpha1)) {

			TurnToWeapon (Weapon.Shield);
		}
		else if (Input.GetKeyDown (KeyCode.Alpha2)) {

			TurnToWeapon (Weapon.Axe);
		}
		else if (Input.GetKeyDown (KeyCode.Alpha3)) {

			TurnToWeapon (Weapon.Sword);
		}
	}

	public void TurnToWeapon(Weapon weapon) {

		if (weapon == Weapon.Shield) {

			targetAngle = angleForShield;
			StopBlinking ();
			StartCoroutine (BlinkWeapon (shieldImage));
		}
		else if (weapon == Weapon.Sword) {

			targetAngle = angleForSword;
			StopBlinking ();
			StartCoroutine (BlinkWeapon (swordImage));
		}
		else if (weapon == Weapon.Axe) {

			targetAngle = angleForAxe;
			StopBlinking ();
			StartCoroutine (BlinkWeapon (axeImage));
		}
	}

	void StopBlinking () {

		swordImage.color = Color.white;
		shieldImage.color = Color.white;
		axeImage.color = Color.white;
		this.StopAllCoroutines ();
	}

	IEnumerator BlinkWeapon (Image targetImage) {

		while (true) {

			targetImage.color = (targetImage.color == Color.red) ? Color.white : Color.red;

			yield return new WaitForSeconds(0.25f);
		}
	}
}
