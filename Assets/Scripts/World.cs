using UnityEngine;
using System.Collections;

public class World : MonoBehaviour {
	/// <summary>
	/// In hours.
	/// </summary>
	public int SUN_RISE;
	public int SUN_SET;
	public float STARTING_TIME;
	public int MIN_TIME_BETWEEN_WEATHER_CHANGE;
	public int MAX_TIME_BETWEEN_WEATHER_CHANGE;

	/// <summary>
	/// Speed at which the time is passing.
	/// A factor of 3600 means that a day lasts 24 seconds.
	/// </summary>
	public int passingTimeFactor;

	/// <summary>
	/// The current time in seconds.
	/// </summary>
	private float time;

	/// <summary>
	/// The time between weather change in seconds.
	/// </summary>
	private float timeBetweenWeatherChange;

	// Use this for initialization
	void Start () {
		time = STARTING_TIME * 3600;
	}
	
	// Update is called once per frame
	void Update () {
		// Time passing
		time += Time.deltaTime * passingTimeFactor;

		// Weather change
		timeBetweenWeatherChange += Time.deltaTime * passingTimeFactor;
		if (timeBetweenWeatherChange >= MIN_TIME_BETWEEN_WEATHER_CHANGE) {
			if (Random.value > 0.5f) {
			}
		}


		// End of the day
		if (time >= 86400) {
			time = 0;
		}
	}

	/// <summary>
	/// Checks if it the day.
	/// </summary>
	/// <returns><c>true</c>, if day, <c>false</c> otherwise.</returns>
	public bool isDay () {
		return time > SUN_RISE * 3600 && time < SUN_SET * 3600;
	}

	/// <summary>
	/// Checks if it is raining.
	/// </summary>
	/// <returns><c>true</c>, if raining, <c>false</c> otherwise.</returns>
	public bool isRaining() {
		return false;
	}
}
