using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum Weapon { Sword, Axe, Shield, None }

[DisallowMultipleComponent]
public class Forge : MonoBehaviour {

	[Header("Parameters")]
	[SerializeField] int maxMetalAmount = 5;

	[Header("References")]
	[SerializeField] Image metalBarFill;
	[SerializeField] Image meltingBarFill;
	[SerializeField] GameObject forgedIngotPrefab;
	[SerializeField] GameObject aButtonGameobject;
	[SerializeField] GameObject smokeParticleEffect;

	int meltingAmount = 0;
	int metalAmount = 0;
	Weapon weaponForging;
	AudioSource audioSource;

	void Awake () {

		meltingBarFill.fillAmount = 0;
		metalBarFill.fillAmount = 0;
		meltingBarFill.transform.parent.gameObject.SetActive(false);
		aButtonGameobject.SetActive(false);
		smokeParticleEffect.SetActive(false);
		audioSource = this.GetComponent<AudioSource>();
	}

	void Update () {

		if(meltingAmount > 0) {
			
			if(meltingBarFill.fillAmount > 0) {

				meltingBarFill.fillAmount -= 0.05f * Time.deltaTime;

				if(smokeParticleEffect.activeSelf == false) { smokeParticleEffect.SetActive(true); }
			}
			else if(meltingBarFill.fillAmount <= 0) {
				
				meltingAmount--;
				metalAmount++;
				meltingBarFill.transform.parent.gameObject.SetActive(false);
				metalBarFill.fillAmount = ((float) metalAmount / maxMetalAmount);
				smokeParticleEffect.SetActive(false);
			}
		}
	}

	void OnTriggerEnter(Collider other) {

		if(other.CompareTag("Player")) {

			if(other.GetComponent<PlayerController>().HasObject()) {

				aButtonGameobject.SetActive(true);
			}
			else if(metalAmount > 0) {

				aButtonGameobject.SetActive(true);
			}
		}
	}

	void OnTriggerExit(Collider other) {

		if(other.CompareTag("Player")) {

			aButtonGameobject.SetActive(false);
		}
	}
		
	public void UpdateMeltingAmount() {

		meltingAmount++;
		meltingBarFill.fillAmount += 0.5f;
		meltingBarFill.transform.parent.gameObject.SetActive(true);
		//if(audioSource.isPlaying == false) { audioSource.Play(); }
	}

	public void RetrieveIngot (PlayerController player) {

		if (metalAmount > 0) {

			GameObject newIngot = Instantiate (forgedIngotPrefab, Vector3.zero, Quaternion.identity) as GameObject;
			player.ReceiveItem (newIngot);
			metalAmount--;
			metalBarFill.fillAmount = ((float)metalAmount / maxMetalAmount);
			aButtonGameobject.SetActive(false);
		}
		else {

			//audioSource.Stop();
		}
	}
}
