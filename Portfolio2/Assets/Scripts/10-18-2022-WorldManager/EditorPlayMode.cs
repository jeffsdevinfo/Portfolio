// Demo code
// https://answers.unity.com/questions/447701/event-for-unity-editor-pause-and-playstop-events.html
// Stephen Lautier

using UnityEditor;
using System;

public enum PlayModeState
{
    Stopped,
     Playing,
     Paused
 }

[InitializeOnLoad]
public class EditorPlayMode
{
    private static PlayModeState _currentState = PlayModeState.Stopped;

    static EditorPlayMode()
    {
        EditorApplication.playmodeStateChanged = OnUnityPlayModeChanged;
    }

    public static event Action<PlayModeState, PlayModeState> PlayModeChanged;

    public static void Play()
    {
        EditorApplication.isPlaying = true;
    }

    public static void Pause()
    {
        EditorApplication.isPaused = true;
    }

    public static void Stop()
    {
        EditorApplication.isPlaying = false;
    }


    private static void OnPlayModeChanged(PlayModeState currentState, PlayModeState changedState)
    {
        if (PlayModeChanged != null)
            PlayModeChanged(currentState, changedState);
    }

    private static void OnUnityPlayModeChanged()
    {
        var changedState = PlayModeState.Stopped;
        switch (_currentState)
        {
            case PlayModeState.Stopped:
                if (EditorApplication.isPlayingOrWillChangePlaymode)
                {
                    changedState = PlayModeState.Playing;
                }
                break;
            case PlayModeState.Playing:
                if (EditorApplication.isPaused)
                {
                    changedState = PlayModeState.Paused;
                }
                else
                {
                    changedState = PlayModeState.Stopped;
                }
                break;
            case PlayModeState.Paused:
                if (EditorApplication.isPlayingOrWillChangePlaymode)
                {
                    changedState = PlayModeState.Playing;
                }
                else
                {
                    changedState = PlayModeState.Stopped;
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        // Fire PlayModeChanged event.
        OnPlayModeChanged(_currentState, changedState);

        // Set current state.
        _currentState = changedState;
    }

}
