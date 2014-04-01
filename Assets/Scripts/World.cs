using UnityEngine;
using System.Collections;

public class World : MonoBehaviour {
	/// <summary>
	/// In hours.
	/// </summary>
	public int SUN_RISE;
	public int SUN_SET;
	public float STARTING_TIME;

	/// <summary>
	/// Speed at which the time is passing.
	/// A factor of 3600 means that a day lasts 24 seconds.
	/// </summary>
	public int passingTimeFactor;

	/// <summary>
	/// The current time in seconds.
	/// </summary>
	private float time;

	// Use this for initialization
	void Start () {
		time = STARTING_TIME * 3600;
	}
	
	// Update is called once per frame
	void Update () {
		// Time passing
		time += Time.deltaTime * passingTimeFactor;


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
}
