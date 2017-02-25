using Rewired;
using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public class PlayerInput : MonoBehaviour {

	[SerializeField] int playerID;
	[SerializeField] float moveSpeed = 1;

	[Header("References")]
	[SerializeField] Transform carryPosition;

	Player player;
	Rigidbody2D rb2d;
	GameObject pickedUpObject;
	GameObject stationFound;

	void Awake () {

		player = ReInput.players.GetPlayer(playerID);
		rb2d = this.GetComponentInChildren<Rigidbody2D>();
	}

	void Update () {

		ProcessMovementInput();
		ProcessActions();
	}

	void ProcessMovementInput () {

		float horizontalInput = player.GetAxis ("Horizontal");
		float verticalInput = player.GetAxis ("Vertical");

		if (horizontalInput != 0 || verticalInput != 0) {

			Vector2 directionVector = new Vector2 (horizontalInput, verticalInput);

			if (directionVector.magnitude > 0.5f) {

				float angleFacing = Mathf.Atan2 (player.GetAxis ("Vertical"), player.GetAxis ("Horizontal")) * Mathf.Rad2Deg;
				this.transform.rotation = Quaternion.Euler (0, 0, angleFacing);

				rb2d.MovePosition ((Vector2)this.transform.position + moveSpeed * directionVector * Time.deltaTime);
			}

			if (stationFound != null && Vector3.Distance (this.transform.position, stationFound.transform.position) > 1.5f) {

				if (stationFound.CompareTag ("Forge")) {

					stationFound.GetComponent<Forge> ().RemoveForgingPlayer ();
				}
				else if (stationFound.CompareTag ("Anvil")) {


				}
				else if (stationFound.CompareTag ("Woodworks")) {
					
					stationFound.GetComponent<Woodworks> ().ResetWoodCuttingBar ();
					stationFound.GetComponent<Woodworks> ().RemovePlayerCutting ();
				}

				stationFound = null;
			}
		}
	}

	void ProcessActions () {

		if (player.GetButtonDown ("Pickup")) {

			if (pickedUpObject != null) {

				FindStationToPlace ();
			}
			else if (pickedUpObject == null) {

				FindPickup ();
			}
		}
		else if (player.GetButtonDown ("Action")) {

			if (pickedUpObject == null && stationFound == null) {
				
				FindStationToActOn ();
			}
		}
		else if (player.GetButton ("Action")) {

			if (stationFound && pickedUpObject == null) {

				if (stationFound.CompareTag ("Forge")) {

					stationFound.GetComponent<Forge> ().ForgeWeapon ();
				}
				else if (stationFound.CompareTag ("Anvil")) {

					stationFound.GetComponent<Anvil> ().Hammer ();
				}
				else if (stationFound.CompareTag ("Woodworks")) {
					
					stationFound.GetComponent<Woodworks> ().ChopWood ();
				}
			}
		}
		else if (player.GetButtonUp ("Action")) {

			if (stationFound) {

				if (stationFound.CompareTag ("Forge")) {

					stationFound.GetComponent<Forge> ().ResetForgingBar ();
				}
				else if (stationFound.CompareTag ("Woodworks")) {

					stationFound.GetComponent<Woodworks> ().ResetWoodCuttingBar ();
					stationFound.GetComponent<Woodworks> ().RemovePlayerCutting ();
				}
			}
		}
	}

	void FindStationToPlace () {

		Collider2D[] collidersFound = GetCollidersInFront ();

		if(collidersFound.Length > 0) {

			foreach (Collider2D collider in collidersFound) {

				if (collider.CompareTag ("Forge")) {

					collider.GetComponent<Forge> ().UpdateMetalAmount (1);
					Destroy (pickedUpObject.gameObject);
					pickedUpObject = null;
				}
				else if(collider.CompareTag("Anvil") && collider.GetComponent<Anvil>().GetPlacedObject() == null) {

					collider.GetComponent<Anvil> ().PlaceObject (pickedUpObject);
					pickedUpObject = null;
				}
			}
		}
		else {

			DropItem();
		}
	}

	void DropItem () {

		pickedUpObject.transform.parent = null;
		pickedUpObject.GetComponent<Collider2D>().enabled = true;
		pickedUpObject.GetComponent<Rigidbody2D>().isKinematic = false;
		pickedUpObject = null;
	}

	void FindPickup () {

		Collider2D[] collidersFound = GetCollidersInFront ();

		foreach (Collider2D collider in collidersFound) {

			if(collider.transform.CompareTag("Pickup")) {

				pickedUpObject = collider.gameObject;
				pickedUpObject.transform.SetParent(carryPosition);
				pickedUpObject.transform.localPosition = Vector3.zero;
				pickedUpObject.transform.localRotation = Quaternion.identity;
				pickedUpObject.GetComponent<Collider2D>().enabled = false;
				pickedUpObject.GetComponent<Rigidbody2D>().isKinematic = true;
				if (pickedUpObject.GetComponent<BrokenWeapon> ()) { pickedUpObject.GetComponent<BrokenWeapon>().enabled = false; }
				break;
			}
		}
	}

	void FindStationToActOn () {

		Collider2D[] collidersFound = GetCollidersInFront ();

		if(collidersFound.Length > 0) {

			foreach (Collider2D collider in collidersFound) {

				if (collider.CompareTag ("Forge") || collider.CompareTag ("Anvil")) {

					stationFound = collider.gameObject;
				}
				else if (collider.CompareTag ("Woodworks")) {

					stationFound = collider.gameObject;
					stationFound.GetComponent<Woodworks> ().SetPlayerCutting (this.gameObject);
				}
				else if (collider.CompareTag ("Tannery")) {

					stationFound = collider.gameObject;
					stationFound.GetComponent<Tannery> ().PlaceLeather ();
				}
			}
		}
	}

	Collider2D[] GetCollidersInFront () {

		var layerMask = 1 << 8;
		layerMask = ~layerMask;
		Collider2D[] collidersFound = Physics2D.OverlapCircleAll((Vector2)(this.transform.position + this.transform.right), 0.5f, layerMask);
		//Debug.Log("stationsFound: " + collidersFound.Length);
		return collidersFound;
	}

	public void ReceiveItem (GameObject item) {

		pickedUpObject = item;
		pickedUpObject.transform.SetParent (carryPosition);
		pickedUpObject.transform.localPosition = Vector3.zero;
		pickedUpObject.transform.localRotation = Quaternion.identity;
		pickedUpObject.GetComponent<Collider2D> ().enabled = false;
	}
}
