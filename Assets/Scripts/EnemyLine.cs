using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLine : MonoBehaviour {

	[SerializeField] float startingZ = 33f;
	[SerializeField] float endZ = -1f;
	[SerializeField] float moveSpeed = 1;

	Vector3 endPosition;

	void Awake () {

		this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, startingZ);
		endPosition = new Vector3(this.transform.position.x, this.transform.position.y, endZ);
	}

	void Update () {

		this.transform.position = Vector3.Lerp(this.transform.position, endPosition, moveSpeed * Time.deltaTime);
	}
}
