using System.Linq;
using FMODUnity;
using UnityEngine;

public class MusicTrigger : MonoBehaviour
{

	public string Beats = "";
	private string[] _beatsList;
	public string FmodEvent = "";
	
	// Use this for initialization
	void Start ()
	{
		var osc = GameObject.Find("/OSC").GetComponent<OSC>();
		var oscPosAdr = "/" + gameObject.name + "/pos";
		var oscToggleAdr = "/" + gameObject.name + "/toggle";
//		Debug.Log("adr: " + oscAdr);
		osc.SetAddressHandler(oscPosAdr, UpdatePosFromOsc);
		osc.SetAddressHandler(oscToggleAdr, ToggleFromOsc);
		
		 _beatsList = Beats.Split(',');
	}
	
	void UpdatePosFromOsc(OscMessage msg)
	{
		var x = msg.GetFloat(0);
		var z = msg.GetFloat(1);
//		Debug.Log("pos: " + x + " , " + z);
		gameObject.transform.position = gameObject.transform.position.SetX(x).SetZ(z);
	}

	public void ToggleFromOsc(OscMessage msg)
	{
		var shouldPlay = msg.GetFloat(0) > 0;
		ToggleTrigger(shouldPlay);
	}
	
	public void ToggleTrigger(bool shouldPlay)
	{
//        Debug.Log("shouldPlay : " + shouldPlay);
		gameObject.SetActive(shouldPlay);
	}

	public void TriggerEvent(string curBeat)
	{
		if (gameObject.activeInHierarchy && (_beatsList.Contains(curBeat) || Beats == ""))
		{
			RuntimeManager.PlayOneShotAttached(FmodEvent, gameObject);
			gameObject.SetActive(false);
		}
	}
}
