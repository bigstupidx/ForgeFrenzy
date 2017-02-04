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

	void OnTriggerEnter2D(Collider2D other) {

		if (other.CompareTag ("Player")) {

			forgingPlayer = other.gameObject;
			forge.SetForgingPlayer (forgingPlayer, weaponForging);
			this.GetComponent<SpriteRenderer> ().color = Color.green;
		}
	}

	void OnTriggerExit2D(Collider2D other) {

		if (other.CompareTag ("Player") && other.gameObject == forgingPlayer) {

			forgingPlayer = null;
			forge.RemoveForgingPlayer ();
			this.GetComponent<SpriteRenderer> ().color = Color.white;
		}
	}
}
