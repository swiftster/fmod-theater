using UnityEngine;
using UnityEngine.UI;

public class EventController : MonoBehaviour {

	public FmodEvent _fmodEvent;

	private Text distance;
	
	void Start()
	{
//		distance = gameObject.transform.Find("Distance").GetComponent<Text>();
	}

	private void OnGUI()
	{
//		distance.text = _fmodEvent.DistanceToListener.ToString();
	}

	public void ToggleFmodEvent(bool shouldPlay)
	{
//		_fmodEvent.ToggleFmodEvent(shouldPlay);
	}
	
	public void UpdateParameter(string param, float value)
	{
//		_fmodEvent.UpdateParameter(param, value);
//		Debug.Log("UpdateParam: " + param + " " + value);
	}

}
