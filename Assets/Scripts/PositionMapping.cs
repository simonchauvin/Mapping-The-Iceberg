using UnityEngine;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

public class PositionMapping : MonoBehaviour {

	public float direction;
	public string ip;
	public string port;
	
	public float factor;
	private string data;
	private string dataPhone;
	private IPEndPoint ipep;
	private IPEndPoint ipepPhone;
	private UdpClient client;
	private UdpClient clientPhone;
	private string[] dataArray;
	private string[] positions;
	private string[] lastPosition;

	// Use this for initialization
	void Start () {
		ipep = new IPEndPoint(IPAddress.Any, 12345);
		ipepPhone = new IPEndPoint(IPAddress.Any, 11999);
		client = new UdpClient();
		client.Client.SetSocketOption(
			SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
		client.Client.Bind(ipep);
		clientPhone = new UdpClient();
		clientPhone.Client.SetSocketOption(
			SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
		clientPhone.Client.Bind(ipepPhone);
	}

	// Update is called once per frame
	void Update () {
		// Retrieve data from openframeworks
		if (client.Available > 0)
		{
			data = Encoding.UTF8.GetString(client.Receive(ref ipep));
			if (data != null && data.Length > 0) {
				dataArray = Regex.Split(data, "&");
				float x = float.Parse(dataArray[0]);
				float y = float.Parse(dataArray[1]);
				Vector3 pointToReach = new Vector3(x * factor, 1.0f, y * factor);
				float journeyLength = Vector3.Distance(transform.position, pointToReach);
				float fracJourney = 1.0f / journeyLength;
				// Set y position
				transform.position = Vector3.Lerp(transform.position, pointToReach, fracJourney);

			}
		}
		// Retrieve data from the phone
		if (clientPhone.Available > 0)
		{
			dataPhone = Encoding.UTF8.GetString(clientPhone.Receive(ref ipepPhone));
			if (dataPhone != null && dataPhone.Length > 0) {
				dataArray = Regex.Split(dataPhone, "direction:");
				float direction = float.Parse(Regex.Split(dataArray[1], "accelerometer:")[0]);
				float accelerometer = float.Parse(Regex.Split(dataArray[1], "accelerometer:")[1]);
				transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, -direction, transform.localEulerAngles.z);
				transform.localEulerAngles = new Vector3(-accelerometer * 9, transform.localEulerAngles.y, transform.localEulerAngles.z);
			}
		}
	}

	void OnApplicationQuit() {
		client.Close();
		clientPhone.Close();
	}
}
