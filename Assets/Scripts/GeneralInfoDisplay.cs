using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GeneralInfoDisplay : MonoBehaviour
{
	public TextMeshProUGUI PatientName;
	public TextMeshProUGUI BPD;
	public TextMeshProUGUI BPS;
	public TextMeshProUGUI Pulse;
	public TextMeshProUGUI Observation;


	public void DisplayToUI(string patientName = null, string bpd = null, string bps = null, string pulse = null, string obs = null)
	{
		PatientName.text = patientName;
		BPD.text = bpd;
		BPS.text = bps;
		Pulse.text = pulse;
		Observation.text = obs;
	}
}
