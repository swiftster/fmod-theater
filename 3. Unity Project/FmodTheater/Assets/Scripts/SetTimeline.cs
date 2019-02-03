using System;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.UI;

public class SetTimeline : MonoBehaviour
{

	private Dictionary<String, EventInstance> fmodEvents = new Dictionary<string, EventInstance>();
	private Dropdown _dropdown;
	
	// Use this for initialization
	void Start ()
	{
		_dropdown = gameObject.GetComponent<Dropdown>();
		RefreshEventList();
	}

	public void RefreshEventList(bool refresh=true)
	{
		_dropdown.options.Clear();
		GameObject[] fmodObjects = GameObject.FindGameObjectsWithTag("EventController");
		foreach (var fmodEvent in fmodObjects)
		{
			var eventName = fmodEvent.name;
//			Debug.Log("name : " + eventName);
			EventInstance eventInstance = fmodEvent.GetComponent<FmodEvent>().FmodEventInstance;
			fmodEvents.Add(eventName, eventInstance);
			_dropdown.options.Add(new Dropdown.OptionData(eventName));
		}
	}

	public void SetTime(string value)
	{
		var curEventName = _dropdown.options[_dropdown.value].text;
		var curEvent = fmodEvents[curEventName];
		int seconds;
		
		if (int.TryParse(value, out seconds))
		{
			curEvent.setTimelinePosition(seconds * 1000);
		}
		
	}

	public void Pause(bool value)
	{
		var curEventName = _dropdown.options[_dropdown.value].text;
		var curEvent = fmodEvents[curEventName];
		Debug.Log("valid " + curEvent.isValid());
		curEvent.setPaused(value);
	}

}
