using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MatchThree
{
    public class UIController : Singleton<UIController>
    {
        private void Awake()
        {
            EventManager.OnGameEvent += EventManager_OnGameEvent;
        }

        private void EventManager_OnGameEvent(EventManager.GameEvents eventID)
        {

        }

        private void OnDestroy()
        {
            EventManager.OnGameEvent -= EventManager_OnGameEvent;
        }

        public void Init(GameUIForms UIForms, RectTransform UIRoot)
        {
            foreach (var UIForm in UIForms.Forms)
            {
                GameObject newForm = Instantiate(UIForm.FormPrefab) as GameObject;
                newForm.GetComponent<RectTransform>().SetParent(UIRoot);
                newForm.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
                newForm.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                newForm.GetComponent<RectTransform>().localScale = Vector3.one;
                var form = newForm.GetComponent<UIForm>();
                switch (form.FormID)
                {
                    case UIFormIDs.GameStart:
                        ((UIFormStart)form).Init(
                            onStartButtonClick: () => EventManager.CallGameEvent(EventManager.GameEvents.GameStart));
                        break;
                    case UIFormIDs.GameEnd:
                        ((UIFormEnd)form).Init(
                            onRestartButtonClick: () => EventManager.CallGameEvent(EventManager.GameEvents.GameStart));
                        break;
                    case UIFormIDs.ScreenUI:
                        ((UIScreenGUI)form).Init();
                        break;
                }
            }
        }
    }
}