using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//--------------------------------------------------------------------
//
// This is a Unity behaviour script that demonstrates how to use
// FMOD Studio Events in your game code. This can be more effective
// than using the provided FMOD Studio components only, and will 
// allow access to the full capabilities of FMOD Studio.
//
//   Basic Unity Integration usage contents:
//
//  1: Allowing designers to select Events in the Unity Inspector
//  2: Using Instances to allow management of events over their lifetime
//  3: One shot sounds
//  4: Creating Event Instances and starting them
//  5: Attaching an Event Instance to a GameObject
//  6: Immediate stop of Instance's and releasing resources
//  7: Releasing resources.
//  8: Starting and restarting Instances
//  9: Manually updating instances to a game objects position
// 10: Updating parameters every frame
// 11: Playing one shots
// 12: Stopping events with a fade out
// 13: Playing one shots with parameters
// 14: Stopping all events on a bus
//
// This document assumes familiarity with Unity scripting. See
// https://unity3d.com/learn/tutorials/topics/scripting for resources
// on learning Unity scripting. 
//
//--------------------------------------------------------------------

class FmodExamples : MonoBehaviour
{
	//--------------------------------------------------------------------
	// 1: Using the EventRef attribute will present the designer with
	//    the UI for selecting events.
	//--------------------------------------------------------------------
	[FMODUnity.EventRef]
	public string PlayerStateEvent;

	//--------------------------------------------------------------------
	// 2: Using the EventInstance class will allow us to manage an event
	//    over it's lifetime. Including starting, stopping and changing 
	//    parameters.
	//--------------------------------------------------------------------
	FMOD.Studio.EventInstance playerState;


	//--------------------------------------------------------------------
	// 3: These two events represent one shot sounds. They are sounds that 
	//    have a finite length. We do not store an EventInstance to
	//    manage the sounds. Once started they will play to completion and
	//    then all resources will be released.
	//--------------------------------------------------------------------
	[FMODUnity.EventRef]
	public string DamageEvent;
	[FMODUnity.EventRef]
	public string HealEvent;


	//--------------------------------------------------------------------
	//    This event is also one shot, but we want to track it's state and
	//    take action when it ends. We could also change parameter
	//    values over the lifetime.       
	//--------------------------------------------------------------------
	[FMODUnity.EventRef]
	public string PlayerIntroEvent;
	FMOD.Studio.EventInstance playerIntro;


	public int StartingHealth = 100;
	int health;

	Rigidbody cachedRigidBody;

	void Start()
	{
		cachedRigidBody = GetComponent<Rigidbody>();
		health = StartingHealth;

		//--------------------------------------------------------------------
		// 4: This shows how to create an instance of an Event and manually 
		//    start it.
		//--------------------------------------------------------------------
		playerState = FMODUnity.RuntimeManager.CreateInstance(PlayerStateEvent);
		playerState.start();

		playerIntro = FMODUnity.RuntimeManager.CreateInstance(PlayerIntroEvent);
		playerIntro.start();

		//--------------------------------------------------------------------
		// 5: The RuntimeManager can track Event Instances and update their 
		//    positions to match a given Game Object every frame. This is
		//    an easier alternative to manually doing this for every Instance
		//    such as shown in (8).
		//--------------------------------------------------------------------
		FMODUnity.RuntimeManager.AttachInstanceToGameObject(playerIntro, GetComponent<Transform>(), GetComponent<Rigidbody>());
	}

	void OnDestroy()
	{
		StopAllPlayerEvents();

		//--------------------------------------------------------------------
		// 6: This shows how to release resources when the unity object is 
		//    disabled
		//--------------------------------------------------------------------
		playerState.release();
	}

	void SpawnIntoWorld()
	{
		health = StartingHealth;

		//--------------------------------------------------------------------
		// 7: This shows that a single event instance can be started, stopped, 
		//    and restarted as often as needed.
		//--------------------------------------------------------------------
		playerState.start();
	}

	void Update()
	{
		//--------------------------------------------------------------------
		// 8: This shows how to manually update the instance of a 3D event so 
		//    it has the position and velocity of it's game object. (5) Show
		//    how this can be done automatically.
		//--------------------------------------------------------------------
		playerState.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject, cachedRigidBody));

		//--------------------------------------------------------------------
		//  9: This shows how to update a parameter of an instance every frame
		//--------------------------------------------------------------------
		playerState.setParameterValue("health", (float)health);

		//--------------------------------------------------------------------
		// 10: This shows how to query the playback state of an event instance.
		//     This can be useful when playing a one shot to take action
		//     when it finishes. Other playback states can be checked including
		//     Sustaining and Fading-Out.
		//--------------------------------------------------------------------
		if (playerIntro.isValid())
		{
			FMOD.Studio.PLAYBACK_STATE playbackState;
			playerIntro.getPlaybackState(out playbackState);
			if (playbackState == FMOD.Studio.PLAYBACK_STATE.STOPPED)
			{
				playerIntro.release();
				playerIntro.clearHandle();
				SendMessage("PlayerIntroFinished");
			}
		}
	}

	void TakeDamage()
	{
		health -= 1;

		//--------------------------------------------------------------------
		// 11: This shows how to play a one shot at the game objects current 
		//     location.
		//--------------------------------------------------------------------
		FMODUnity.RuntimeManager.PlayOneShot(DamageEvent, transform.position);

		if (health == 0)
		{
			//--------------------------------------------------------------------
			// 12: This shows how to stop a sound will allowing the the AHDSR set
			//     by the sound designer to control the fade out.
			//--------------------------------------------------------------------
			playerState.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		}
	}


	void ReceiveHealth(bool restoreAll)
	{
		if (restoreAll)
		{
			health = StartingHealth;
		}
		else
		{
			health = Math.Min(health + 3, StartingHealth);
		}        

		//--------------------------------------------------------------------
		// 13: This section shows how to manually play one shot sound so we can
		//     set a parameter before starting.
		//--------------------------------------------------------------------
		FMOD.Studio.EventInstance heal = FMODUnity.RuntimeManager.CreateInstance(HealEvent);
		heal.setParameterValue("FullHealth", restoreAll ? 1.0f : 0.0f);
		heal.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
		heal.start();
		heal.release();
	}


	//--------------------------------------------------------------------
	// 14: This section shows how to retrieve a bus, and how busses can be
	//     used to control multiple events. This can be usefull to stop
	//     one shots that are still playing but have no other way to be
	//     controlled.
	//--------------------------------------------------------------------
	void StopAllPlayerEvents()
	{
		FMOD.Studio.Bus playerBus = FMODUnity.RuntimeManager.GetBus("bus:/player");
		playerBus.stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);
	}
}