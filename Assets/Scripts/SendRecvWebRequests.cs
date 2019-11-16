using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class SendRecvWebRequests : MonoBehaviour
{
	readonly string getURL = "http://idcards2go.ngrok.io/all";
	readonly string postURL = "http://0cda2668.ngrok.io/whoswho/patientData";


	[Serializable]
	public class PatientData
	{
		public string Pid;
		public string Name;
		public string Age;
		public string BPD;
		public string BPS;
		public string Pulse;
		public string Observation;
		public string PhoneNumber;
		public string EmergencyContact;
		public string Address;
		public string Health;
	}
	public List<PatientData> cacheData = new List<PatientData>();
	public class RecvJson
	{
		public string name;
	}

	void Start()
	{

		/// A correct website page.
		//StartCoroutine(PostJson(postURL, patientData));

		StartCoroutine(GetRequest(getURL));
	}


	IEnumerator GetRequest(string uri)
	{
		while (true)
		{
			using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
			{
				// Request and wait for the desired page.
				webRequest.GetRequestHeader("application/json");
				webRequest.GetResponseHeader("application/json");
				yield return webRequest.SendWebRequest();

				string[] pages = uri.Split('/');
				int page = pages.Length - 1;

				if (webRequest.isNetworkError)
				{
					Debug.Log(pages[page] + ": Error: " + webRequest.error);
				}
				else
				{
					Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
					var data = webRequest.downloadHandler.text;

					cacheData = JsonConvert.DeserializeObject<List<PatientData>>(data);
					//Debug.Log(output.Name);
				}
			}
			yield return new WaitForSeconds(5);
			Debug.Log("List size is " + cacheData.Count);
		}
	}

	public static object ByteArrayToObject(byte[] arrBytes)
	{
		MemoryStream memStream = new MemoryStream();
		BinaryFormatter binForm = new BinaryFormatter();

		memStream.Write(arrBytes, 0, arrBytes.Length);
		memStream.Seek(0, SeekOrigin.Begin);

		object obj = (object)binForm.Deserialize(memStream);

		return obj;
	}

	public T FromByteArray<T>(byte[] data)
	{
		if (data == null)
			return default(T);
		BinaryFormatter bf = new BinaryFormatter();
		using (MemoryStream ms = new MemoryStream(data))
		{
			object obj = bf.Deserialize(ms);
			return (T)obj;
		}
	}

}
