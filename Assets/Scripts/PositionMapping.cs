using UnityEngine;
using System.Collections;
using System.IO;

public class PositionMapping : MonoBehaviour {

	public string positionFilePath;
	public float factor;
	private Vector2 currentPosition;
	StreamReader sr;
	string content;
	string[] positions;
	string[] lastPosition;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		sr = new StreamReader(Application.dataPath + "/" + positionFilePath);
		content = sr.ReadToEnd();
		sr.Close();
		if (content.Length > 0) {
			positions = content.Split(";"[0]);
			lastPosition = positions[positions.Length - 2].Split("&"[0]);
			Vector3 pointToReach = new Vector3(float.Parse(lastPosition[0]) * factor, 1.0f, float.Parse(lastPosition[1]) * factor);
			float journeyLength = Vector3.Distance(transform.position, pointToReach);
			float fracJourney = 1.0f / journeyLength;
			transform.position = Vector3.Lerp(transform.position, pointToReach, fracJourney);

		}
	}
}
