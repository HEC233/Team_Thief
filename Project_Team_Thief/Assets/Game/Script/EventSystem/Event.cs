using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

namespace PS.Event
{
    public enum EventType
    {
        Scene, Script, Battle
    }

    public enum TriggerType
    {
        Queue,TRUE,Come,Next
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

    [CreateAssetMenu(fileName = "Event", menuName = "ScriptableObject/Event")]
    public class Event : ScriptableObject
    {
        public string eventIndex;
        public string name;
        public string stopCondition;
        public EventType eventType;
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