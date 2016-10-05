using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections;

namespace MatchThree
{
    [Serializable]
    public class Gem : MonoBehaviour, IPoolable
    {
        public GemColors GemColor;
        public int X;
        public int Y;
        private bool active;

        void Awake()
        {
            gameObject.GetComponent<Button>().onClick.AddListener(new UnityAction(OnClick));
        }

        public void Reset()
        {
            gameObject.SetActive(false);
        }

        public void Initialize()
        {
            
        }

        public bool IsActive
        {
            get { return active; }
            set
            {
                if (value)
                {
                    Initialize();
                }
                else
                {
                    Reset();
                }
                active = value;
            }
        }

        void OnClick()
        {
            EventManager.CallGemClickEvent(this);
        }
    }
}
