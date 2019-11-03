using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

public class TrackSensorPosition : MonoBehaviour
{
	// Start is called before the first frame update
	
	public MLImageTrackerBehavior imageTracker;
	public GameObject RandomCube;
	void Start()
    {
		imageTracker.LongerDimensionInSceneUnits = 0.135f;
		imageTracker.OnTargetFound += OnImageTargetFound;
		imageTracker.OnTargetUpdated += OnTargetUpdated;
		// If not listed here, the PrivilegeRequester assumes the request for
		// the privileges needed, CameraCapture in this case, are in the editor.
		
	}

	private void OnImageTargetFound(bool obj)
	{
		Debug.Log("OnImageTargetFound" + obj);
		RandomCube.transform.position = this.transform.position;
		Debug.Log("OnImageTargetFound" + RandomCube.transform.position + "Cam pos " + Camera.main.transform.position);
	}

	private void OnTargetUpdated(MLImageTargetResult obj)
	{


	}

}
