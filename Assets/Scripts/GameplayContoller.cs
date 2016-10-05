using UnityEngine;
using System.Collections;

namespace MatchThree
{
    public class GameplayController : Singleton<GameplayController>
    {
        public GameSettings Settings;
        public bool GameActive;
        public int PlayerSteps;
        public float PlayerSeconds;
        public int PlayerScore;

        private void Awake()
        {
            EventManager.OnGameEvent += EventManager_OnGameEvent;
            EventManager.OnGemMatchEvent += EventManager_OnGemMatchEvent;
        }

        private void OnDestroy()
        {
            EventManager.OnGameEvent -= EventManager_OnGameEvent;
            EventManager.OnGemMatchEvent -= EventManager_OnGemMatchEvent;
        }

        public void Init(GameSettings settings)
        {
            Settings = settings;
        }

        private void EventManager_OnGemMatchEvent(int matchLength, GridController.MatchDirection direction)
        {
            PlayerScore += 100;
            switch (direction)
            {
                    case GridController.MatchDirection.Horizontal:
                    if (matchLength >= 4) AddSeconds();
                    break;
                    case GridController.MatchDirection.Vertical:
                    if (matchLength >= 4) AddSteps();
                    break;
            }
        }

        private void EventManager_OnGameEvent(EventManager.GameEvents eventID)
        {
            switch (eventID)
            {
                case EventManager.GameEvents.GameStart:
                    GameActive = true;
                    PlayerScore = 0;
                    PlayerSteps = Settings.StartSteps;
                    PlayerSeconds = Settings.StartTime;
                    break;
                case EventManager.GameEvents.GemSwap:
                    MakeStep();
                    break;
            }
        }

        private void MakeStep()
        {
            PlayerSteps--;
            if (PlayerSteps <= 0)
            {
                GameActive = false;
                EventManager.CallGameEvent(EventManager.GameEvents.GameEnd);
            }
        }

        private void AddSteps()
        {
            PlayerSteps += Settings.BonusSteps;
        }

        private void AddSeconds()
        {
            PlayerSeconds += Settings.BonusSeconds;
        }

        void Update()
        {
            if (GameActive)
            {
                PlayerSeconds -= Time.deltaTime;
                if (PlayerSeconds <= 0f)
                {
                    GameActive = false;
                    EventManager.CallGameEvent(EventManager.GameEvents.GameEnd);
                }
            }
        }
    }
}
