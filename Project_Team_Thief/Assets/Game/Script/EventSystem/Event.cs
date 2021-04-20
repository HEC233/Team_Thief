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
        Talk,Arrive,Come,Next
    }

    public enum CmpType
    {
        Bigger, Smaller, Equal, None
    }
    
    public enum PlayAmount
    {
        Finite, Infinite
    }

    [CreateAssetMenu(fileName = "Event", menuName = "ScriptableObject/Event")]
    public class Event : ScriptableObject
    {
        public string eventIndex;
        public string name;
        public string stopCondition;
        public EventType eventType;
        public TriggerType triggerType;
        [StructLayout(LayoutKind.Explicit)]
        public struct TriggerCondition
        {
            // for Talk
            //[FieldOffset(0)]
            // for Arrive
            //[FieldOffset(0)]
            // for Come
            [FieldOffset(0)] public CmpType xCmp;
            [FieldOffset(4)] public int xValue;
            [FieldOffset(8)] public CmpType yCmp;
            [FieldOffset(12)] public int yValue;
        }
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