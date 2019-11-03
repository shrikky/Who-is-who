using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class SendRecvWebRequests : MonoBehaviour
{
	readonly string getURL = "http://b94c3ed9.ngrok.io/whoswho/tracker";
	readonly string postURL = "http://b94c3ed9.ngrok.io/whoswho/patientData";

	public class TestJson
	{
		public int age;
		public string name;
	}

	public class RecvJson
	{
		public string name;
	}

	void Start()
	{

		/// A correct website page.
		//StartCoroutine(PostJson(postURL, patientData));

		// A non-existing page.
		StartCoroutine(GetRequest(getURL));
	}

	private void Update()
	{

	}
	IEnumerator GetRequest(string uri)
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
				var output = JsonUtility.FromJson<RecvJson>(data);
				Debug.Log(output.name);
			}
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

	IEnumerator PostRequest(string uri)
	{
		int score = 40;
		List<IMultipartFormSection> data = new List<IMultipartFormSection>();
		data.Add(new MultipartFormDataSection("curScoreKey", score.ToString()));

		using (UnityWebRequest webRequest = UnityWebRequest.Post(uri, data))
		{
			// Request and wait for the desired page.
			//webRequest.SetRequestHeader("Content-Type", "application/json");
			yield return webRequest.SendWebRequest();

			if (webRequest.isNetworkError || webRequest.isHttpError)
			{
				Debug.Log(webRequest.error);
			}
			else
			{
				Debug.Log("Form upload complete!");
			}
		}
	}

	//IEnumerator PostJson(string uri, PatientData_1 jsonData)
	//{
	//	string data = JsonUtility.ToJson(jsonData);
	//	var byteData = Encoding.ASCII.GetBytes(data);
			

	//	using (UnityWebRequest webRequest = UnityWebRequest.Put(uri, byteData))
	//	{
	//		webRequest.method = UnityWebRequest.kHttpVerbPUT;
	//		webRequest.SetRequestHeader("Content-Type", "application/json");

	//		yield return webRequest.SendWebRequest();

	//		if (webRequest.isNetworkError || webRequest.isHttpError)
	//		{
	//			Debug.Log(webRequest.error);
	//		}
	//		else
	//		{
				
	//			Debug.Log(webRequest.responseCode);
	//			Debug.Log("Form upload complete!");
	//		}
	//	}
	//}

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
