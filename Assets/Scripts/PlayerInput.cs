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

	void Awake () {

		player = ReInput.players.GetPlayer(playerID);
		rb2d = this.GetComponentInChildren<Rigidbody2D>();
	}

	void Update () {

		ProcessMovementInput();
		ProcessActions();
	}

	void ProcessMovementInput () {

		Vector2 directionVector = Vector2.zero;

		if(player.GetAxis("Horizontal") != 0) {

			directionVector += new Vector2(player.GetAxis("Horizontal"), 0);
		}

		if(player.GetAxis("Vertical") != 0) {

			directionVector += new Vector2(0, player.GetAxis("Vertical"));
		}

		if(directionVector != Vector2.zero) {
			
			float angleFacing = Mathf.Atan2(player.GetAxis("Vertical"), player.GetAxis("Horizontal")) * Mathf.Rad2Deg;
			this.transform.rotation = Quaternion.Euler(0, 0, angleFacing);

			rb2d.MovePosition((Vector2)this.transform.position + moveSpeed * directionVector * Time.deltaTime);
		}
	}

	void ProcessActions () {

		if(player.GetButtonDown("Pickup")) {

			if(pickedUpObject != null) {

				FindStationToPlace();
			}
			else if(pickedUpObject == null) {

				FindPickup();
			}
		}
		else if(player.GetButtonDown("Action")) {

			if(pickedUpObject == null) {
				
				FindStationToActOn();
			}
		}
	}

	void FindStationToPlace () {

		var layerMask = 1 << 8;
		layerMask = ~layerMask;
		Collider2D[] collidersFound = Physics2D.OverlapCircleAll((Vector2)(this.transform.position + this.transform.right), 1, layerMask);
		//Debug.Log("stationsFound: " + collidersFound.Length);

		if(collidersFound.Length > 0) {

			foreach (Collider2D collider in collidersFound) {

				if(collider.CompareTag("Anvil") && collider.transform.childCount == 0) {

					pickedUpObject.transform.SetParent(collider.transform);
					pickedUpObject.transform.localPosition = Vector3.zero;
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

		var layerMask = 1 << 8;
		layerMask = ~layerMask;
		Collider2D[] collidersFound = Physics2D.OverlapCircleAll((Vector2)(this.transform.position + this.transform.right), 1, layerMask);
		//Debug.Log("collidersFound: " + pickupsFound.Length);

		foreach (Collider2D collider in collidersFound) {

			if(collider.transform.CompareTag("Pickup")) {

				pickedUpObject = collider.gameObject;
				pickedUpObject.transform.SetParent(carryPosition);
				pickedUpObject.transform.localPosition = Vector3.zero;
				pickedUpObject.GetComponent<Collider2D>().enabled = false;
				pickedUpObject.GetComponent<Rigidbody2D>().isKinematic = true;
				break;
			}
		}
	}

	void FindStationToActOn () {


	}
}
