using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using FMODUnity;
using UnityEngine;
using UnityEngine.UI;

public class FmodEvent : MonoBehaviour
{
    [EventRef] public string FmodEventName;
    public FMOD.Studio.EventInstance FmodEventInstance;
    public bool Preload = true;
    public bool OverrideAttenuation;
    public float OverrideMinDistance = -1.0f;
    public float OverrideMaxDistance = -1.0f;

    public GameObject SoundEmitter; // the object in unity that represents where the sound source is.
    
    private GameObject _listener;

    private FMOD.Studio.EventDescription _eventDescription;

    public FMOD.Studio.EventDescription EventDescription
    {
        get { return _eventDescription; }
    }

    private FMOD.Studio.EVENT_CALLBACK _callback;

    private Rigidbody _cachedRigidBody;
    private Animator _animator;
    public List<String> Triggers;

    private bool _isQuitting;

    private float _distBtwnSrcNAud;
    private Text _distanceReadout;
    private Toggle _playToggle;
    private Slider _volumeSlider;
    public MusicTrigger[] MusicTriggers;

    private Dictionary<string, ValueUpdate> _params = new Dictionary<string, ValueUpdate>();

    void Start()
    {
        _distanceReadout = gameObject.transform.Find("Distance").GetComponent<Text>();
        _playToggle = gameObject.transform.Find("Play").GetComponent<Toggle>();
        _listener = GameObject.FindGameObjectWithTag("Player");
        _animator = SoundEmitter.GetComponent<Animator>();
        
        var osc = GameObject.Find("/OSC").GetComponent<OSC>();
        var oscPosAdr = "/" + gameObject.name + "/pos";
//		Debug.Log("adr: " + oscAdr);
        osc.SetAddressHandler(oscPosAdr, UpdatePosFromOsc);

        if (_animator)
        {
            var oscTriggerAdr = "/" + gameObject.name + "/trigger";
            osc.SetAddressHandler(oscTriggerAdr, TriggerFromOsc);
        }

        _volumeSlider = gameObject.transform.Find("Volume").GetComponent<Slider>();
        foreach (Transform child in gameObject.transform)
            if (child.CompareTag("FmodParam"))
            {
                var valueUpdate = child.GetComponent<ValueUpdate>();
                if (valueUpdate)
                {
//                    Debug.Log("Adding Param: " + child.name + " : " + valueUpdate.SettingValue);
                    _params.Add(child.name, valueUpdate);
                }
            }

        RuntimeUtils.EnforceLibraryOrder();

        if (Preload)
        {
            _eventDescription = RuntimeManager.GetEventDescription(FmodEventName);
            _eventDescription.loadSampleData();
            RuntimeManager.StudioSystem.update();
            FMOD.Studio.LOADING_STATE loadingState;
            _eventDescription.getSampleLoadingState(out loadingState);
            while (loadingState == FMOD.Studio.LOADING_STATE.LOADING)
            {
#if WINDOWS_UWP
                    System.Threading.Tasks.Task.Delay(1).Wait();
#else
                System.Threading.Thread.Sleep(1);
#endif
                _eventDescription.getSampleLoadingState(out loadingState);
            }
        }
    }

    private void OnGUI()
    {
        var distBtwnSrcNAud = Vector3.Distance(_listener.transform.position, SoundEmitter.transform.position);
        _distanceReadout.text = Math.Round(distBtwnSrcNAud, 2).ToString(CultureInfo.InvariantCulture);
    }

    void OnApplicationQuit()
    {
        _isQuitting = true;
    }

    void OnDestroy()
    {
        if (!_isQuitting)
        {
            if (FmodEventInstance.isValid())
            {
                FmodEventInstance.release();
                RuntimeManager.DetachInstanceFromGameObject(FmodEventInstance);
            }

            if (Preload)
            {
                _eventDescription.unloadSampleData();
            }
        }
    }

    private void Play()
    {
        if (String.IsNullOrEmpty(FmodEventName))
        {
            return;
        }

        if (!_eventDescription.isValid())
        {
            _eventDescription = RuntimeManager.GetEventDescription(FmodEventName);
        }

        bool isOneshot = false;
        if (!FmodEventName.StartsWith("snapshot", StringComparison.CurrentCultureIgnoreCase))
        {
            _eventDescription.isOneshot(out isOneshot);
        }

        bool is3D;
        _eventDescription.is3D(out is3D);

        if (!FmodEventInstance.isValid())
        {
            FmodEventInstance.clearHandle();
        }

        // Let previous oneshot instances play out
        if (isOneshot && FmodEventInstance.isValid())
        {
            FmodEventInstance.release();
            FmodEventInstance.clearHandle();
        }

        if (!FmodEventInstance.isValid())
        {
            FmodEventInstance = RuntimeManager.CreateInstance(FmodEventName);

            _callback = MyEventCallback;
            FmodEventInstance.setCallback(_callback);

            // Only want to update if we need to set 3D attributes
            if (is3D)
            {
                var rigidBody = SoundEmitter.GetComponent<Rigidbody>();
                var rigidBody2D = SoundEmitter.GetComponent<Rigidbody2D>();
                var emitterTransform = SoundEmitter.GetComponent<Transform>();
                if (rigidBody)
                {
                    FmodEventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(SoundEmitter, rigidBody));
                    RuntimeManager.AttachInstanceToGameObject(FmodEventInstance, emitterTransform, rigidBody);
                }
                else
                {
                    FmodEventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(SoundEmitter, rigidBody2D));
                    RuntimeManager.AttachInstanceToGameObject(FmodEventInstance, emitterTransform, rigidBody2D);
                }
            }
        }

        if (is3D && OverrideAttenuation)
        {
            FmodEventInstance.setProperty(FMOD.Studio.EVENT_PROPERTY.MINIMUM_DISTANCE, OverrideMinDistance);
            FmodEventInstance.setProperty(FMOD.Studio.EVENT_PROPERTY.MAXIMUM_DISTANCE, OverrideMaxDistance);
        }
        
        if (_volumeSlider)
            UpdateVolume(_volumeSlider.value);
        if (!_volumeSlider)
        {
            Debug.Log("No Vol Slider for " + gameObject.name);
        }
        foreach (var param in _params)
        {
            SetParameter(param.Key, param.Value.SettingValue);
        }

        FmodEventInstance.start();
    }

    private void Stop()
    {
        if (FmodEventInstance.isValid())
        {
            FmodEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            FmodEventInstance.release();
            FmodEventInstance.clearHandle();
        }
    }

    public void SetParameter(string param, float value)
    {
//        Debug.Log("param; " + param + " : " + value);
        if (FmodEventInstance.isValid())
        {
            FmodEventInstance.setParameterValue(param, value);
//            Debug.Log("param set; " + param + " : " + value);
        }
    }

    public void UpdateVolume(float volScale)
    {
        if (FmodEventInstance.isValid())
        {
            FmodEventInstance.setVolume(volScale);
        }
    }

    public bool IsPlaying()
    {
        if (FmodEventInstance.isValid() && FmodEventInstance.isValid())
        {
            FMOD.Studio.PLAYBACK_STATE playbackState;
            FmodEventInstance.getPlaybackState(out playbackState);
            return (playbackState != FMOD.Studio.PLAYBACK_STATE.STOPPED);
        }

        return false;
    }

    public void ToggleFmodEvent(bool shouldPlay)
    {
//        Debug.Log("shouldPlay : " + shouldPlay);
        SoundEmitter.SetActive(shouldPlay);
        if (shouldPlay) { Play(); } else { Stop(); }
    }

    void TriggerFromOsc(OscMessage message)
    {
        var triggerName = message.values[0].ToString();
//        Debug.Log("trigger :" + triggerName + "!");
        if (_animator)
            _animator.SetTrigger(triggerName);
    }

    void UpdatePosFromOsc(OscMessage msg)
    {
        var x = msg.GetFloat(0);
        var z = msg.GetFloat(1);
 //       Debug.Log("pos: " + x + " , " + z);
        SoundEmitter.transform.position = SoundEmitter.transform.position.SetX(x).SetZ(z);
    }

    private FMOD.RESULT MyEventCallback(FMOD.Studio.EVENT_CALLBACK_TYPE type,
        FMOD.Studio.EventInstance instance, IntPtr paramPtr)
    {
        if (type == FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER)
        {
            var marker = (FMOD.Studio.TIMELINE_MARKER_PROPERTIES) Marshal.PtrToStructure(paramPtr,
                typeof(FMOD.Studio.TIMELINE_MARKER_PROPERTIES));

            string mrkName = marker.name;
            if (mrkName == "EOF")
            {
//                Debug.Log("EOF Reached!");
                _playToggle.isOn = false;
                if (MusicTriggers.Length > 0)
                {
                    foreach (var mTrigger in MusicTriggers)
                    {
                        mTrigger.ToggleTrigger(false);
                    }
                }
            }
        } else if ( MusicTriggers.Length > 0 && type == FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT)
        {
            var beatProps = (FMOD.Studio.TIMELINE_BEAT_PROPERTIES) Marshal.PtrToStructure(paramPtr, 
                typeof(FMOD.Studio.TIMELINE_BEAT_PROPERTIES));

            foreach (var mTrigger in MusicTriggers)
            {
               mTrigger.TriggerEvent(beatProps.beat.ToString()); 
            }            
        }


        return FMOD.RESULT.OK;
    }
}