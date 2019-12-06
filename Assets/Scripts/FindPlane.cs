using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindPlane : MonoBehaviour
{
	public GameObject origin;
	public GameObject cube1;
	public GameObject cube2;
	public GameObject cube3;
	// Start is called before the first frame update
	public static void UpdatePosition(Transform transform)
    {
		//Vector3 position = transform.position;
		//Vector3 scale = Vector3.one;
		//Matrix4x4 mat = Matrix4x4.TRS(position, Quaternion.identity, scale);

		//var newpos = mat.MultiplyPoint3x4(transform.position);
		//GameObject Axis = new GameObject();
		//Axis.transform.position = transform.position;
	}

    // Update is called once per frame
    void Update()
    {
        
    } 
}
