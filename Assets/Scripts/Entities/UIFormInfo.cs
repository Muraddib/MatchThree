using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;

namespace MatchThree
{
    [Serializable]
    public class UIFormInfo
    {
        public UIFormIDs FormID;
        public GameObject FormPrefab;
    }
}