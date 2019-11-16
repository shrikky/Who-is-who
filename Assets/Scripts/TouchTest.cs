using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TouchTest : MonoBehaviour
{
	public TextMeshProUGUI textMesh;
	private void OnCollisionEnter(Collision collision)
	{
		textMesh.text = "Can't touch me";
	}

}
