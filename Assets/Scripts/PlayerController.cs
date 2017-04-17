using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rewired;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {

	[Header("Parameters")]
	[SerializeField] int inputID;
	[SerializeField] float moveSpeed = 0.1f;
	[SerializeField] float boostForce = 300;

	[Header("Station UI")]
	[SerializeField] Image stationGuideImage;
	[SerializeField] Sprite forgeIcon;
	[SerializeField] Sprite anvilIcon;
	[SerializeField] Sprite workbenchIcon;
	[SerializeField] Sprite tanRackIcon;

	[Header("References")]
	[SerializeField] Transform carryPosition;
	[SerializeField] SpriteRenderer playerSpriteRenderer;
	//[SerializeField] Sprite playerFrontSprite;
	//[SerializeField] Sprite playerBackSprite;
	[SerializeField] Animator playerAnim;

	Player player;
	Rigidbody rb;
	GameObject pickedUpObject;
	GameObject stationFound;
	bool isBoosting = false;
	bool isStunned = false;


	void Awake () {

		player = ReInput.players.GetPlayer(inputID);
		rb = this.GetComponent<Rigidbody>();

		if(stationGuideImage == null) {

			stationGuideImage = this.GetComponentInChildren<Image>();
		}
		stationGuideImage.enabled = false;

		playerAnim.SetBool ("Move", false);
		playerAnim.SetBool ("Facing", true);

	}

	void Update () {

		if (!isStunned) {
			
			ProcessActions ();
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

		if (horizontalAxis != 0 || verticalAxis != 0) {

			//float angleFacing = Mathf.Atan2 (player.GetAxis ("Horizontal"), player.GetAxis ("Vertical")) * Mathf.Rad2Deg;
			//this.transform.rotation = Quaternion.Euler (0, angleFacing, 0);

			playerAnim.SetBool ("Move", true);

			// Flip sprite horizontally
			if (horizontalAxis > 0) {
				playerSpriteRenderer.flipX = true;
			} else if (horizontalAxis < 0) {
				playerSpriteRenderer.flipX = false;
			}

			// Change player sprite for forward and backward movement
			if (verticalAxis > 0) { 
				
				//playerSpriteRenderer.sprite = playerBackSprite;
				playerAnim.SetBool ("Facing", false);
				playerAnim.SetBool ("Move", true);
				carryPosition.transform.localPosition = new Vector3 (0, -0.1f, 0.2f);
				if (pickedUpObject) {
					pickedUpObject.GetComponent<SpriteRenderer> ().sortingOrder = -1;
				}
			} else if (verticalAxis < 0) { 

				//playerSpriteRenderer.sprite = playerFrontSprite;
				playerAnim.SetBool ("Facing", true);
				playerAnim.SetBool ("Move", true);
				carryPosition.transform.localPosition = new Vector3 (0, -0.1f, -0.2f);
				if (pickedUpObject) {
					pickedUpObject.GetComponent<SpriteRenderer> ().sortingOrder = 1;
				}
			}

			// Move Player
			rb.MovePosition (this.transform.position + moveSpeed * new Vector3 (horizontalAxis, 0, verticalAxis) * Time.deltaTime);
		} else {

			playerAnim.SetBool("Move", false);

		}

		if (player.GetButtonDown ("Boost") && !isBoosting) {

			isBoosting = true;
			Vector3 boostDirection = new Vector3(horizontalAxis, 0, verticalAxis);
			rb.AddForce (boostForce * boostDirection.normalized);
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

		Collider[] collidersFound = GetCollidersInFront ();

		// Filter colliders to only stations
		List<GameObject> stationsFoundList = new List<GameObject>();

		for(int i = 0; i < collidersFound.Length; i++) {

			if(collidersFound[i].transform.CompareTag("Pickup") || collidersFound[i].transform.CompareTag("Forge") ||
				collidersFound[i].transform.CompareTag("Anvil") || collidersFound[i].transform.CompareTag("Workbench") ||
				collidersFound[i].transform.CompareTag("Tannery") || collidersFound[i].transform.CompareTag("Dropoff")) {

				stationsFoundList.Add(collidersFound[i].gameObject);
			}
		}

		if(stationsFoundList.Count == 0) {

			DropItem();
		}
		else {
			
			// Find station closest to player
			float closestDistance = 1000;
			GameObject closestStation = null;
			GameObject[] stationsFound = stationsFoundList.ToArray();

			for(int i = 0; i < stationsFound.Length; i++) {

				float stationDistance = Vector3.Distance(this.transform.position - 0.5f * Vector3.forward, stationsFound[i].transform.position);

				if(stationDistance < closestDistance) {

					closestDistance = stationDistance;
					closestStation = stationsFound[i];
				}
			}

			// Act on station depending on what it is
			if (closestStation.CompareTag ("Forge")) {

				if (pickedUpObject.name.Contains ("Broken")) {
					
					closestStation.GetComponent<Forge> ().UpdateMeltingAmount ();
					Destroy (pickedUpObject.gameObject);
				}
				else {

					Destroy (pickedUpObject.gameObject);
				}

				pickedUpObject = null;
			}
			else if(closestStation.CompareTag("Anvil") && closestStation.GetComponent<Anvil>().GetPlacedObject() == null) {

				closestStation.GetComponent<Anvil> ().PlaceObject (pickedUpObject);
				pickedUpObject = null;
			}
			else if(closestStation.CompareTag("Tannery") && pickedUpObject.name.Contains("Broken Leather")) {

				closestStation.GetComponent<Tannery>().PlaceLeather(pickedUpObject);
				pickedUpObject = null;
			}
			else if(closestStation.CompareTag("Workbench")) {
				//Debug.Log("Dropping off item at workbench");
				closestStation.GetComponent<Workbench>().AddObject(pickedUpObject);
				if (pickedUpObject.GetComponent<ChoppedWood> ()) { pickedUpObject.GetComponent<ChoppedWood> ().HidePickedUpUI (); }
				pickedUpObject = null;
				//Debug.Log("Dropped off item");
			}
			else if(closestStation.CompareTag("Dropoff")) {
				
				closestStation.GetComponent<WeaponDropoff>().DropOffWeapon(pickedUpObject);
				pickedUpObject = null;
			}
		}

		if(pickedUpObject == null) {

			stationGuideImage.enabled = false;
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

		// Add all relevant colliders to a list
		List<Collider> collidersFoundList = new List<Collider>();
		foreach (Collider collider in collidersFound) {

			if(collider.transform.CompareTag("Pickup") || collider.transform.CompareTag("Forge") ||
				collider.transform.CompareTag("Anvil") || collider.transform.CompareTag("Workbench")) {

				collidersFoundList.Add(collider);
			}
		}

		// Find collider closest to the player
		if(collidersFound.Length > 0) {
			
			collidersFound = collidersFoundList.ToArray();
			GameObject closestObject = null;
			float closestDistance = 1000f;
			for(int i = 0; i < collidersFound.Length; i++) {

				float objectDistance = Vector3.Distance(collidersFound[i].transform.position, this.transform.position - 0.5f * Vector3.forward);

				if(objectDistance < closestDistance) {

					closestObject = collidersFound[i].gameObject;
					closestDistance = objectDistance;
				}
			}

			// Act on closest object depending on where it was found
			if(closestObject != null) {
				
				if (closestObject.transform.CompareTag ("Pickup")) {
					
					pickedUpObject = closestObject;
					pickedUpObject.transform.SetParent (carryPosition);
					pickedUpObject.transform.localPosition = Vector3.zero;
					pickedUpObject.transform.localRotation = Quaternion.identity;
					pickedUpObject.transform.localScale = Vector3.one;
					pickedUpObject.GetComponent<Collider> ().enabled = false;
					pickedUpObject.GetComponent<Rigidbody> ().isKinematic = true;

					if (pickedUpObject.GetComponent<BrokenWeapon> ()) { pickedUpObject.GetComponent<BrokenWeapon> ().enabled = false; }
					else if (pickedUpObject.GetComponent<ChoppedWood> ()) { pickedUpObject.GetComponent<ChoppedWood> ().ShowPickedUpUI (); }
				}
				else if (closestObject.transform.CompareTag("Forge")) {
					
					closestObject.GetComponent<Forge> ().RetrieveIngot (this);
				}
				else if (closestObject.transform.CompareTag ("Anvil")) {

					closestObject.GetComponent<Anvil> ().RemoveObject (this);
				}
				else if (closestObject.transform.CompareTag ("Workbench")) {

					closestObject.GetComponent<Workbench> ().RemoveObject (this);
				}

				UpdateStationGuideIcon();
			}
		}
	}

	void FindStationToActOn () {
		
		Collider[] collidersFound = GetCollidersInFront ();
		//Debug.Log("colliders found: " + collidersFound.Length);
		if(collidersFound.Length > 0) {
			
			// Filter array to only stations
			List<GameObject> stationsFoundList = new List<GameObject>();

			for(int i = 0; i < collidersFound.Length; i++) {

				string foundTag = collidersFound[i].tag;

				if(foundTag == "Forge" || foundTag == "Anvil" || foundTag == "Woodworks" ||
					foundTag == "Tannery" || foundTag == "Workbench") {
					//Debug.Log("Added station to list");
					stationsFoundList.Add(collidersFound[i].gameObject);
				}
			}

			GameObject[] stationsFound = stationsFoundList.ToArray();
			//Debug.Log("stations found: " + stationsFound.Length);
			// Find station closest to player
			float closestDistance = 1000f;
			GameObject closestStation = null;

			for(int i = 0; i < stationsFound.Length; i++) {

				float stationDistance = Vector3.Distance(this.transform.position - 0.5f * Vector3.forward, stationsFound[i].transform.position);

				if(stationDistance < closestDistance) {

					closestDistance = stationDistance;
					closestStation = stationsFound[i].gameObject;
				}
			}
			//Debug.Log("closest station: " + closestStation);

			// Act on closest station depending on what it is
			if(closestStation != null) {
				
				if (closestStation.CompareTag ("Forge")) {

					stationFound = closestStation;
				}
				else if(closestStation.CompareTag("Anvil")) {

					stationFound = closestStation;
					stationFound.GetComponent<Anvil>().SetPlayerHammering(this.gameObject);
				}
				else if (closestStation.CompareTag ("Woodworks")) {

					stationFound = closestStation;
					stationFound.GetComponent<Woodworks> ().SetPlayerCutting (this.gameObject);
				}
				else if (closestStation.CompareTag ("Tannery")) {

					stationFound = closestStation;
				}
				else if(closestStation.CompareTag("Workbench")) {

					stationFound = closestStation;
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
		UpdateStationGuideIcon();
	}

	void UpdateStationGuideIcon () {

		if(pickedUpObject) {

			stationGuideImage.enabled = true;

			if(pickedUpObject.GetComponent<BrokenWeapon>()) { stationGuideImage.sprite = forgeIcon; }
			else if(pickedUpObject.name.Contains("Forged Ingot")) { stationGuideImage.sprite = anvilIcon; }
			else if(pickedUpObject.name.Contains("Axe Metal")) { stationGuideImage.sprite = workbenchIcon; }
			else if(pickedUpObject.name.Contains("Sword Metal")) { stationGuideImage.sprite = workbenchIcon; }
			else if(pickedUpObject.name.Contains("Broken Leather")) { stationGuideImage.sprite = tanRackIcon; }
			else if(pickedUpObject.name.Contains("Tanned Leather")) { stationGuideImage.sprite = workbenchIcon; }
			else if(pickedUpObject.name.Contains("Wood")) { stationGuideImage.sprite = workbenchIcon; }
			else if(pickedUpObject.name.Contains("Finished")) { stationGuideImage.enabled = false; }
		}
		else {

			stationGuideImage.enabled = false;
		}
	}

	IEnumerator BoostCooldown () {

		yield return new WaitForSeconds (0.4f);
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

	public bool HasObject () {

		return (pickedUpObject ? true : false);
	}

	public GameObject GetPickedUpObject () {

		return pickedUpObject;
	}

	public void SlowMoveSpeed () {

		moveSpeed = 0.05f;
	}

	public void ResetMoveSpeed () {

		moveSpeed = 0.1f;
	}

	public float GetMoveSpeed () {

		return moveSpeed;
	}

	public void SetMoveSpeed(float newMoveSpeed) {

		moveSpeed = newMoveSpeed;
	}
}