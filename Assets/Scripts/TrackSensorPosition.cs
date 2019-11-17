using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

public class TrackSensorPosition : MonoBehaviour
{
	// Start is called before the first frame update
	

	public GameObject TrackedUI;
	public int TrackerID;
	public SendRecvWebRequests webData;
	public VitalDisplayData display;
	void Start()
    {
		
	
		// If not listed here, the PrivilegeRequester assumes the request for
		// the privileges needed, CameraCapture in this case, are in the editor.

	}
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.A))
		{
			ProcessData();
		}
	}
	void ProcessData()
	{
		var data = webData.cacheData.Find(x => x.Pid == TrackerID.ToString());
		display.DisplayToUI(data.Name, data.BPD,data.BPS,data.Pulse,data.Observation);
	}
	private void OnImageTargetFound(bool obj)
	{
		if(obj)
		ProcessData();
	}

	private void OnTargetLost()
	{
		
	}
	private void OnTargetUpdated(MLImageTargetResult obj)
	{

	}

}
