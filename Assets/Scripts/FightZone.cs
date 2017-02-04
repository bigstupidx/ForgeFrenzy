using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
public class FightZone : MonoBehaviour {

	[Header("Parameters")]
	[SerializeField, Range(0, 1f)] float swordSpawnChance;
	[SerializeField, Range(0, 1f)] float axeSpawnChance;
	[SerializeField, Range(0, 1f)] float spearSpawnChance;

	[Header("References")]
	[SerializeField] GameObject brokenWeaponPrefab;
	[SerializeField] Sprite swordSprite;
	[SerializeField] Sprite axeSprite;
	[SerializeField] Sprite spearSprite;

	float spawnRate = 3;


	void Start () {

		if ((swordSpawnChance + axeSpawnChance + spearSpawnChance) < 0.99f || (swordSpawnChance + axeSpawnChance + spearSpawnChance) > 1.01f) {

			Debug.Log ("Check your spawn probabilities foo! " + (swordSpawnChance + axeSpawnChance + spearSpawnChance));
		}

		StartCoroutine (SpawnWeapon ());
	}

	IEnumerator SpawnWeapon () {

		Bounds bounds = this.GetComponent<Collider2D> ().bounds;
		Vector3 spawnLocation = new Vector3 (this.transform.position.x, Random.Range (bounds.min.y, bounds.max.y), 0);
		GameObject newBrokenWeapon = Instantiate (brokenWeaponPrefab, spawnLocation, Quaternion.identity) as GameObject;

		float randomValue = Random.value;

		if (randomValue > axeSpawnChance + spearSpawnChance) {

			newBrokenWeapon.GetComponent<SpriteRenderer> ().sprite = swordSprite;
		}
		else if (randomValue > spearSpawnChance) {

			newBrokenWeapon.GetComponent<SpriteRenderer> ().sprite = axeSprite;
		}
		else if (randomValue < spearSpawnChance) {

			newBrokenWeapon.GetComponent<SpriteRenderer> ().sprite = spearSprite;
		}

		yield return new WaitForSeconds (spawnRate);

		StartCoroutine (SpawnWeapon ());
	}
}
