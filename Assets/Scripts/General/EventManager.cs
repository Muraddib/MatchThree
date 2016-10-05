using UnityEngine;
using System.Collections;

namespace MatchThree
{
    public class EventManager : MonoBehaviour
    {
        public enum GameEvents
        {
            GameLoaded,
            GameStart,
            GameEnd,
            GemSwap
        }

        public delegate void GameEvent(GameEvents eventID);
        public static event GameEvent OnGameEvent;
        public static void CallGameEvent(GameEvents eventID)
        {
            if (OnGameEvent != null)
            {
                OnGameEvent(eventID);
            }
        }

        public delegate void GemClickEvent(Gem gem);
        public static event GemClickEvent OnGemClickEvent;
        public static void CallGemClickEvent(Gem gem)
        {
            if (OnGemClickEvent != null)
            {
                OnGemClickEvent(gem);
            }
        }

        public delegate void GemMatchEvent(int matchLength, GridController.MatchDirection direction);
        public static event GemMatchEvent OnGemMatchEvent;
        public static void CallGemMatchEvent(int matchLength, GridController.MatchDirection direction)
        {
            if (OnGemMatchEvent != null)
            {
                OnGemMatchEvent(matchLength, direction);
            }
        }

        public delegate void SoundEvent(AudioIDs sound, Vector3 pos);
        public static event SoundEvent OnSoundEvent;
        public static void CallSoundEvent(AudioIDs sound, Vector3 pos)
        {
            if (OnSoundEvent != null)
                OnSoundEvent(sound, pos);
        }
    }
}