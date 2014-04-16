using UnityEngine;
using System.Collections;

public class AudioPhysics : MonoBehaviour {
	public const string TRANSITION_FROM_DAY = "DAY";
	public const string TRANSITION_FROM_NIGHT = "NIGHT";
	public const string TRANSITION_FROM_DAY_FILTERED = "DAY FILTERED";
	public const string TRANSITION_FROM_NIGHT_FILTERED = "NIGHT FILTERED";

    public float maxVolume = 1.0f;
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
	private int savedTimeChanging;
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
	private float timerTimeTransition1;
	private float timerWallTransition1;
	private float timerTimeTransition2;
	private float timerWallTransition2;
	private bool endingTransition;
	private bool canEndTransition;

	private string transitionFrom;

	private AudioSource defaultSource;
	private AudioSource transitionSource;

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag("Player").transform;
		world = GameObject.FindGameObjectWithTag("World").GetComponent("World") as World;
		currentClip = null;
		newAudio = null;
		timeTransitioning = false;
		wallTransitioning = false;
		timerTimeTransition1 = 0.0f;
		timerWallTransition1 = 0.0f;
		timerTimeTransition2 = 0.0f;
		timerWallTransition2 = 0.0f;
		transitionToDay = false;
		transitionToNight = false;
		transitionToDayFiltered = false;
		transitionToNightFiltered = false;
		endingTransition = false;
		canEndTransition = false;
		otherLayerMask = ~otherLayerMask;

		transitionFrom = TRANSITION_FROM_DAY;

		defaultSource = GetComponents<AudioSource>()[0];
		transitionSource = GetComponents<AudioSource>()[1];
	}

	// Update is called once per frame
	void Update () {
		// Transitionning from or to a day or night song
		if (timeTransitioning) {
			if (!endingTransition) {
				//Debug.Log("fade out");
				defaultSource.volume = 0.0f;
				defaultSource.volume = Mathf.Lerp(maxVolume, 0.0f, timerTimeTransition1 / 8.5f);
				if (defaultSource.volume <= 0.0f) {
					timerTimeTransition1 = 0.0f;
					endingTransition = true;
					transitionToDay = false;
					transitionToNight = false;
					transitionToDayFiltered = false;
					transitionToNightFiltered = false;
				}
			}
			if (canEndTransition) {
				if (transitionSource.clip == null) {
					transitionSource.clip = newAudio;
					transitionSource.volume = 0.0f;
					transitionSource.timeSamples = savedTimeChanging;
					transitionSource.Play();
				}
				transitionSource.volume = Mathf.Lerp(0.0f, maxVolume, timerTimeTransition2 / 8.5f);
				if (transitionSource.volume >= maxVolume)
                {
					savedTimeChanging = defaultSource.timeSamples;
					timeTransitioning = false;
					timerTimeTransition2 = 0.0f;
					timeTransitioning = false;
					endingTransition = false;
					newAudio = null;
					defaultSource.Stop();
					defaultSource.clip = transitionSource.clip;
					defaultSource.timeSamples = transitionSource.timeSamples;
					defaultSource.volume = transitionSource.volume;
					defaultSource.Play();
					transitionSource.Stop();
					transitionSource.clip = null;
				}
			}
			if (!canEndTransition && endingTransition) {
				timeTransitioning = false;
				endingTransition = false;
				canEndTransition = false;
				newAudio = null;
				timerTimeTransition1 = 0.0f;
				timerTimeTransition2 = 0.0f;
			}
			timerTimeTransition1 += Time.deltaTime;
			timerTimeTransition2 += Time.deltaTime;
		} else if (wallTransitioning) {
			//Transitionning from or to a filtered sound
			timerWallTransition1 += Time.deltaTime;
			if (!endingTransition) {
				audio.volume = Mathf.Lerp(maxVolume, 0.0f, timerWallTransition1 / 0.10f);
				if (audio.volume <= 0.0f) {
					int time = audio.timeSamples;
					audio.Stop();
					audio.clip = newAudio;
					//audio.timeSamples = savedTime;
					audio.timeSamples = time;
					audio.Play();
					timerWallTransition1 = 0.0f;
					endingTransition = true;
				}
			} else if (canEndTransition) {
				audio.volume = Mathf.Lerp(0.0f, maxVolume, timerWallTransition1 / 0.10f);
				if (audio.volume >= maxVolume)
				{
					wallTransitioning = false;
					endingTransition = false;
					newAudio = null;
				}
			} else {
				wallTransitioning = false;
				endingTransition = false;
				canEndTransition = false;
				newAudio = null;
				timerWallTransition1 = 0.0f;
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
							if (timeChanged) {
								timeTransitioning = true;
								timerTimeTransition1 = 0.0f;
								timerTimeTransition2 = 0.0f;
								wallTransitioning = false;
								timerWallTransition1 = 0.0f;
								timerWallTransition2 = 0.0f;
								if (currentClip.Length == 0) {
									endingTransition = true;
								} else {
									endingTransition = false;
								}
								canEndTransition = false;
							} else if (wallChanged) {
								wallTransitioning = true;
								timerWallTransition1 = 0.0f;
								timerWallTransition2 = 0.0f;
								timeTransitioning = false;
								timerTimeTransition1 = 0.0f;
								timerTimeTransition2 = 0.0f;
								endingTransition = false;
								canEndTransition = false;
							}
							currentClip = clips[0];
							newAudio = Resources.Load<AudioClip>("Sounds/" + clips[0]) as AudioClip;
							if (newAudio != null) {
								if (timeTransitioning) {
									transitionToDay = true;
								}
								canEndTransition = true;
							}
						} else if (currentClip == null) {
							Debug.Log(transitionFrom);
							currentClip = clips[0];
							audio.clip = Resources.Load<AudioClip>("Sounds/" + clips[0]) as AudioClip;
							if (audio.clip != null) {
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
							if (timeChanged) {
								timeTransitioning = true;
								timerTimeTransition1 = 0.0f;
								timerTimeTransition2 = 0.0f;
								wallTransitioning = false;
								timerWallTransition1 = 0.0f;
								timerWallTransition2 = 0.0f;
								if (currentClip.Length == 0) {
									endingTransition = true;
								} else {
									endingTransition = false;
								}
								canEndTransition = false;
							} else if (wallChanged) {
								wallTransitioning = true;
								timerWallTransition1 = 0.0f;
								timerWallTransition2 = 0.0f;
								timeTransitioning = false;
								timerTimeTransition1 = 0.0f;
								timerTimeTransition2 = 0.0f;
								endingTransition = false;
								canEndTransition = false;
							}
							currentClip = clips[1];
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
								timerTimeTransition1 = 0.0f;
								timerTimeTransition2 = 0.0f;
								wallTransitioning = false;
								timerWallTransition1 = 0.0f;
								timerWallTransition2 = 0.0f;
								endingTransition = false;
								canEndTransition = false;
							} else if (wallChanged) {
								wallTransitioning = true;
								timerWallTransition1 = 0.0f;
								timerWallTransition2 = 0.0f;
								timeTransitioning = false;
								timerTimeTransition1 = 0.0f;
								timerTimeTransition2 = 0.0f;
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
								timerTimeTransition1 = 0.0f;
								timerTimeTransition2 = 0.0f;
								wallTransitioning = false;
								timerWallTransition1 = 0.0f;
								timerWallTransition2 = 0.0f;
								endingTransition = false;
								canEndTransition = false;
							} else if (wallChanged) {
								wallTransitioning = true;
								timerWallTransition1 = 0.0f;
								timerWallTransition2 = 0.0f;
								timeTransitioning = false;
								timerTimeTransition1 = 0.0f;
								timerTimeTransition2 = 0.0f;
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
				if (transitionSource.clip != null) {
					defaultSource.clip = transitionSource.clip;
					defaultSource.timeSamples = transitionSource.timeSamples;
					defaultSource.volume = 1.0f;
					transitionSource.Stop();
					transitionSource.clip = null;
				}
				savedTime = defaultSource.timeSamples;
				defaultSource.Stop();
                Resources.UnloadUnusedAssets();
				audio.clip = null;
				timeTransitioning = false;
				wallTransitioning = false;
				timerTimeTransition1 = 0.0f;
				timerTimeTransition2 = 0.0f;
				timerWallTransition1 = 0.0f;
				timerWallTransition2 = 0.0f;
				endingTransition = false;
				newAudio = null;
			}
		}
	}
}
