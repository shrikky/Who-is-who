using System.Collections;
using System.Collections.Generic;
using UDP;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
public class UIPositionManager : MonoBehaviour
{
	public SocketConnection socketConnection;
	public Dictionary<int, GameObject> UIDataList = new Dictionary<int, GameObject>();

	public void SetUIData(KeyValuePair<int,Vector3>[] UIpos)
	{
		foreach (var data in UIpos)
		{
			if (UIDataList[data.Key] != null)
			{
				UIDataList[data.Key].transform.position = data.Value;
			}
		}
	}
}
