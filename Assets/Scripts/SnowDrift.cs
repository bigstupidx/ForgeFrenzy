using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowDrift : MonoBehaviour {

    int dir;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.Translate(Vector3.right * dir * Time.deltaTime * 0.5f);
        transform.Translate(-Vector3.up * Time.deltaTime * 0.03f);

        if (this.transform.position.y < -1.4f) {
            Destroy(this.gameObject);
        }
    }

    public void SetDir(int newDir) {
        dir = newDir;
    }

	void OnTriggerEnter2D(Collider2D other) {

		if(other.CompareTag("Player")) {

			other.GetComponent<PlayerController>().SlowMoveSpeed();
		}
	}

	void OnTriggerExit2D(Collider2D other) {

		if(other.CompareTag("Player")) {

			other.GetComponent<PlayerController>().ResetMoveSpeed();
		}
	}
}
