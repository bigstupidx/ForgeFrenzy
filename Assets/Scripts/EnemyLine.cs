﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class EnemyLine : MonoBehaviour {

	[Header("Enemy Line Parameters")]
	[SerializeField] float startingZ = 33f;
	[SerializeField] float losingZ = -1f;
	[SerializeField] float winningZ = 35f;
	[SerializeField] float moveSpeed = 1;

	[Header("Enemy Army Composition")]
	[SerializeField] float enemyAxePercentage = 0.33f;
	[SerializeField] float enemySwordPercentage = 0.33f;
	[SerializeField] float enemyShieldPercentage = 0.33f;

	[Header("Ally Army Composition")]
	[SerializeField] float allyAxePercentage = 0.33f;
	[SerializeField] float allySwordPercentage = 0.33f;
	[SerializeField] float allyShieldPercentage = 0.33f;

	[Header("Spawn Parameters")]
	[SerializeField] float spawnCooldown = 2f;

	[Header("Trebuchet Parameters")]
	[SerializeField] float projectileCooldown = 2f;

	[Header("References")]
	[SerializeField] GameObject brokenMetal;
	[SerializeField] GameObject brokenLeather;
	[SerializeField] GameObject trebuchetProjectileController;
	[SerializeField] HUDGears hudGears;

	Vector3 winPosition;
	Vector3 losePosition;


	void Awake () {

		this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, startingZ);
		winPosition = new Vector3(this.transform.position.x, this.transform.position.y, winningZ);
		losePosition = new Vector3(this.transform.position.x, this.transform.position.y, losingZ);
		StartCoroutine (SpawnItem ());
		StartCoroutine(DecreaseAllyStrength());
		StartCoroutine (LaunchProjectile ());
	}

	void Update () {
		
		this.transform.position = Vector3.MoveTowards(this.transform.position, losePosition, moveSpeed * Time.deltaTime);

		if(Vector3.Distance(this.transform.position, winPosition) < 0.1f) {

			GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().PlayersWin();
			StopAllCoroutines();
		}
		else if(Vector3.Distance(this.transform.position, losePosition) < 0.1f) {

			GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().PlayersLose();
			StopAllCoroutines();
		}
	}

	IEnumerator SpawnItem () {

		Bounds bounds = this.GetComponent<Collider> ().bounds;
		Vector3 spawnLocation = new Vector3 (Random.Range (bounds.min.x, bounds.max.x), this.transform.position.y, this.transform.position.z - 6);

		if (Random.value < 0.75f) {
			Instantiate (brokenMetal, spawnLocation, Quaternion.identity);
		}
		else {
			Instantiate (brokenLeather, spawnLocation, Quaternion.identity);
		}

		yield return new WaitForSeconds (spawnCooldown);

		StartCoroutine (SpawnItem ());
	}

	void OnCollisionEnter (Collision other) {

		if (other.transform.CompareTag ("Pickup")) {

			Destroy (other.gameObject);
		}
	}

	void UpdateMoveSpeed () {

		float advantageAmount = 0;

		if(enemyShieldPercentage != 0) { advantageAmount += allyAxePercentage - enemyShieldPercentage; }
		if(enemyAxePercentage != 0) { advantageAmount += allySwordPercentage - enemyAxePercentage; }
		if(enemySwordPercentage != 0) { advantageAmount += allyShieldPercentage - enemySwordPercentage; }

		float disadvantageAmount = 0;

		if(enemyAxePercentage - allyShieldPercentage > 0) { disadvantageAmount += enemyAxePercentage - allyShieldPercentage; }
		if(enemySwordPercentage - allyAxePercentage > 0) { disadvantageAmount += enemySwordPercentage - allyAxePercentage; }
		if(enemyShieldPercentage - allySwordPercentage > 0) { disadvantageAmount += enemyShieldPercentage - allySwordPercentage; }

		float balanceFactor = 0.1f;
		moveSpeed += balanceFactor * (disadvantageAmount - advantageAmount);
	}

	public void IncrementAxeCount () {

		allyAxePercentage += 0.05f;
		UpdateMoveSpeed();
	}

	public void IncrementSwordCount () {

		allySwordPercentage += 0.05f;
		UpdateMoveSpeed();
	}

	public void IncrementShieldCount () {

		allyShieldPercentage += 0.05f;
		UpdateMoveSpeed();
	}

	IEnumerator DecreaseAllyStrength () {

		float decreaseAmount = Random.Range(0.01f, 0.03f);

		float weaponToDecrease = Random.value;

		if(weaponToDecrease < 0.33f && allyAxePercentage >= decreaseAmount) {

			allyAxePercentage -= decreaseAmount;
		}
		else if(weaponToDecrease < 0.66f && allySwordPercentage >= decreaseAmount) {

			allySwordPercentage -= decreaseAmount;
		}
		else if(allyShieldPercentage >= decreaseAmount) {

			allyShieldPercentage -= decreaseAmount;
		}

		UpdateHUDGears ();
		yield return new WaitForSeconds(5);
		StartCoroutine(DecreaseAllyStrength());
	}

	void UpdateHUDGears () {

		// Determine which part of the army is weakest
		Weapon weakestPart = Weapon.Shield;
		float weakestPercentage = Mathf.Min (enemyAxePercentage, enemySwordPercentage, enemyShieldPercentage);

		if (weakestPercentage == enemyAxePercentage) {

			weakestPart = Weapon.Axe;
		}
		else if (weakestPercentage == enemySwordPercentage) {

			weakestPart = Weapon.Sword;
		}
		else if (weakestPercentage == enemyShieldPercentage) {

			weakestPart = Weapon.Shield;
		}

		// Change HUD to reflect weakest composition
		hudGears.TurnToWeapon(weakestPart);
	}

	IEnumerator LaunchProjectile () {

		yield return new WaitForSeconds (projectileCooldown);

		Bounds bounds = this.GetComponent<Collider> ().bounds;
		float randomX = Random.Range (bounds.min.x, bounds.max.x);
		float randomZ = Random.Range (losePosition.z, this.transform.position.z);
		Vector3 spawnPosition = new Vector3(randomX, 0, randomZ);

		Instantiate (trebuchetProjectileController, spawnPosition, Quaternion.identity);

		StartCoroutine (LaunchProjectile ());
	}
}
