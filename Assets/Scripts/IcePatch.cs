using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IcePatch : MonoBehaviour {

    [SerializeField] float speedBoost = 1.5f;
    [SerializeField] float playerSpeed = 0.1f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            other.GetComponent<PlayerController>().SetMoveSpeed(speedBoost);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) {
            other.GetComponent<PlayerController>().SetMoveSpeed(playerSpeed);
        }
    }
}
