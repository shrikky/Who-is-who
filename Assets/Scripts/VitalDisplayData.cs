using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VitalDisplayData : MonoBehaviour
{
	public TextMeshProUGUI PatientName;
	public TextMeshProUGUI BP;
	public TextMeshProUGUI Location;
	public TextMeshProUGUI Pulse;
	public TextMeshProUGUI Observation;


	public void DisplayToUI(string patientName = null, string bp = null, string location = null, string pulse = null, string obs = null)
	{
		PatientName.text = patientName;
		BP.text = bp;
		Location.text = location;
		Pulse.text = pulse;
		Observation.text = obs;
	}
}
