using UnityEngine;
using System.Collections;
using System;

namespace MatchThree
{
    public abstract class UIForm : MonoBehaviour
    {
        public abstract void HandleGameEvent(EventManager.GameEvents gameEvents);
        public UIFormIDs FormID;
    }
}
