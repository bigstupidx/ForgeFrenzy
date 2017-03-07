using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {

	[Header("Parameters")]
	[SerializeField] int inputID;
	[SerializeField] float moveSpeed = 0.1f;
	[SerializeField] float boostForce = 300;

	[Header("References")]
	[SerializeField] Transform carryPosition;
	[SerializeField] SpriteRenderer playerSpriteRenderer;
	[SerializeField] Sprite playerFrontSprite;
	[SerializeField] Sprite playerBackSprite;

	Player player;
	Rigidbody rb;
	GameObject pickedUpObject;
	GameObject stationFound;
	bool isBoosting = false;
	bool isStunned = false;

	void Awake () {

		player = ReInput.players.GetPlayer(inputID);
		rb = this.GetComponent<Rigidbody>();
	}

	void Update () {

		if (!isStunned) {
			
			ProcessActions ();

			if (pickedUpObject == null) {

				TogglePickupUI ();
			}
		}
	}

	void FixedUpdate () {

		if (!isStunned) {

			ProcessMovement ();
		}
	}

	void ProcessMovement () {

		float horizontalAxis = player.GetAxis("Horizontal");
		float verticalAxis = player.GetAxis("Vertical");

		if(horizontalAxis != 0 || verticalAxis != 0) {

			//float angleFacing = Mathf.Atan2 (player.GetAxis ("Horizontal"), player.GetAxis ("Vertical")) * Mathf.Rad2Deg;
			//this.transform.rotation = Quaternion.Euler (0, angleFacing, 0);

			// Flip sprite horizontally
			if(horizontalAxis > 0) { playerSpriteRenderer.flipX = true; }
			else if(horizontalAxis < 0) { playerSpriteRenderer.flipX = false; }

			// Change player sprite for forward and backward movement
			if(verticalAxis > 0) { 
				
				playerSpriteRenderer.sprite = playerBackSprite;
				carryPosition.transform.localPosition = new Vector3 (0, -0.1f, 0.2f);
			}
			else if(verticalAxis < 0) { 

				playerSpriteRenderer.sprite = playerFrontSprite;
				carryPosition.transform.localPosition = new Vector3 (0, -0.1f, -0.2f);
			}

			// Move Player
			rb.MovePosition(this.transform.position + moveSpeed * new Vector3(horizontalAxis, 0, verticalAxis) * Time.deltaTime);
		}

		if (player.GetButtonDown ("Boost") && !isBoosting) {

			isBoosting = true;
			rb.AddForce (boostForce * (carryPosition.position - this.transform.position).normalized);
			StopCoroutine (BoostCooldown ());
			StartCoroutine (BoostCooldown ());
		}

		// Cancel any station actions if player moves away from it
		if (stationFound != null && Vector3.Distance (this.transform.position, stationFound.transform.position) > 3f) {

			if (stationFound.CompareTag ("Anvil")) {

				stationFound.GetComponent<Anvil>().RemovePlayerHammering();
			}
			else if (stationFound.CompareTag ("Woodworks")) {

				stationFound.GetComponent<Woodworks> ().ResetWoodCuttingBar ();
				stationFound.GetComponent<Woodworks> ().RemovePlayerCutting ();
			}
			else if(stationFound.CompareTag("Workbench")) {
				
				stationFound.GetComponent<Workbench>().RemovePlayerCrafting();
			}

			stationFound = null;
		}
	}

	void ProcessActions () {

		if (player.GetButtonDown ("Pickup")) {
			
			if (pickedUpObject == null) {

				FindPickup ();
			}
			else if (pickedUpObject != null) {
				
				FindStationToPlace ();
			}
		}
		else if (player.GetButtonDown ("Action")) {

			if (pickedUpObject == null && stationFound == null) {

				FindStationToActOn ();
			}
		}
		else if (player.GetButton ("Action")) {

			if (stationFound && pickedUpObject == null) {

				if (stationFound.CompareTag ("Anvil")) {
					
					stationFound.GetComponent<Anvil> ().Hammer ();
				}
				else if (stationFound.CompareTag ("Woodworks")) {

					stationFound.GetComponent<Woodworks> ().ChopWood ();
				}
				else if(stationFound.CompareTag("Workbench")) {

					stationFound.GetComponent<Workbench>().CraftWeapon();
				}
			}
		}
		else if (player.GetButtonUp ("Action")) {

			if (stationFound) {

				if (stationFound.CompareTag ("Anvil")) {

					stationFound.GetComponent<Anvil> ().RemovePlayerHammering ();
				}
				else if (stationFound.CompareTag ("Woodworks")) {

					stationFound.GetComponent<Woodworks> ().ResetWoodCuttingBar ();
					stationFound.GetComponent<Woodworks> ().RemovePlayerCutting ();
				}
				else if(stationFound.CompareTag("Workbench")) {

					stationFound.GetComponent<Workbench>().RemovePlayerCrafting();
				}

				stationFound = null;
			}
		}
	}

	void FindStationToPlace () {

		Collider[] stationsFound = GetCollidersInFront ();
		bool stationFound = false;

		if(stationsFound.Length > 0) {

			foreach (Collider collider in stationsFound) {
				
				if (collider.CompareTag ("Forge")) {

					stationFound = true;

					if (pickedUpObject.name.Contains ("Broken")) {
						
						collider.GetComponent<Forge> ().UpdateMetalAmount (1);
						Destroy (pickedUpObject.gameObject);
					}
					else {

						Destroy (pickedUpObject.gameObject);
					}

					pickedUpObject = null;
				}
				else if(collider.CompareTag("Anvil") && collider.GetComponent<Anvil>().GetPlacedObject() == null) {

					stationFound = true;
					collider.GetComponent<Anvil> ().PlaceObject (pickedUpObject);
					pickedUpObject = null;
				}
				else if(collider.CompareTag("Tannery") && pickedUpObject.name.Contains("Broken Leather")) {

					stationFound = true;
					collider.GetComponent<Tannery>().PlaceLeather(pickedUpObject);
					pickedUpObject = null;
				}
				else if(collider.CompareTag("Workbench")) {

					stationFound = true;
					collider.GetComponent<Workbench>().AddObject(pickedUpObject);
					if (pickedUpObject.GetComponent<ChoppedWood> ()) { pickedUpObject.GetComponent<ChoppedWood> ().HidePickedUpUI (); }
					pickedUpObject = null;
				}
				else if(collider.CompareTag("Delivery")) {

					stationFound = true;
					collider.GetComponent<DeliveryStation>().DropOffWeapon(pickedUpObject);
					pickedUpObject = null;
				}
			}
		}

		if(!stationFound || stationsFound.Length == 0) {

			DropItem();
		}
	}

	void DropItem () {
		
		pickedUpObject.transform.parent = null;
		pickedUpObject.GetComponent<Collider>().enabled = true;
		pickedUpObject.GetComponent<Rigidbody>().isKinematic = false;
		pickedUpObject.transform.rotation = Quaternion.Euler(0, 180, 0);
		pickedUpObject.transform.localScale = Vector3.one;

		if (pickedUpObject.GetComponent<ChoppedWood> ()) { pickedUpObject.GetComponent<ChoppedWood> ().HidePickedUpUI (); }
		pickedUpObject = null;
	}

	void FindPickup () {

		Collider[] collidersFound = GetCollidersInFront ();
		//Debug.Log ("Pickups Found: " + collidersFound.Length);
		foreach (Collider collider in collidersFound) {
			//Debug.Log ("collider found: " + collider.name);	
			if (collider.transform.CompareTag ("Pickup")) {
				
				pickedUpObject = collider.gameObject;
				pickedUpObject.transform.SetParent (carryPosition);
				pickedUpObject.transform.localPosition = Vector3.zero;
				pickedUpObject.transform.localRotation = Quaternion.identity;
				pickedUpObject.transform.localScale = Vector3.one;
				pickedUpObject.GetComponent<Collider> ().enabled = false;
				pickedUpObject.GetComponent<Rigidbody> ().isKinematic = true;
				if (pickedUpObject.GetComponent<BrokenWeapon> ()) { pickedUpObject.GetComponent<BrokenWeapon> ().enabled = false; }
				if (pickedUpObject.GetComponent<ChoppedWood> ()) { pickedUpObject.GetComponent<ChoppedWood> ().ShowPickedUpUI (); }
				break;
			}
			else if (collider.transform.CompareTag("Forge")) {
				
				collider.GetComponent<Forge> ().RetrieveIngot (this);
			}
			else if (collider.transform.CompareTag ("Anvil")) {

				collider.GetComponent<Anvil> ().RemoveObject (this);
			}
			else if (collider.transform.CompareTag ("Workbench")) {

				collider.GetComponent<Workbench> ().RemoveObject (this);
			}
		}
	}

	void FindStationToActOn () {
		
		Collider[] stationsFound = GetCollidersInFront ();

		if(stationsFound.Length > 0) {

			foreach (Collider collider in stationsFound) {
				
				if (collider.CompareTag ("Forge")) {

					stationFound = collider.gameObject;
				}
				else if(collider.CompareTag("Anvil")) {
					
					stationFound = collider.gameObject;
					stationFound.GetComponent<Anvil>().SetPlayerHammering(this.gameObject);
				}
				else if (collider.CompareTag ("Woodworks")) {

					stationFound = collider.gameObject;
					stationFound.GetComponent<Woodworks> ().SetPlayerCutting (this.gameObject);
				}
				else if (collider.CompareTag ("Tannery")) {

					stationFound = collider.gameObject;
				}
				else if(collider.CompareTag("Workbench")) {

					stationFound = collider.gameObject;
					stationFound.GetComponent<Workbench>().SetPlayerCrafting(this.gameObject);
				}
			}
		}
	}

	Collider[] GetCollidersInFront () {

		//Collider[] collidersFound = Physics.OverlapSphere(this.transform.position + 0.5f * this.transform.forward - 0.5f * this.transform.up, 0.5f);
		//Collider[] collidersFound = Physics.OverlapBox(this.transform.position - 0.5f * this.transform.forward, 0.5f * Vector3.one);
		Collider[] collidersFound = Physics.OverlapBox(carryPosition.position, 0.5f * Vector3.one);
		//Debug.Log("stationsFound: " + collidersFound.Length);
		return collidersFound;
	}

	public void ReceiveItem (GameObject item) {

		pickedUpObject = item;
		pickedUpObject.transform.SetParent (carryPosition);
		pickedUpObject.transform.localPosition = Vector3.zero;
		pickedUpObject.transform.localRotation = Quaternion.identity;
		pickedUpObject.GetComponent<Collider> ().enabled = false;
	}

	IEnumerator BoostCooldown () {

		yield return new WaitForSeconds (0.5f);
		isBoosting = false;
	}

	public IEnumerator PlayerHit (float stunTime) {

		if(pickedUpObject) { Destroy(pickedUpObject.gameObject); }

		isStunned = true;
		this.transform.rotation = Quaternion.Euler (0, 0, 90);

		yield return new WaitForSeconds (stunTime);

		isStunned = false;
		this.transform.rotation = Quaternion.identity;
	}

	void TogglePickupUI () {

		// TODO: Turn on some kind of thought bubble over the player's head if there's a pickup in front of the player
	}
}