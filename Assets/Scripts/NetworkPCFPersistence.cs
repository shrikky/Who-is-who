using MagicLeap;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;
using CFUID = MagicLeapInternal.MagicLeapNativeBindings.MLCoordinateFrameUID;

public class NetworkPCFPersistence : MonoBehaviour, IPunObservable
{
	IEnumerator _findAllPCFs = null;
	public GameObject _PCFVizPrefab;
	private bool isDebugOn;
	private List<MLPCF> PCFList = new List<MLPCF>();
	private List<GameObject> _pcfObjs = new List<GameObject>();
	private int _ongoingQueriesCount;
	private PrivilegeRequester _privilegeRequester;
	protected PhotonView pv;
	public delegate void ObtainPCFsCallback();
	public static event ObtainPCFsCallback OnPCFsObtained;
	public CFUID closestPCFCFUID;
	public MLPCF closestPCF;
	private GameObject closestPCFGameObject;

	public static bool IsDebugMode
	{
		get; private set;
	}
	private void Awake()
	{
		RequestPrivs();
	}
	private void Start()
	{
		pv = GetComponent<PhotonView>();
		_findAllPCFs = FindAllPCFs();
		if (_PCFVizPrefab == null)
		{
			Debug.LogError("PCFVisualizer Prefab is not assigned");
			return;
		}

		MLResult result = MLPersistentStore.Start();
		if (!result.IsOk)
		{
			Debug.LogErrorFormat("Error: PCFVisualizer failed starting MLPersistentStore, disabling script. Reason: {0}", result);
			enabled = false;
			return;
		}
		else
			Debug.Log("Persistent store started");

		result = MLPersistentCoordinateFrames.Start();
		if (!result.IsOk)
		{
			Debug.LogErrorFormat("MLPersistentCoordinateFrames failed to start. Reason {0}", result);
			return;
		}
		else
			Debug.Log("MLPersistentCoordinateFrames started");

		MLPCF.OnCreate += OnNewPCFCreated;
		StartCoroutine(DelayedPCFRetreival());
		if(MLInput.IsStarted ==  false)
		{
			MLInput.Start();
		}
		MLInput.OnControllerButtonDown += HandleControllerButtonDown;
	}

	private void HandleControllerButtonDown(byte controllerId, MLInputControllerButton button)
	{
		Debug.Log("Handle Controller button");
		if (button == MLInputControllerButton.HomeTap)
		{
			if (pv.IsMine)
			{
				Debug.Log("Sending RPC to toggle PCF");
				pv.RPC("OnTogglePCFRecvd", RpcTarget.All);
			}
		}
		if (button == MLInputControllerButton.Bumper)
		{
			if (pv.IsMine)
				FindNearestPCF();
		}
	}

	public IEnumerator DelayedPCFRetreival()
	{
		yield return new WaitForSeconds(1.0f);
		StartCoroutine(FindAllPCFs());
	}

	private void OnNewPCFCreated(MLPCF pcf)
	{
		Debug.Log("CFUID " + pcf.CFUID + "CFUID.ToString()" + pcf.CFUID.ToString());
		AddPCFObject(pcf);
	}

	private void AddPCFObject(MLPCF pcf)
	{
		GameObject repObj = Instantiate(_PCFVizPrefab, Vector3.zero, Quaternion.identity);
		repObj.name = pcf.CFUID.ToString();
		repObj.transform.position = pcf.Position;
		repObj.transform.rotation = pcf.Orientation;
		PCFStatusText statusTextBehavior = repObj.GetComponent<PCFStatusText>();
		if (statusTextBehavior != null)
		{
			statusTextBehavior.PCF = pcf;
		}
		repObj.SetActive(IsDebugMode);
		_pcfObjs.Add(repObj);
	}

	public void FindNearestPCF()
	{
		Debug.Log("Finding nearest pcf");
		MLPersistentCoordinateFrames.FindClosestPCF(Camera.main.transform.position, OnFoundNearestPCF);
	}

	private void OnFoundNearestPCF(MLResult arg1, MLPCF pcf)
	{
		Debug.Log("ML : Calling SpawnPCFVisual with PCF CFUID " + pcf.CFUID);
		pv.RPC("SpawnPCFVisual", RpcTarget.All, pcf.CFUID);

	}

	[PunRPC]
	private void OnTogglePCFRecvd()
	{
		Debug.Log("Recvd event");
		isDebugOn = !isDebugOn;
		Debug.Log("ML: Pressed home button ,DebugMode " + isDebugOn);
		EnableDebugPCFViz(isDebugOn);
	}

	[PunRPC]
	private void SpawnPCFVisual(CFUID cfuid)
	{
		Debug.Log("Spawning Visual at nearest PCF " + cfuid);
		var pcf = PCFList.Find(x => x.CFUID == cfuid);
		if (pcf != null)
			Debug.Log(pcf.Position);

		PhotonNetwork.Instantiate("PCFVisual", pcf.Position, pcf.Orientation);
	}
	private void HandlePCFPositionQuery(MLResult result, MLPCF pcf)
	{
		//if (result.IsOk)
		//{
		//    // This is only for demonstration purposes because we want to track all the PCFs found.
		//    // Ideally in a production app, we only wish to track PCFs that have virtual content
		//    // bound to them - which is already automatically done by MLPersistentBehavior.
		//    MLPersistentCoordinateFrames.QueueForUpdates(pcf);
		//}
		--_ongoingQueriesCount;
	}
	private IEnumerator FindAllPCFs()
	{
		float timer = 5;
		while(timer > 0)
		{
			// GetAllPCFs returns a list of PCFs that doesn't have a position yet
			MLResult result = MLPersistentCoordinateFrames.GetAllPCFs(out PCFList);
			if(!result.IsOk)
			{
				Debug.LogErrorFormat("Error : MLPersistenceCoordinateFrames failed to get all PCFS. Reason {0}", result);
				yield break;
			}

			foreach(var pcf in PCFList)
			{
				result = MLPersistentCoordinateFrames.GetPCFPose(pcf, HandlePCFPositionQuery);
				// HandlePCFPositionQuery could execute immediately (when the requested PCF has been requested before)
				// or later (when the PCF is completely new).
				if (!result.IsOk)
				{
					Debug.LogErrorFormat("Error: MLPersistentCoordinateFrames failed to get PCF position. Reason: {0}", result);
					yield break;
				}
			}
			timer -= Time.deltaTime;
		}
		Debug.Log("OnGoingQUeriesCount " + _ongoingQueriesCount);
		Debug.Log("Exiting Coroutine");
		if (OnPCFsObtained != null)
			OnPCFsObtained();

		AssignClosestPCF();
	}

	private void AssignClosestPCF()
	{
		if(PCFList.Count <=0)
		{
			Debug.Log("No pcfs in the list");
		}
		closestPCF = PCFList.Find(x => x.CFUID == closestPCFCFUID);
		MLPersistentCoordinateFrames.GetPCFPose(closestPCF, AssignClosestPCF);
	}
	private void AssignClosestPCF(MLResult result, MLPCF pcf)
	{
		if (result.IsOk)
		{
			MLPersistentCoordinateFrames.QueueForUpdates(pcf);
			closestPCF = pcf;
			closestPCFGameObject = new GameObject();
			closestPCFGameObject.transform.position = closestPCF.Position;
			closestPCFGameObject.transform.rotation = closestPCF.Orientation;
			Debug.Log("Found closest PCF " + closestPCF.Position + " " + closestPCF.CFUID);

			if (PhotonNetwork.IsMasterClient)
				pv.RPC("SyncClosestPCF", RpcTarget.OthersBuffered, closestPCF.CFUID);
		}
	}

	private void OnDestroy()
	{
		if (_findAllPCFs != null)
			StopCoroutine(_findAllPCFs);
		foreach (GameObject go in _pcfObjs)
		{
			if (go != null)
			{
				Destroy(go);
			}
		}

		MLPCF.OnCreate -= OnNewPCFCreated;
		if (MLPersistentStore.IsStarted)
		{
			MLPersistentStore.Stop();
		}
		if (MLPersistentCoordinateFrames.IsStarted)
		{
			MLPersistentCoordinateFrames.Stop();
		}

		if (_privilegeRequester != null)
		{
			_privilegeRequester.OnPrivilegesDone -= HandlePrivilegesDone;
		}
	}

	void RequestPrivs()
	{
		_privilegeRequester = GetComponent<PrivilegeRequester>();
		if (_privilegeRequester == null)
		{
			// Handle if the Privilege Requester is missing from the scene
		}
		// Could have also been set via the editor. 
		_privilegeRequester.Privileges = new[]
		 {
			  MLRuntimeRequestPrivilegeId.LocalAreaNetwork,
			  MLRuntimeRequestPrivilegeId.CoarseLocation
			 };
		_privilegeRequester.OnPrivilegesDone += HandlePrivilegesDone;
	}
	private void HandlePrivilegesDone(MLResult result)
	{
		if (!result.IsOk)
		{
			Debug.Log("some privs were not given");
		}
		// Privileges granted
	}

	public void EnableDebugPCFViz(bool turnOn)
	{
		IsDebugMode = turnOn;
		Debug.Log("IsDebugMode On? " + turnOn);

		Debug.Log("Reached Enable Debug PCF viz" + turnOn);
		foreach (GameObject pcfGO in _pcfObjs)
		{
			pcfGO.SetActive(turnOn);
		}

		if (turnOn)
		{
			Debug.Log("Starting coroutine");
			StartCoroutine(_findAllPCFs);
		}
		else
		{
			Debug.Log("Stopping coroutine");
			StopCoroutine(_findAllPCFs);
		}
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{

	}
}
