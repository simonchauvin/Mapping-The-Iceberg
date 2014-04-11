using UnityEngine;
using System.Collections;

public class AudioPhysics : MonoBehaviour {
	public const string TRANSITION_FROM_DAY = "DAY";
	public const string TRANSITION_FROM_NIGHT = "NIGHT";
	public const string TRANSITION_FROM_DAY_FILTERED = "DAY FILTERED";
	public const string TRANSITION_FROM_NIGHT_FILTERED = "NIGHT FILTERED";
	/// <summary>
	/// The clips.
	/// </summary>
	public string[] clips = new string[4];

	public float timeTransitionTime = 3.0f;
	public float wallTransitionTime = 1.0f;

	private Transform player;
	/// <summary>
	/// Name of the sound being used.
	/// </summary>
	private string currentClip;
	private World world;
	private int playerLayerMask = 1 << 8;
	private int otherLayerMask = 1 << 9;
	private int soundLayerMask = 1 << 10;
	private RaycastHit hitInfo;
	private int savedTime;
	private int savedTimeDay;
	private int savedTimeNight;
	private int savedTimeDayFiltered;
	private int savedTimeNightFiltered;
	private bool transitionToDay;
	private bool transitionToNight;
	private bool transitionToDayFiltered;
	private bool transitionToNightFiltered;
	private AudioClip newAudio;
	private bool timeTransitioning;
	private bool wallTransitioning;
	private float timerTimeTransition;
	private float timerWallTransition;
	private bool endingTransition;
	private bool canEndTransition;

	private string transitionFrom;

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag("Player").transform;
		world = GameObject.FindGameObjectWithTag("World").GetComponent("World") as World;
		currentClip = null;
		newAudio = null;
		timeTransitioning = false;
		wallTransitioning = false;
		timerTimeTransition = 0.0f;
		timerWallTransition = 0.0f;
		transitionToDay = false;
		transitionToNight = false;
		transitionToDayFiltered = false;
		transitionToNightFiltered = false;
		endingTransition = false;
		canEndTransition = false;
		otherLayerMask = ~otherLayerMask;

		transitionFrom = TRANSITION_FROM_DAY;
	}

	// Update is called once per frame
	void Update () {
		// Transitionning from or to a day or night song
		if (timeTransitioning) {
			timerTimeTransition += Time.deltaTime;
			if (!endingTransition) {
				audio.volume = Mathf.Lerp(1.0f, 0.0f, timerTimeTransition / 1.5f);
				if (audio.volume <= 0.0f) {
                    int time = audio.timeSamples;
					/*int time = 0;
					if (transitionToDay) {
						time = savedTimeDay;
					} else if (transitionToNight) {
						time = savedTimeNight;
					} else if (transitionToDayFiltered) {
						time = savedTimeDayFiltered;
					} else if (transitionToNightFiltered) {
						time = savedTimeNightFiltered;
					}*/
					audio.Stop();
					audio.clip = newAudio;
					audio.timeSamples = time;
					audio.Play();
					timerTimeTransition = 0.0f;
					endingTransition = true;
					transitionToDay = false;
					transitionToNight = false;
					transitionToDayFiltered = false;
					transitionToNightFiltered = false;
				}
			} else if (canEndTransition) {
				audio.volume = Mathf.Lerp(0.0f, 1.0f, timerTimeTransition / 1.5f);
				if (audio.volume >= 1.0f) {
					timeTransitioning = false;
					endingTransition = false;
					newAudio = null;
				}
			} else {
				timeTransitioning = false;
				endingTransition = false;
				canEndTransition = false;
				newAudio = null;
			}
		} else if (wallTransitioning) {
			//Transitionning from or to a filtered sound
			timerWallTransition += Time.deltaTime;
			if (!endingTransition) {
				audio.volume = Mathf.Lerp(1.0f, 0.0f, timerWallTransition / 0.5f);
				if (audio.volume <= 0.0f) {
                    int time = audio.timeSamples;
					audio.Stop();
					audio.clip = newAudio;
					//audio.timeSamples = savedTime;
                    audio.timeSamples = time;
					audio.Play();
					timerWallTransition = 0.0f;
					endingTransition = true;
				}
			} else if (canEndTransition) {
				audio.volume = Mathf.Lerp(0.0f, 1.0f, timerWallTransition / 0.5f);
				if (audio.volume >= 1.0f) {
					wallTransitioning = false;
					endingTransition = false;
					newAudio = null;
				}
			} else {
				wallTransitioning = false;
				endingTransition = false;
				canEndTransition = false;
				newAudio = null;
			}
		}

		// Check if the player is in the trigger area of the sound
		bool timeChanged = false;
		bool wallChanged = false;
		if (Physics.Raycast(transform.position, player.position - transform.position, out hitInfo, audio.maxDistance, otherLayerMask)) {
			Debug.DrawLine(transform.position, hitInfo.point, Color.blue);
			//Enter the trigger area
			if (audio.clip == null) {
				audio.clip = Resources.Load<AudioClip>("Sounds/" + currentClip) as AudioClip;
				if (audio.clip != null) {
					audio.timeSamples = savedTime;
					audio.Play();
				}
			}
			// Check if there is a wall between the audio source and the player
			if (Physics.Raycast(transform.position, player.position - transform.position, out hitInfo, audio.maxDistance)) {
				Debug.DrawLine(transform.position, hitInfo.point, Color.green);
				if (hitInfo.collider.transform.CompareTag("Player")) {
					// Select the normal audio clip to play
					if (world.isDay()) {
						// Day
						if (currentClip != null && !transitionFrom.Equals(TRANSITION_FROM_DAY)) {
							if (transitionFrom.Equals(TRANSITION_FROM_NIGHT)) {
								savedTimeNight = audio.timeSamples;
								timeChanged = true;
							} else if (transitionFrom.Equals(TRANSITION_FROM_DAY_FILTERED)) {
								savedTime = audio.timeSamples;
								wallChanged = true;
							} else if (transitionFrom.Equals(TRANSITION_FROM_NIGHT_FILTERED)) {
								savedTimeNightFiltered = audio.timeSamples;
								timeChanged = true;
							}
							currentClip = clips[0];
							if (timeChanged) {
								timeTransitioning = true;
								timerTimeTransition = 0.0f;
								wallTransitioning = false;
								timerWallTransition = 0.0f;
								endingTransition = false;
								canEndTransition = false;
							} else if (wallChanged) {
								wallTransitioning = true;
								timerWallTransition = 0.0f;
								timeTransitioning = false;
								timerTimeTransition = 0.0f;
								endingTransition = false;
								canEndTransition = false;
							}
							newAudio = Resources.Load<AudioClip>("Sounds/" + clips[0]) as AudioClip;
							if (newAudio != null) {
								if (timeTransitioning) {
									transitionToDay = true;
								}
								canEndTransition = true;
							}
						} else if (currentClip == null) {
							currentClip = clips[0];
							audio.clip = Resources.Load<AudioClip>("Sounds/" + clips[0]) as AudioClip;
							if (audio.clip != null) {
                                Debug.Log(savedTimeDay);
								audio.timeSamples = savedTimeDay;
								audio.Play();
							}
						}
						transitionFrom = TRANSITION_FROM_DAY;
					} else {
						// Night
						if (currentClip != null && !transitionFrom.Equals(TRANSITION_FROM_NIGHT)) {
							if (transitionFrom.Equals(TRANSITION_FROM_DAY)) {
								savedTimeDay = audio.timeSamples;
								timeChanged = true;
							} else if (transitionFrom.Equals(TRANSITION_FROM_DAY_FILTERED)) {
								savedTimeDayFiltered = audio.timeSamples;
								timeChanged = true;
							} else if (transitionFrom.Equals(TRANSITION_FROM_NIGHT_FILTERED)) {
								savedTime = audio.timeSamples;
								wallChanged = true;
							}
							currentClip = clips[1];
							if (timeChanged) {
								timeTransitioning = true;
								timerTimeTransition = 0.0f;
								wallTransitioning = false;
								timerWallTransition = 0.0f;
								endingTransition = false;
								canEndTransition = false;
							} else if (wallChanged) {
								wallTransitioning = true;
								timerWallTransition = 0.0f;
								timeTransitioning = false;
								timerTimeTransition = 0.0f;
								endingTransition = false;
								canEndTransition = false;
							}
							newAudio = Resources.Load<AudioClip>("Sounds/" + clips[1]) as AudioClip;
							if (newAudio != null) {
								if (timeTransitioning) {
									transitionToNight = true;
								}
								canEndTransition = true;
							}
						} else if (currentClip == null) {
							currentClip = clips[1];
							audio.clip = Resources.Load<AudioClip>("Sounds/" + clips[1]) as AudioClip;
							if (audio.clip != null) {
								audio.timeSamples = savedTimeNight;
								audio.Play();
							}
						}
						transitionFrom = TRANSITION_FROM_NIGHT;
					}
				} else {
					// Select the filtered audio clip to play
					if (world.isDay()) {
						// Day filtered
						if (currentClip != null && !transitionFrom.Equals(TRANSITION_FROM_DAY_FILTERED)) {
							// Transition
							if (transitionFrom.Equals(TRANSITION_FROM_DAY)) {
								savedTime = audio.timeSamples;
								wallChanged = true;
							} else if (transitionFrom.Equals(TRANSITION_FROM_NIGHT)) {
								savedTimeNight = audio.timeSamples;
								timeChanged = true;
							} else if (transitionFrom.Equals(TRANSITION_FROM_NIGHT_FILTERED)) {
								savedTimeNightFiltered = audio.timeSamples;
								timeChanged = true;
							}
							currentClip = clips[2];
							if (timeChanged) {
								timeTransitioning = true;
								timerTimeTransition = 0.0f;
								wallTransitioning = false;
								timerWallTransition = 0.0f;
								endingTransition = false;
								canEndTransition = false;
							} else if (wallChanged) {
								wallTransitioning = true;
								timerWallTransition = 0.0f;
								timeTransitioning = false;
								timerTimeTransition = 0.0f;
								endingTransition = false;
								canEndTransition = false;
							}
							newAudio = Resources.Load<AudioClip>("Sounds/" + clips[2]) as AudioClip;
							if (newAudio != null) {
								if (timeTransitioning) {
									transitionToDayFiltered = true;
								}
								canEndTransition = true;
							}
						} else if (currentClip == null) {
							currentClip = clips[2];
							audio.clip = Resources.Load<AudioClip>("Sounds/" + clips[2]) as AudioClip;
							if (audio.clip != null) {
								audio.timeSamples = savedTimeDayFiltered;
								audio.Play();
							}
						}
						transitionFrom = TRANSITION_FROM_DAY_FILTERED;
					} else {
						// Night filtered
						if (currentClip != null && !transitionFrom.Equals(TRANSITION_FROM_NIGHT_FILTERED)) {
							// Transition
							if (transitionFrom.Equals(TRANSITION_FROM_DAY)) {
								savedTimeDay = audio.timeSamples;
								timeChanged = true;
							} else if (transitionFrom.Equals(TRANSITION_FROM_NIGHT)) {
								savedTime = audio.timeSamples;
								wallChanged = true;
							} else if (transitionFrom.Equals(TRANSITION_FROM_DAY_FILTERED)) {
								savedTimeDayFiltered = audio.timeSamples;
								timeChanged = true;
							}
							currentClip = clips[3];
							if (timeChanged) {
								timeTransitioning = true;
								timerTimeTransition = 0.0f;
								wallTransitioning = false;
								timerWallTransition = 0.0f;
								endingTransition = false;
								canEndTransition = false;
							} else if (wallChanged) {
								wallTransitioning = true;
								timerWallTransition = 0.0f;
								timeTransitioning = false;
								timerTimeTransition = 0.0f;
								endingTransition = false;
								canEndTransition = false;
							}
							newAudio = Resources.Load<AudioClip>("Sounds/" + clips[3]) as AudioClip;
							if (newAudio != null) {
								if (timeTransitioning) {
									transitionToNightFiltered = true;
								}
								canEndTransition = true;
							}
						} else if (currentClip == null) {
							currentClip = clips[3];
							audio.clip = Resources.Load<AudioClip>("Sounds/" + clips[3]) as AudioClip;
							if (audio.clip != null) {
								audio.timeSamples = savedTimeNightFiltered;
								audio.Play();
							}
						}
						transitionFrom = TRANSITION_FROM_NIGHT_FILTERED;
					}
				}
			}
		} else {
			//Leave the trigger area
			if (audio.clip != null) {
				savedTime = audio.timeSamples;
				audio.Stop();
				Resources.UnloadAsset(audio.clip);
				audio.clip = null;
				timeTransitioning = false;
				wallTransitioning = false;
				timerTimeTransition = 0.0f;
				timerWallTransition = 0.0f;
				endingTransition = false;
				newAudio = null;
			}
		}
	}
}
