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

		Destroy(dropOff.gameObject);
	}
}
