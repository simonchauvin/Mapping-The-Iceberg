using UnityEngine;
using System.Collections;

public class AudioPhysics : MonoBehaviour {
	/// <summary>
	/// The clips.
	/// </summary>
	public string[] clips = new string[8];

	private Transform player;
	/// <summary>
	/// Name of the sound being used.
	/// </summary>
	private string currentClip;
	private World world;
	private int playerLayerMask = 1 << 8;
	private int architectureLayerMask = 1 << 9;
	private RaycastHit hitInfo;
	private int savedTime;

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag("Player").transform;
		world = GameObject.FindGameObjectWithTag("World").GetComponent("World") as World;
	}
	
	// Update is called once per frame
	void Update () {
		// Check if there is an obstacle between the architecture and the player
		/*if (Physics.Raycast(transform.position, player.transform.position - transform.position, out hitInfo, Mathf.Infinity)) {
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
		}*/
		// Day or night ?
		bool timeChanged = false;
		if (world.isDay()) {
			if (currentClip == null || !currentClip.Equals(clips[0])) {
				currentClip = clips[0];
				timeChanged = true;
			}
		} else {
			if (currentClip == null || !currentClip.Equals(clips[1])) {
				currentClip = clips[1];
				timeChanged = true;
			}
		}
		if (timeChanged && audio.isPlaying) {
			audio.clip = Resources.Load<AudioClip>("Sounds/" + currentClip) as AudioClip;
			audio.Play();
		}
	}

	void OnTriggerEnter(Collider other) {
		if (other.CompareTag("Player")) {
			Debug.Log(currentClip);
			audio.clip = Resources.Load<AudioClip>("Sounds/" + currentClip) as AudioClip;
			audio.timeSamples = savedTime;
			// TODO audio.timeSamples++ pour faire croire qu'il a continué à se passer des trucs
			audio.Play();
		}
	}

	void OnTriggerExit(Collider other) {
		if (other.CompareTag("Player")) {
			savedTime = audio.timeSamples;
			audio.Stop();
			Resources.UnloadAsset(audio.clip);
			audio.clip = null;
		}
	}
}
