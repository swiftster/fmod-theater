using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panner2d : MonoBehaviour {

	// Use this for initialization
	void Start () {
		var osc = GameObject.Find("/OSC").GetComponent<OSC>();
		var oscAdr = "/" + gameObject.name + "/pos";
//		Debug.Log("adr: " + oscAdr);
		osc.SetAddressHandler( oscAdr , UpdatePosFromOsc );
	}

	void UpdatePosFromOsc(OscMessage msg)
	{
		var x = msg.GetFloat(0);
		var z = msg.GetFloat(1);
		Debug.Log("pos: " + x + " , " + z);
		gameObject.transform.position = gameObject.transform.position.SetX(x).SetZ(z);
	}
}
