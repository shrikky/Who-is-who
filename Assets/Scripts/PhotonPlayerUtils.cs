using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using MagicLeapInternal;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using CFUID = MagicLeapInternal.MagicLeapNativeBindings.MLCoordinateFrameUID;

public static class PhotonCustomDataSerialization
{
	public static byte BYTE_CFUID = (byte)'A';
}
public abstract class PhotonPlayerUtils : MonoBehaviourPun, IPunObservable
{
	protected PhotonView pv;

	protected virtual void Start()
	{
		pv = GetComponent<PhotonView>();
		RegisterCustomTypes();
	}

	public void RegisterCustomTypes()
	{
		PhotonPeer.RegisterType(typeof(CFUID), PhotonCustomDataSerialization.BYTE_CFUID, PhotonPlayerUtils.CFUIDSerialize, PhotonPlayerUtils.CFUIDDeserialize); // Need to create class for serialization
		Debug.Log("Serialized CFUID");
	}

	static object CFUIDDeserialize(Byte[] data)
	{
		if (data == null)
			return default(CFUID);

		BinaryFormatter bf = new BinaryFormatter();
		using (MemoryStream ms = new MemoryStream(data))
		{
			object obj = bf.Deserialize(ms);
			return (CFUID)obj;
		}
	}

	static byte[] CFUIDSerialize(object obj)
	{
		if (obj == null)
			return null;
		BinaryFormatter bf = new BinaryFormatter();
		using (MemoryStream ms = new MemoryStream())
		{
			bf.Serialize(ms, obj);
			return ms.ToArray();
		}
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{

	}
}
