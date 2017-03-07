using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class DeliveryStation : MonoBehaviour {

	[SerializeField] EnemyLine enemyLine;
	MeshRenderer meshRenderer;

	void Awake () {

		meshRenderer = this.GetComponent<MeshRenderer>();
	}

	public void DropOffWeapon (GameObject dropOff) {

		if(dropOff.name.Contains("Finished Axe")) {

			enemyLine.IncrementAxeCount();
			meshRenderer.material.color = Color.green;
		}
		else if(dropOff.name.Contains("Finished Sword")) {

			enemyLine.IncrementSwordCount();
			meshRenderer.material.color = Color.green;
		}
		else if(dropOff.name.Contains("Finished Shield")) {

			enemyLine.IncrementShieldCount();
			meshRenderer.material.color = Color.green;
		}

		if(meshRenderer.material.color != Color.green) {

			meshRenderer.material.color = Color.red;
		}

		StartCoroutine (DisplayWeapon (dropOff));
	}

	IEnumerator DisplayWeapon (GameObject weapon) {

		weapon.transform.parent = this.transform;
		weapon.transform.localPosition = new Vector3 (0, 0.6f, 0);
		weapon.transform.rotation = Quaternion.Euler (90, 0, 0);
		weapon.transform.localScale = Vector3.one;

		yield return new WaitForSeconds (1);

		this.GetComponent<MeshRenderer>().material.color = Color.white;
		Destroy (weapon);
	}
}
