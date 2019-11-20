using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;
public class PhotonRoom : MonoBehaviour, IInRoomCallbacks, IMatchmakingCallbacks
{
	private bool isGameLoaded;
	private PhotonView pv;
	public static PhotonRoom Room;
	Player[] photonPlayers;
	private int playersInRoom;
	private int myNumberInRoom;
	public int maxPlayers = 2;
	// Start is called before the first frame update
	void Start()
    {
		if (PhotonRoom.Room == null)
		{
			PhotonRoom.Room = this;
		}
		else
		{
			if (PhotonRoom.Room != this)
			{
				Destroy(PhotonRoom.Room.gameObject);
				PhotonRoom.Room = this;
			}
		}
		DontDestroyOnLoad(this.gameObject);
		pv = GetComponent<PhotonView>();
	}


	public void OnEnable()
	{
		PhotonNetwork.AddCallbackTarget(this);
	}
	public void OnDisable()
	{
		PhotonNetwork.RemoveCallbackTarget(this);
	}

	public void OnJoinedRoom()
	{
		Debug.Log("ML : Photon Room -> Joined a room");
		photonPlayers = PhotonNetwork.PlayerList;
		playersInRoom = photonPlayers.Length;
		myNumberInRoom = playersInRoom;
		PhotonNetwork.NickName = myNumberInRoom.ToString();
		StartGame();
	}
	public void OnPlayerEnteredRoom(Player newPlayer)
	{
		// base.OnPlayerEnteredRoom(newPlayer);
		Debug.Log("New Player joiend room " + newPlayer.UserId);
		photonPlayers = PhotonNetwork.PlayerList;
		playersInRoom++;
		StartGame();
	}

	private void StartGame()
	{
		isGameLoaded = true;

		if (!PhotonNetwork.IsMasterClient)
		{
			PlayerType.ID = 2;
			Debug.Log("SETPLAYER ID " + 2);
			return;
		}

		if (maxPlayers == playersInRoom)
		{
			PlayerType.ID = 1;
			Debug.Log("SETPLAYER ID " + 1);
			PhotonNetwork.LoadLevel(1);
		}

	}

	public IEnumerator LoadNewLevel()
	{
		yield return new WaitForSeconds(1);
	
	}
	public void OnPlayerLeftRoom(Player otherPlayer)
	{
		Debug.Log("Player left room");
		//throw new NotImplementedException();
	}

	public void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
	{
		//throw new NotImplementedException();
	}

	public void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
	{
		//throw new NotImplementedException();
	}

	public void OnMasterClientSwitched(Player newMasterClient)
	{
		//throw new NotImplementedException();
	}

	public void OnFriendListUpdate(List<FriendInfo> friendList)
	{
		//throw new NotImplementedException();
	}

	public void OnCreatedRoom()
	{
		Debug.Log("[ML] : PhotonRoom : OnCreatedRoom");
	}

	public void OnCreateRoomFailed(short returnCode, string message)
	{
		Debug.Log("[ML] : PhotonRoom : OnCreateRoomFailed");
	}

	public void OnJoinRoomFailed(short returnCode, string message)
	{
		Debug.Log("[ML] : PhotonRoom : OnJoinRoomFailed");
	}

	public void OnJoinRandomFailed(short returnCode, string message)
	{
		Debug.Log("[ML] : PhotonRoom : OnJoinRandomFailed");
	}

	public void OnLeftRoom()
	{
		Debug.Log("[ML] : PhotonRoom : OnLeftRoom");
	}
}
