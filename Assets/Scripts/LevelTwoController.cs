using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class LevelTwoController : MonoBehaviour {
	
	void Start () {

		ConversationManager.Instance.StartConversation(this.GetComponent<ConversationComponent>().Conversations[0]);
	}

	void Update () {

		UpdateDialogue();
	}

	void UpdateDialogue () {

		if(ConversationManager.Instance._IsTalking()) {

			if(Input.anyKeyDown) {

				ConversationManager.Instance.stepSpeed = 0f;
			}

			if(Input.anyKeyDown && ConversationManager.Instance._GetStepCoroutine() == false) {

				ConversationManager.Instance._SetNextLine(true);
			}
		}

		// Check if intro convo is finished
		if(ConversationManager.Instance._IsTalking() == false /*&& uiController.gameObject.activeSelf == false*/) {
			Debug.Log("Convo finished");
//			currentState = TutorialState.SHIELD;
//			uiController.gameObject.SetActive(true);
//			uiController.ShowShieldInstructions();
//			uiController.ShowInstructions("Quick! Make 3 Shields for your army!");
//			for(int i = 0; i < 2; i++) { enemyLine.GetComponent<EnemyLine>().CreateSpecificWeaponDropoff(Weapon.Shield, 1000); }
		}

		// Check if tutorial complete
//		if(ConversationManager.Instance._IsTalking() == false && axeCount == requiredItemCount) {
//
//			StartCoroutine(FadeSceneToBlack());
//		}
	}
}
