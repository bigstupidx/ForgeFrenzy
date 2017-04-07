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
        transform.Translate(Vector3.right * dir * Time.deltaTime);
    }

    public void SetDir(int newDir) {
        dir = newDir;
    }
}
