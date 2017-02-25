using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
public class WeaponPedestal : MonoBehaviour {

	[SerializeField] Weapon weaponForging;
		
	Forge forge;
	GameObject forgingPlayer;

	void Awake () {

		forge = this.GetComponentInParent<Forge> ();
	}

	void OnTriggerEnter(Collider other) {

		if (other.CompareTag ("Player")) {

			forgingPlayer = other.gameObject;
			forge.SetForgingPlayer (forgingPlayer, weaponForging);
			this.GetComponentInChildren<SpriteRenderer> ().color = Color.green;
		}
	}

	void OnTriggerExit(Collider other) {

		if (other.CompareTag ("Player") && other.gameObject == forgingPlayer) {

			forgingPlayer = null;
			forge.RemoveForgingPlayer ();
			this.GetComponentInChildren<SpriteRenderer> ().color = Color.white;
		}
	}
}
