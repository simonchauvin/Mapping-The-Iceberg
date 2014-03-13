using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	// Use this for initialization
	void Start () {
		// Ignore collisions between the player and architecture
		Physics.IgnoreLayerCollision(8, 9);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider other) {
		if (other.CompareTag("Sound")) {
			other.gameObject.SetActive(true);
		}
	}

	void OnTriggerExit(Collider other) {
		if (other.CompareTag("Sound")) {
			other.gameObject.SetActive(false);
		}
	}
}
