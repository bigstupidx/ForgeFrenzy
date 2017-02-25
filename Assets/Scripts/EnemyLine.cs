using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLine : MonoBehaviour {

	[Header("Enemy Line Parameters")]
	[SerializeField] float startingZ = 33f;
	[SerializeField] float endZ = -1f;
	[SerializeField] float moveSpeed = 1;

	[Header("Spawn Parameters")]
	[SerializeField] float spawnCooldown = 2;

	[Header("References")]
	[SerializeField] GameObject brokenMetal;
	[SerializeField] GameObject brokenLeather;
	[SerializeField] GameObject brokenWood;


	Vector3 endPosition;

	void Awake () {

		this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, startingZ);
		endPosition = new Vector3(this.transform.position.x, this.transform.position.y, endZ);
		StartCoroutine (SpawnItem ());
	}

	void Update () {

		this.transform.position = Vector3.Lerp(this.transform.position, endPosition, moveSpeed * Time.deltaTime);
	}

	IEnumerator SpawnItem () {

		Bounds bounds = this.GetComponent<Collider> ().bounds;
		Vector3 spawnLocation = new Vector3 (Random.Range (bounds.min.x, bounds.max.x), this.transform.position.y, this.transform.position.z - 6);
		Instantiate (brokenMetal, spawnLocation, Quaternion.identity);

		yield return new WaitForSeconds (spawnCooldown);

		StartCoroutine (SpawnItem ());
	}

	void OnCollisionEnter (Collision other) {

		if (other.transform.CompareTag ("Pickup")) {

			Destroy (other.gameObject);
		}
	}
}
