using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;

namespace MatchThree
{
    public class UIScreenGUI : UIForm
    {
        public Text CurrentSteps;
        public Text CurrentTime;

        public void Init()
        {
            EventManager.OnGameEvent += HandleGameEvent;
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            EventManager.OnGameEvent -= HandleGameEvent;
        }

        void Update()
        {
            if (GameplayController.Instance.GameActive)
            {
                CurrentSteps.text = string.Concat("STEPS:", GameplayController.Instance.PlayerSteps);
                CurrentTime.text = string.Concat("TIME:", GameplayController.Instance.PlayerSeconds);
            }
        }

        public override void HandleGameEvent(EventManager.GameEvents gameEvent)
        {
            switch (gameEvent)
            {
                case EventManager.GameEvents.GameStart:
                    gameObject.SetActive(true);
                    break;
                case EventManager.GameEvents.GameEnd:
                    gameObject.SetActive(false);
                    break;
            }
        }
    }
}
