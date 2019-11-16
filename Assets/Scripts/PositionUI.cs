using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionUI : MonoBehaviour
{
	// Start is called before the first frame update
	public GameObject cube;
	public Vector3 trackerPosition;
	public GameObject UICanvas;
    void Start()
    {
		cube.transform.position = trackerPosition;
		UICanvas.transform.position = trackerPosition + new Vector3(0, 0.3f, 0);

	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
