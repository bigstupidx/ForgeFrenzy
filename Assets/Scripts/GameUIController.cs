using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour {

    [SerializeField] Text gameOverText;

    [SerializeField] GameObject gameController;

    [Header("Tutorial UI")]
    [SerializeField] Text instructionsText;
    [SerializeField] Text instructionsTextShadow;
    [SerializeField] GameObject shieldRecipe;
    [SerializeField] GameObject swordRecipe;
    [SerializeField] GameObject axeRecipe;

	// Use this for initialization
	void Awake () {
        // if (gameOverText == null) { gameOverText = GameObject.Find("Game Over Text").GetComponent<Text>(); }
        gameOverText.gameObject.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ShowInstructions(string instructions) {
        instructionsText.gameObject.SetActive(true);
        instructionsTextShadow.gameObject.SetActive(true);
        instructionsText.text = instructions;
        instructionsTextShadow.text = instructions;
    }

    public void ShowShieldInstructions() {
        shieldRecipe.SetActive(true);
        swordRecipe.SetActive(false);
        axeRecipe.SetActive(false);
    }

    public void ShowSwordInstructions() {
        shieldRecipe.SetActive(false);
        swordRecipe.SetActive(true);
        axeRecipe.SetActive(false);
    }

    public void ShowAxeInstructions() {
        shieldRecipe.SetActive(false);
        swordRecipe.SetActive(false);
        axeRecipe.SetActive(true);
    }

    IEnumerator InstructionTimeout() {
        yield return new WaitForSeconds(5);

        instructionsText.gameObject.SetActive(false);
    }

    public void TutorialComplete() {
        gameOverText.text = "Army Repelled!";
        gameOverText.gameObject.SetActive(true);
        instructionsText.gameObject.SetActive(false);
        instructionsTextShadow.gameObject.SetActive(false);
    }
}
