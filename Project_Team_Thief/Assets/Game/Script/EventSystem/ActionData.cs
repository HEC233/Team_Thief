using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS.Event
{
    public enum ActionType
    {
        Dialog, CutScene, RestrictStart, RestrictEnd,
        Spawn, CameraChange, CameraShake, CameraZoom,
        CharacterMove, CharacterAnimation, Effect,
        SFX, BGMChange, BGMEnd, BGMStart, BossActive
    }

    public enum CameraShakeIntensity
    {
        Strong, Wick
    }

    public enum PositionSetMethod
    {
        Absolute, Relative
    }

    [System.Serializable]
    public class ActionData
    {
        public ActionType type;
        public float actionLength;

        // for Dialog
        public string dialogName;

        // for Cut Scene
        public Sprite image;

        // for Restrict
        public Vector2Int leftDown;
        public Vector2Int rightUp;

        // for Spawn
        public string unitName;
        public Vector2Int spawnPos;
        public int count;

        // for Camera Change
        public string cameraName;

        // for Camera Shake
        public CameraShakeIntensity intensity;

        // for Zoom
        public int cameraSize;

        // for Character Move
        public Vector2Int movePos;

        // for Animation
        public string animationName;

        // for Effect
        public string effectName;
        public PositionSetMethod positionSetMethod;
        public string originPointObject;
        public Vector2Int effectPos;

        // for SFX
        public string sfxName;

        // for BGM Change
        public string bgmName;
    }
}