using UnityEngine;
using System.Globalization;

public class ValueUpdate : MonoBehaviour {

	private UnityEngine.UI.Slider _mySlider;
	private UnityEngine.UI.InputField _myField;
	private UnityEngine.UI.Toggle _myToggle;
	private string _objPath;
	private GameObject _eventControllerUi;
	private FmodEvent _eventController;
	
	private float _settingValue;
	public  float SettingValue { get { return _settingValue; } }

	void Start()
	{
		var osc = GameObject.Find("/OSC").GetComponent<OSC>();
		
		var tools = GameObject.Find("/Tools").GetComponent<Tools>();
		_eventControllerUi = tools.GetFmodEvent(gameObject, "EventController", out _objPath);
//		Debug.Log("adr: " + _objPath);

		osc.SetAddressHandler( _objPath , UpdateValueFromOsc );
		
		if ( gameObject.CompareTag("FmodParam") && _eventControllerUi)
			_eventController = _eventControllerUi.GetComponent<FmodEvent>();
		
		_mySlider = gameObject.GetComponent<UnityEngine.UI.Slider>();
		_myField = gameObject.GetComponent<UnityEngine.UI.InputField>();
		_myToggle = gameObject.GetComponent<UnityEngine.UI.Toggle>();

		_settingValue = PlayerPrefs.GetFloat (_objPath);
//		Debug.Log ("setting " + _objPath + ": " + _settingValue);
		UpdateValueFromFloat(_settingValue);		
	}

	public void UpdateValueFromFloat(float value) {
//		Debug.Log(name + " float value changed: " + value);
		if (_mySlider) 
			_mySlider.value = value;
		if (_myField) 
			_myField.text = value.ToString(CultureInfo.InvariantCulture);
		if (_myToggle)
			_myToggle.isOn = value > 0;
		if (_eventController)
		{
//			Debug.Log("update param " + name + " : " + value);
			_eventController.SetParameter(gameObject.name, value);
		}
		
		PlayerPrefs.SetFloat(_objPath, value);
	}

	public void UpdateValueFromString(string value) {
//		Debug.Log(name + " string value changed: " + value);
		if (float.TryParse(value, out _settingValue))
		{
			UpdateValueFromFloat(_settingValue);
		}
	}

//	public void UpdateValueFromBool(bool value)
//	{
//		_settingValue = value ? 1f : 0f;
//		UpdateValueFromFloat(_settingValue);	
//	}
	
	public void StoreBoolValueAsFloat(bool value)
	{
		_settingValue = value ? 1f : 0f;
//		Debug.Log(value + "bool: " + _settingValue);
		if (_eventController)
		{
//			Debug.Log("update param " + name + " : " + value);
			_eventController.SetParameter(gameObject.name, _settingValue);
		}
		PlayerPrefs.SetFloat(_objPath, _settingValue);
	}

	void UpdateValueFromOsc(OscMessage oscMessage)
	{
		UpdateValueFromFloat(oscMessage.GetFloat(0));
	}

}