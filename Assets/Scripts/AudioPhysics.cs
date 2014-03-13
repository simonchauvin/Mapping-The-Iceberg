using UnityEngine;
using System.Collections;

public class AudioPhysics : MonoBehaviour {
	/// <summary>
	/// The clips.
	/// </summary>
	public AudioClip[] clips = new AudioClip[8];

	private Transform player;
	private World world;
	private int playerLayerMask = 1 << 8;
	private int architectureLayerMask = 1 << 9;
	private RaycastHit hitInfo;

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag("Player").transform;
		world = GameObject.FindGameObjectWithTag("World").GetComponent("World") as World;
	}
	
	// Update is called once per frame
	void Update () {
		// Check if there is an obstacle between the architecture and the player
		if (Physics.Raycast(transform.position, player.transform.position - transform.position, out hitInfo, Mathf.Infinity)) {
			//TODO do something according to the thickness of the wall (hitInfo.collider)
			if (hitInfo.collider.transform.CompareTag("Player")) {
				// Select the normal audio clip to play
				if (world.isDay()) {
					// Check if it rains
					if (world.isRaining()) {
						// Day raining
						audio.clip = clips[4];
					} else {
						// Day
						audio.clip = clips[0];
					}
				} else {
					// Check if it rains
					if (world.isRaining()) {
						// Night raining
						audio.clip = clips[5];
					} else {
						// Night
						audio.clip = clips[1];
					}
				}
				Debug.DrawLine(transform.position, hitInfo.point, Color.green);
			} else {
				// Select the filtered audio clip to play
				if (world.isDay()) {
					// Check if it rains
					if (world.isRaining()) {
						// Day raining filtered
						audio.clip = clips[6];
					} else {
						// Day filtered
						audio.clip = clips[2];
					}
				} else {
					// Check if it rains
					if (world.isRaining()) {
						// Night raining filtered
						audio.clip = clips[7];
					} else {
						// Night filtered
						audio.clip = clips[3];
					}
				}
			}
			if (!audio.isPlaying) {
				audio.Play();
			}
		}
	}
}
