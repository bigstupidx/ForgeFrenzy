using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class DeliveryStation : MonoBehaviour {

	[SerializeField] EnemyLine enemyLine;

	public void DropOffWeapon (GameObject dropOff) {

		if(dropOff.name.Contains("Finished Axe")) {

			enemyLine.IncrementAxeCount();
		}
		else if(dropOff.name.Contains("Finished Sword")) {

			enemyLine.IncrementSwordCount();
		}
		else if(dropOff.name.Contains("Finished Shield")) {

			enemyLine.IncrementShieldCount();
		}

		StartCoroutine (DisplayWeapon (dropOff));
	}

	IEnumerator DisplayWeapon (GameObject weapon) {

		weapon.transform.parent = this.transform;
		weapon.transform.localPosition = new Vector3 (0, 0.6f, 0);
		weapon.transform.rotation = Quaternion.Euler (90, 0, 0);
		weapon.transform.localScale = Vector3.one;

		yield return new WaitForSeconds (1);
		Destroy (weapon);
	}
}
