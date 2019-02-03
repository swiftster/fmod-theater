using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using UnityEngine;
using UnityEngine.UI;

public class Tools : MonoBehaviour
{

	public static GameObject FindParentWithTag(GameObject childObject, string ancestorTag)
	{
		var t = childObject.transform;
		while (t.parent != null)
		{
			if (t.parent.CompareTag(ancestorTag))
			{
				return t.parent.gameObject;
			}
			t = t.parent;
		}
		return null; // Could not find a parent with given tag.
	}

	public GameObject GetFmodEvent(GameObject childObject, string ancestorTag, out string oscAddress)
	{
		var t = childObject.transform;

		var hierarchy = new List<string>();

		oscAddress = "/dev/null";
		
		while (t.parent != null)
		{
			hierarchy.Insert(0, t.parent.name);
			if (t.parent.CompareTag(ancestorTag))
			{
				oscAddress = "/" + string.Join("/", hierarchy.ToArray()) + "/" + childObject.name;
				return t.parent.gameObject;
			}
			t = t.parent;
		}

		return null;
	}

	public void TurnOffAllPlayToggles()
	{
		var playToggles = GameObject.FindGameObjectsWithTag("PlayToggle");

		foreach (var toggle in playToggles)
		{
			toggle.GetComponent<UnityEngine.UI.Toggle>().isOn = false;
		}
	}

	void Update()
	{
		if (Input.GetKeyUp(KeyCode.Escape))
			TurnOffAllPlayToggles();
	}
	
}
