using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

[DisallowMultipleComponent]
public class NewPlayer : MonoBehaviour {

	[SerializeField] int inputID;
	[SerializeField] float moveSpeed = 0.1f;

	Player player;
	Rigidbody rb;

	void Awake () {

		player = ReInput.players.GetPlayer(inputID);
		rb = this.GetComponent<Rigidbody>();
	}

	void Update () {

		ProcessMovement();
	}

	void ProcessMovement () {

		float horizontalAxis = player.GetAxis("Horizontal");
		float verticalAxis = player.GetAxis("Vertical");

		if(horizontalAxis != 0 || verticalAxis != 0) {

			float angleFacing = Mathf.Atan2 (player.GetAxis ("Horizontal"), player.GetAxis ("Vertical")) * Mathf.Rad2Deg;
			this.transform.rotation = Quaternion.Euler (0, angleFacing, 0);

			rb.MovePosition(this.transform.position + moveSpeed * new Vector3(horizontalAxis, 0, verticalAxis) * Time.deltaTime);
		}
	}
}
