using UnityEngine;
using UnityEngine.UI;

public class FMODController : MonoBehaviour {

	public Toggle GlobalMuteToggle;

	void Start()
	{

		var osc = GameObject.Find("/OSC").GetComponent<OSC>();
		osc.SetAddressHandler( "/panic" , UpdateValueFromOsc );
		var mutePref = PlayerPrefs.GetInt("Global Mute")==1;
		GlobalMuteToggle.isOn = mutePref;
	}

	void UpdateValueFromOsc(OscMessage oscMessage)
	{
		StopAllPlayerEvents ();
	}

	private void OnApplicationQuit()
	{
		StopAllPlayerEvents();
	}

	public void StopAllPlayerEvents()
	{
		GameObject[] playToggles = GameObject.FindGameObjectsWithTag("PlayToggle");
		foreach (var pToggle in playToggles)
		{
			pToggle.GetComponent<Toggle>().isOn = false;
		}
		FMOD.Studio.Bus masterBus = FMODUnity.RuntimeManager.GetBus("bus:/");
		masterBus.stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);
	}

	public void GlobalMute(bool isMuted)
	{
		FMODUnity.RuntimeManager.MuteAllEvents(isMuted);
		FMODUnity.RuntimeManager.PauseAllEvents(isMuted);
		FMOD.Studio.Bus masterBus = FMODUnity.RuntimeManager.GetBus("bus:/");
		masterBus.setMute(isMuted);
		PlayerPrefs.SetInt("Global Mute", isMuted ? 1 : 0);
	}



}


