using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

	[Header("Weapon Dropoff")]
	[SerializeField] GameObject weaponDropoffPrefab;
	[SerializeField] Sprite axeTimerSprite;
	[SerializeField] Sprite shieldTimerSprite;
	[SerializeField] Sprite swordTimerSprite;
	[SerializeField] GameObject weaponDropoffParent;
	[SerializeField] float weaponDropoffSpawnCooldown = 5;

	[Header("Item Spawn Parameters")]
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
			StartCoroutine(SpawnWeaponDropoff());
            Debug.Log("Not Level One");
        }
    }

	void Update () {

		// Move enemy line
		this.transform.position = Vector3.MoveTowards(this.transform.position, losePosition, moveSpeed * Time.deltaTime);

		CheckWarningConditions();
		CheckEndGameConditions();
	}

	void CheckWarningConditions () {

		if(this.transform.position.z <= warningZ) {

			AudioSource[] audioSources = this.GetComponentsInChildren<AudioSource>();

			foreach (AudioSource audioSource in audioSources) {

				audioSource.pitch = 1.1f;
				audioSource.spatialBlend = 0.5f;
			}
		}
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

	IEnumerator SpawnWeaponDropoff () {

		while (true) {

			if(weaponDropoffParent.transform.childCount < 3) {

				Vector3 spawnPosition = GetDropoffPosition();
				GameObject newWeaponDropoff = Instantiate(weaponDropoffPrefab, spawnPosition, Quaternion.identity) as GameObject;
				newWeaponDropoff.transform.SetParent(weaponDropoffParent.transform);

				Weapon weaponType = Weapon.None;
				Sprite weaponSprite = null;

				do {
					
					switch (Random.Range(0, 3)) {

						case 0:
							weaponType = Weapon.Axe;
							weaponSprite = axeTimerSprite;
							break;
						case 1:
							weaponType = Weapon.Shield;
							weaponSprite = shieldTimerSprite;
							break;
						case 2:
							weaponType = Weapon.Sword;
							weaponSprite = swordTimerSprite;
							break;
					}
				} while(weaponType == Weapon.None);

				newWeaponDropoff.GetComponent<WeaponDropoff>().SetWeaponRequired(weaponType);
				newWeaponDropoff.GetComponent<Image>().sprite = weaponSprite;
			}

			yield return new WaitForSeconds(weaponDropoffSpawnCooldown);
		}
	}

	Vector3 GetDropoffPosition () {

		Bounds bounds = this.GetComponent<Collider>().bounds;

		float xPos = 0;
		xPos = Random.Range(bounds.min.x, bounds.max.x);
		bool positionOverlapping;

		do {

			xPos = Random.Range(bounds.min.x, bounds.max.x);
			positionOverlapping = false;

			for(int i = 0; i < weaponDropoffParent.transform.childCount; i++) {

				float existingX = weaponDropoffParent.transform.GetChild(i).transform.position.x;

				if(Mathf.Abs(Mathf.Abs(xPos) - Mathf.Abs(existingX)) < 1) {
					
					positionOverlapping = true;
				}
			}

		} while(positionOverlapping);

		float yPos = 4f;
		float zPos = weaponDropoffParent.transform.position.z;

		return new Vector3(xPos, yPos, zPos);
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

	void OnCollisionEnter (Collision other) {

		if (other.transform.CompareTag ("Pickup")) {

			Destroy (other.gameObject);
		}
	}

	public void IncrementAxeCount () {

        if (isLevelOne) {
            levelOneController.IncreaseAxeCount();
        }
	}

	public void IncrementSwordCount () {

        if (isLevelOne) {
            levelOneController.IncreaseSwordCount();
        }
    }

	public void IncrementShieldCount () {

        if (isLevelOne) {
            levelOneController.IncreaseShieldCount();
        }
    }

    public void StopMovement() {
        moveSpeed = 0;
    }

    public void RestartMovement() {
        moveSpeed = 1;
    }

	public void CreateSpecificWeaponDropoff (Weapon weaponType, float destroyTime = 60) {

		Vector3 spawnPosition = GetDropoffPosition();
		GameObject newWeaponDropoff = Instantiate(weaponDropoffPrefab, spawnPosition, Quaternion.identity) as GameObject;
		newWeaponDropoff.transform.SetParent(weaponDropoffParent.transform);
		newWeaponDropoff.GetComponent<WeaponDropoff>().SetWeaponRequired(weaponType);
		newWeaponDropoff.GetComponent<WeaponDropoff>().SetDestroyTime(destroyTime);

		Sprite weaponSprite = null;
		if(weaponType == Weapon.Axe) { weaponSprite = axeTimerSprite; }
		else if(weaponType == Weapon.Shield) { weaponSprite = shieldTimerSprite; }
		else if(weaponType == Weapon.Sword) { weaponSprite = swordTimerSprite; }
		newWeaponDropoff.GetComponent<Image>().sprite = weaponSprite;
	}
}