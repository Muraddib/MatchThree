using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;

namespace MatchThree
{
    public class UIFormEnd : UIForm
    {
        public Button RestartButton;
        public Text ScoreText;

        public void Init(Action onRestartButtonClick)
        {
            RestartButton.onClick.AddListener(new UnityAction(onRestartButtonClick));
            EventManager.OnGameEvent += HandleGameEvent;
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            EventManager.OnGameEvent -= HandleGameEvent;
        }

        private void UpdateScore()
        {
            ScoreText.text = GameplayController.Instance.PlayerScore.ToString();
        }

        public override void HandleGameEvent(EventManager.GameEvents gameEvent)
        {
            switch (gameEvent)
            {
                case EventManager.GameEvents.GameEnd:
                    gameObject.SetActive(true);
                    UpdateScore();
                    break;
                case EventManager.GameEvents.GameStart:
                    gameObject.SetActive(false);
                    break;
            }
        }
    }
}