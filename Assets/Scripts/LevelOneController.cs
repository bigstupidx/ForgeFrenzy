using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class LevelOneController : MonoBehaviour {

    enum TutorialState { SHIELD, SWORD, AXE };
    TutorialState currentState = TutorialState.SHIELD;

    [SerializeField] GameUIController uiController;

    [SerializeField] GameObject farLine;
    [SerializeField] GameObject midLine;
    [SerializeField] GameObject closeLine;
    [SerializeField] GameObject enemyLine;

    int shieldCount = 0;
    int swordCount = 0;
    int axeCount = 0;
    int requiredItemCount = 3;

	// Use this for initialization
	void Start () {
        currentState = TutorialState.SHIELD;
        uiController.ShowShieldInstructions();
        uiController.ShowInstructions("Quick! Make 3 Shields for your army!");
    }
	
	// Update is called once per frame
	void Update () {
        CheckArmyPosition();

        // put in temp input controlls here -- call all at the same time to make sure it works
        if (Input.GetKeyDown(KeyCode.Space)) {
            Debug.Log("Making Weapons");
            enemyLine.GetComponent<EnemyLine>().IncrementAxeCount();
            enemyLine.GetComponent<EnemyLine>().IncrementShieldCount();
            enemyLine.GetComponent<EnemyLine>().IncrementSwordCount();
        }
	}

    void CheckArmyPosition() {
        // Stop the enemy line before it advances too far
        if (currentState == TutorialState.SHIELD && enemyLine.transform.position.z <= farLine.transform.position.z) {
            Debug.Log("Stopping Enemy Line");
            enemyLine.GetComponent<EnemyLine>().StopMovement();
        } else if (currentState == TutorialState.SWORD && enemyLine.transform.position.z <= midLine.transform.position.z) {
            Debug.Log("Stopping Enemy Line");
            enemyLine.GetComponent<EnemyLine>().StopMovement();
        } else if (currentState == TutorialState.AXE && enemyLine.transform.position.z <= closeLine.transform.position.z) {
            Debug.Log("Stopping Enemy Line");
            enemyLine.GetComponent<EnemyLine>().StopMovement();
        }
    }

    public void IncreaseShieldCount() {
        if (currentState == TutorialState.SHIELD)
        {
            shieldCount++;
            // check task completion here
            if (shieldCount == requiredItemCount)
            {
                currentState++;
                uiController.ShowSwordInstructions();
                uiController.ShowInstructions("Quick! Make 3 Swords for your army!");
                enemyLine.GetComponent<EnemyLine>().RestartMovement();
            }
        }
    }

    public void IncreaseSwordCount() {
        if (currentState == TutorialState.SWORD)
        {
            swordCount++;
            // check task completion here
            if (swordCount == requiredItemCount)
            {
                currentState++;
                uiController.ShowAxeInstructions();
                uiController.ShowInstructions("Quick! Make 3 Axes for your army!");
                enemyLine.GetComponent<EnemyLine>().RestartMovement();
            }
        }
    }

    public void IncreaseAxeCount() {
        if (currentState == TutorialState.AXE)
        {
            axeCount++;
            // check task completion here
            if (axeCount == requiredItemCount)
            {
                //currentState++;
                Debug.Log("Tutorial Complete");
                uiController.TutorialComplete();
            }
        }
    }
}


