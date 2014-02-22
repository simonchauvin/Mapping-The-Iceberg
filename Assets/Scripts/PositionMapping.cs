using UnityEngine;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class PositionMapping : MonoBehaviour {

	public string positionFilePath;
	public float factor;
	private string data;
	private IPEndPoint ipep;
	private UdpClient client;
	private string[] positions;
	private string[] lastPosition;

	// Use this for initialization
	void Start () {
		ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 12345);
		client = new UdpClient(12345);
	}
	
	// Update is called once per frame
	void Update () {
		data = Encoding.UTF8.GetString(client.Receive(ref ipep));
		if (data.Length > 0) {
			positions = data.Split(";"[0]);
			lastPosition = positions[positions.Length - 2].Split("&"[0]);
			Vector3 pointToReach = new Vector3(float.Parse(lastPosition[0]) * factor, 1.0f, float.Parse(lastPosition[1]) * factor);
			float journeyLength = Vector3.Distance(transform.position, pointToReach);
			float fracJourney = 1.0f / journeyLength;
			transform.position = Vector3.Lerp(transform.position, pointToReach, fracJourney);
		}
	}

	void OnApplicationQuit() {
		client.Close();
	}
}
