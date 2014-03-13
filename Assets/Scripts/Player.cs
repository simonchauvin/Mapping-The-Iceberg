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


}
