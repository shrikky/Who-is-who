using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;

public class PhotonLobby : MonoBehaviourPunCallbacks
{
	public void Start()
	{
		PhotonNetwork.ConnectUsingSettings();
	}

	public override void OnConnectedToMaster()
	{
		Debug.Log("ML: Player has connected to the Photon master server");
		PhotonNetwork.AutomaticallySyncScene = true;
		StartCoroutine(ConnectToGame());
	}

	public IEnumerator ConnectToGame()
	{
		yield return new WaitForSeconds(2);
		Debug.Log(PhotonNetwork.CountOfRooms);
		JoinRoom();
	}

	public void JoinRoom()
	{
		Debug.Log("ML: Join Room");
		PhotonNetwork.JoinRandomRoom();
	}

	public override void OnJoinRandomFailed(short returnCode, string message)
	{
		Debug.Log("ML: Tried to join a random game  but failed");
		CreateRoom();
	}

	private void CreateRoom()
	{
		int randomRoomName = UnityEngine.Random.Range(0, 10000);
		Debug.Log("ML: Trying To create room with ID: " + randomRoomName);
		RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = Convert.ToByte(8) };
		PhotonNetwork.CreateRoom("Room " + randomRoomName, roomOps);
	}

	public override void OnCreatedRoom()
	{
		Debug.Log("ML: Created Room ");
	}

	public override void OnCreateRoomFailed(short returnCode, string message)
	{
		Debug.Log("ML: Tried to create a room but failed, there must be a room with same name");
		CreateRoom();
	}

	public void LeaveRoom()
	{
		Debug.Log("ML: Leaving Room");
		PhotonNetwork.LeaveRoom();
	}

	public override void OnJoinedRoom()
	{
		base.OnJoinedRoom();
		Debug.Log("ML: PhotonLobby-> On joined room");
	}

	public override void OnJoinedLobby()
	{
		Debug.Log("ML: OnJoinedLobby");
		base.OnJoinedLobby();
	}

}
