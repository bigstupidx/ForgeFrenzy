using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class ChoppedWood : MonoBehaviour {

	GameObject workbenchUI;

	void Awake () {

		workbenchUI = this.transform.FindChild ("Picked Up UI").gameObject;
		HidePickedUpUI ();
	}

	public void ShowPickedUpUI () {

		workbenchUI.SetActive (true);
	}

	public void HidePickedUpUI () {

		workbenchUI.SetActive (false);
	}
}
