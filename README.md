
# fmod-theater
A Unity project that is intended to be an interface for triggering fmod events via OSC.

This project contains 4 modules:

 1. QLab Workspace - Use this file to run the show. OSC cues require a [QLab](http://figure53.com/qlab) license (or can be run in demo mode).
 2. Unity Build - Run this application to play and stop sound events. This application listens for OSC messages on port 53002.
 3. Unity Project - Explore this project in the [Unity Editor](https://unity3d.com/unity/editor) to examine how OSC messages control FMOD events.
 4. FMOD Project - Explore how the FMOD events were designed in [FMOD Studio](https://fmod.com/download).

# Tips

## No Audio
If you are running the Unity Build, and don't hear any audio, even when triggering events directly from the application UI, make sure you have your speakers configured in Audio MIDI Setup (Applications > Utilities).

Select your ouput device, click the "Configure Speakers" button, and make sure your audio interface outputs have channel assignments assigned. The project was built for a 7.1 system, but should support stereo as well.

## QLab Cues Don't Do Anything
Try using the UI controls directly in the Unity application:

 - Play toggle starts and stops the sound
 - Volume goes from 0.0 (mute) to 1.0 (max)
 - Some events have additional sliders that clamp down audio as well.
 - Try the speaker check dropdown.

If OSC doesn't seem to be reaching the application, open the project in the Unity Editor, try sending the cues again, and look at the console messages to see if there are any errors.
