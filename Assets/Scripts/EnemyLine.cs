using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class EnemyLine : MonoBehaviour {

    GameObject gameController;
    LevelOneController levelOneController;
    bool isLevelOne = false;

    [Header("Enemy Line Parameters")]
	[SerializeField] float startingZ = 33f;
	[SerializeField] float losingZ = -1f;
	[SerializeField] float winningZ = 35f;
	[SerializeField] float warningZ = 10f;
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

	Vector3 winPosition;
	Vector3 losePosition;


	void Awake () {

		this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, startingZ);
		winPosition = new Vector3(this.transform.position.x, this.transform.position.y, winningZ);
		losePosition = new Vector3(this.transform.position.x, this.transform.position.y, losingZ);
		StartCoroutine (SpawnItem ());
		StartCoroutine(DecreaseArmyStrengths());
		StartCoroutine (LaunchProjectile ());
	}

    void Start() {
        gameController = GameObject.FindGameObjectWithTag("GameController");
        if (gameController.GetComponent<LevelOneController>() != null) {
            isLevelOne = true;
            levelOneController = gameController.GetComponent<LevelOneController>();
            Debug.Log("Level One");
        } else {
            isLevelOne = false;
            Debug.Log("Not Level One");
        }
    }

	void Update () {
		
		this.transform.position = Vector3.MoveTowards(this.transform.position, losePosition, moveSpeed * Time.deltaTime);

		if(this.transform.position.z <= warningZ) {

			AudioSource[] audioSources = this.GetComponentsInChildren<AudioSource>();

			foreach (AudioSource audioSource in audioSources) {

				audioSource.pitch = 1.1f;
				audioSource.spatialBlend = 0.5f;
			}
		}

		CheckEndGameConditions();
	}

	void CheckEndGameConditions () {

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

		if (Random.value < 0.7f) {
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
        if (isLevelOne) {
            levelOneController.IncreaseAxeCount();
        }
	}

	public void IncrementSwordCount () {

		allySwordPercentage += 0.05f;
		UpdateMoveSpeed();
        if (isLevelOne) {
            levelOneController.IncreaseSwordCount();
        }
    }

	public void IncrementShieldCount () {

		allyShieldPercentage += 0.05f;
		UpdateMoveSpeed();
        if (isLevelOne) {
            levelOneController.IncreaseShieldCount();
        }
    }

	IEnumerator DecreaseArmyStrengths () {

		if(allyAxePercentage > enemyShieldPercentage) { enemyShieldPercentage *= Random.Range(0.95f, 1.0f); }
		if(allySwordPercentage > enemyAxePercentage) { enemyAxePercentage *= Random.Range(0.95f, 1.0f); }
		if(allyShieldPercentage > enemySwordPercentage) { enemySwordPercentage *= Random.Range(0.95f, 1.0f); }

		if(enemyAxePercentage >= allyShieldPercentage) { allyShieldPercentage *= Random.Range(0.95f, 1.0f); }
		if(enemySwordPercentage >= allyAxePercentage) { allyAxePercentage *= Random.Range(0.95f, 1.0f); }
		if(enemyShieldPercentage >= allySwordPercentage) { allySwordPercentage *= Random.Range(0.95f, 1.0f); }

		UpdateHUDGears ();
		yield return new WaitForSeconds(5);
		StartCoroutine(DecreaseArmyStrengths());
	}

	void UpdateHUDGears () {

		// Determine which part of the army is weakest
		Weapon weakestPart = Weapon.Shield;
		float weakestPercentage = Mathf.Min (allyAxePercentage, allySwordPercentage, allyShieldPercentage);

		if (weakestPercentage == allyAxePercentage) {

			weakestPart = Weapon.Axe;
		}
		else if (weakestPercentage == allySwordPercentage) {

			weakestPart = Weapon.Sword;
		}
		else if (weakestPercentage == allyShieldPercentage) {

			weakestPart = Weapon.Shield;
		}
	}

	IEnumerator LaunchProjectile () {
		
		while(true) {
			
			Bounds bounds = this.GetComponent<Collider> ().bounds;
			float randomX = Random.Range (bounds.min.x, bounds.max.x);
			float randomZ = Random.Range (losePosition.z, this.transform.position.z);
			Vector3 spawnPosition = new Vector3(randomX, 0, randomZ);

			GameObject newTrebuchetProjectile = Instantiate (trebuchetProjectileController, spawnPosition, Quaternion.identity) as GameObject;

			// Get player to target
			GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
			List<GameObject> playersOutside = new List<GameObject>();
			for(int i = 0; i < players.Length; i++) {

				if(players[i].transform.position.z > losePosition.z) {

					playersOutside.Add(players[i]);
				}
			}

			players = playersOutside.ToArray();

			if(players.Length > 0) {
				
				GameObject playerToTarget = players[Random.Range(0, players.Length)];
				newTrebuchetProjectile.GetComponent<TrebuchetProjectileController>().SetTarget(playerToTarget);
			}

			yield return new WaitForSeconds (projectileCooldown);
		}
	}

    public void StopMovement() {
        moveSpeed = 0;
    }


    public void RestartMovement() {
        moveSpeed = 1;
    }
}