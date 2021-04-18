using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace PS.Event
{
    [Serializable]
    public class CutSceneData
    {
        public string cutSceneIndex;
        public string cutSceneName;

        public float actionTime;
        public bool skipable;
        public bool autopass;
        public List<ActionData> actions;
    }
}