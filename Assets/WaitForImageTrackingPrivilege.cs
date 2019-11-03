using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

public class WaitForImageTrackingPrivilege : MonoBehaviour
{
	private PrivilegeRequester _privilegeRequester;
	public TrackSensorPosition trackerSensorPos;
	// Start is called before the first frame update
	void Start()
    {
		_privilegeRequester = GetComponent<PrivilegeRequester>();

		// Before enabling the MLImageTrackerBehavior GameObjects, the scene must wait until the privilege has been granted.
		_privilegeRequester.OnPrivilegesDone += HandlePrivilegesDone;
	}


	private void HandlePrivilegesDone(MLResult result)
	{
		if (!result.IsOk)
		{
			if (result.Code == MLResultCode.PrivilegeDenied)
			{
				Instantiate(Resources.Load("PrivilegeDeniedError"));
			}

			Debug.LogErrorFormat("Error: ImageTrackingExample failed to get requested privileges, disabling script. Reason: {0}", result);

			return;
		}
		trackerSensorPos.gameObject.SetActive(true);
		Debug.Log("Succeeded in requesting all privileges");
	}
}
