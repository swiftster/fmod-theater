using UnityEngine;

public class SpeakerCheck : MonoBehaviour {
	
	[FMODUnity.EventRef]
	public string FmodEventName;
	private FMOD.Studio.EventInstance _fmodEventInstance;
		
	private int[] spkPositions = { 0, -27, 34, 4, 4, -87, 93, -149, 152 }; // l r c lfe ls rs lsr rsr

	// Use this for initialization
	void Start () {
		_fmodEventInstance = FMODUnity.RuntimeManager.CreateInstance(FmodEventName);
	}

	public void TestSpeaker(int index)
	{
//		Debug.Log("index: " + index);
		switch (index)
		{
				case 0:
					_fmodEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
					break;
				case 4:
					_fmodEventInstance.setParameterValue("LFE", 1f);
					_fmodEventInstance.start();
					// set lfe param
					break;
				default:
					_fmodEventInstance.setParameterValue("LFE", 0f);
					_fmodEventInstance.setParameterValue("Position", spkPositions[index]);
					_fmodEventInstance.start();
					break;
		}
	}
}
