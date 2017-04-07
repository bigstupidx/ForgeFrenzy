using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowDriftSpawner : MonoBehaviour {

    [SerializeField] GameObject snowDriftPrefab;

    [SerializeField] float leftX;
    [SerializeField] float rightX;
    [SerializeField] float maxZ;
    [SerializeField] float minZ;
    float randX;

	// Use this for initialization
	void Start () {
        StartCoroutine(SpawnSnowDrift());
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator SpawnSnowDrift() {

        GameObject[] drifts = GameObject.FindGameObjectsWithTag("Snow Drift");

        if (drifts.Length < 2)
        {
            float randZ = Random.Range(minZ, maxZ);
            int randSide = Random.Range(0, 2);

            if (randSide == 0) { randX = rightX; }
            else { randX = leftX; }

            Vector3 newPosition = new Vector3(randX, 0, randZ);
            GameObject newDrift = Instantiate(snowDriftPrefab, newPosition, Quaternion.identity) as GameObject;

            if (randSide == 0) { newDrift.GetComponent<SnowDrift>().SetDir(-1); }
            else { newDrift.GetComponent<SnowDrift>().SetDir(1); }
        }

        float randWait = Random.Range(5, 10);
        yield return new WaitForSeconds(randWait);

        StartCoroutine(SpawnSnowDrift());
    }
}
