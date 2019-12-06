using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.XR.MagicLeap;
using System;

public class NetworkUISync : MonoBehaviour, IPunObservable
{

	private Vector3 _outgoingUserPos;
	private Vector3 _incomingUserPos;
	private object outgoingData = new object();
	private object incomingData = new object();
	public GameObject UI;
	public PhotonView pv;
	public GameObject closestPCFGameobject;
	void Start()
	{
		UI = GameObject.FindObjectOfType<VitalDisplayData>().gameObject;
		pv = this.GetComponent<PhotonView>();
	}
	public void SetUserPosition(Vector3 vec)
	{
		lock (outgoingData)
		{
			UI.transform.position = vec;
			//FindPlane.UpdatePosition(this.transform);
			_outgoingUserPos = closestPCFGameobject.transform.TransformPoint(vec);
		}
	}

	public void Update()
	{
		if (PhotonNetwork.IsMasterClient)
			return;

		lock (incomingData)
		{
			if(closestPCFGameobject != null)
			{
				UI.transform.position = closestPCFGameobject.transform.TransformPoint(_incomingUserPos);		
			}
		}
	}
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			lock (outgoingData)
			{
				stream.SendNext(SerializeVec(_outgoingUserPos));
			}
		}

		else
		{
			byte[] vecBytes = (byte[])stream.ReceiveNext();
			lock (incomingData)
			{
				_incomingUserPos = DeSerializeRFVec(vecBytes);
			}
		}
	}
	public byte[] SerializeVec(Vector3 vec)
	{
		byte[] buff = new byte[sizeof(float)* 3];  //3 * 4 * 3 = 36 bbytes

		Buffer.BlockCopy(BitConverter.GetBytes(vec.x), 0, buff, 0 * sizeof(float), sizeof(float)); // 0 * 4  3 * 4  6 * 4
		Buffer.BlockCopy(BitConverter.GetBytes(vec.y), 0, buff, 1 * sizeof(float), sizeof(float)); // 1 * 4  4 * 4  7 * 4
		Buffer.BlockCopy(BitConverter.GetBytes(vec.z), 0, buff, 2 * sizeof(float), sizeof(float)); // 2 * 4  5 * 4  8 * 4

		return buff;
	}
	public static Vector3 DeSerializeRFVec(byte[] data)
	{
		byte[] buff = data;
		int RFID = 0;
		Vector3 vect;
		vect.x = BitConverter.ToSingle(buff, 0 * sizeof(float));
		vect.y = BitConverter.ToSingle(buff, 1 * sizeof(float));
		vect.z = BitConverter.ToSingle(buff, 2 * sizeof(float));
		//Debug.Log(pairdata.Key + " " + pairdata.Value);
		return vect;
	}

}
