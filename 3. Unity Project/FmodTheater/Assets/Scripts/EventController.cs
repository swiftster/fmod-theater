using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class EventController : MonoBehaviour {

	public FmodEvent MyFmodEvent;
	private GameObject _listener;
	private FMOD.Studio.EVENT_CALLBACK _callback;
	
	private Text _distance;
	
	void Start()
	{
		_distance = gameObject.transform.Find("Distance").GetComponent<Text>();
		_listener = GameObject.FindGameObjectWithTag("Player");
	}

	private void OnGUI()
	{
		var distBtwnSrcNAud = Vector3.Distance(_listener.transform.position, MyFmodEvent.gameObject.transform.position);
		_distance.text = System.Math.Round(distBtwnSrcNAud, 2).ToString(CultureInfo.InvariantCulture);
	}

	public void ToggleFmodEvent(bool shouldPlay)
	{
		MyFmodEvent.gameObject.SetActive(shouldPlay);
	}
	
	public void UpdateParameter(string param, float value)
	{
//		Debug.Log("param; " + param + " : " + value);
		MyFmodEvent.SetParameter(param, value);
	}
	
	public void UpdateVolume(float volScale)
	{
		MyFmodEvent.UpdateVolume(volScale);
	}

}
