using UnityEngine;
using UnityEngine.UI;

public class ColorPicker : MonoBehaviour
{

    private Dropdown _colorPicker;
    private GameObject _eventControllerUi;
    private FmodEvent _eventController;
    private string _objPath;


    private readonly Color[] _colors =
        {Color.red, Color.yellow, Color.green, Color.cyan, Color.blue, Color.magenta, Color.black};
    private Color _selectedColor = Color.red;
	
    void Start()
    {
        _colorPicker = gameObject.GetComponent<Dropdown>();

        var tools = GameObject.Find("/Tools").GetComponent<Tools>();		
        _eventControllerUi = tools.GetFmodEvent(gameObject, "EventController", out _objPath);
		
        if (_eventControllerUi)
            _eventController = _eventControllerUi.GetComponent<FmodEvent>();
        
        ChangeColor(PlayerPrefs.GetInt (_objPath));

    }
	
    public void ChangeColor(int index)
    {
        _selectedColor = _colors[index];
        _colorPicker.captionImage.color = _selectedColor;
        _colorPicker.value = index;
        _eventController.SoundEmitter.GetComponent<Renderer>().material.color = _selectedColor;
        PlayerPrefs.SetInt(_objPath, index);
    }
}