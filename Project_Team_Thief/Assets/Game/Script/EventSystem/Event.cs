using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

namespace PS.Event
{
    public enum TriggerType
    {
        Queue,Always,Come
    }

    public enum CmpType
    {
        Bigger, Smaller, Equal, None
    }
    
    public enum PlayAmount
    {
        Finite, Infinite
    }

    [System.Serializable]
    public struct TriggerCondition
    {
        public string NPCname;
        public CmpType xCmp;
        public int xValue;
        public CmpType yCmp;
        public int yValue;
    }

    [CreateAssetMenu(fileName = "Event", menuName = "ScriptableObject/Event/Event")]
    public class Event : ScriptableObject
    {
        public bool bFollowed;
        public string eventIndex;
        public string eventScene;
        public TriggerType triggerType;
        
        public TriggerCondition trigger;

        //EndCondition
        public PlayAmount playAmount;
        public int playCount;
        public bool stoptime;
        public bool skipable;
        //getter
        public string followingEvent;

        public List<CutSceneData> cutScenes;
    }
}