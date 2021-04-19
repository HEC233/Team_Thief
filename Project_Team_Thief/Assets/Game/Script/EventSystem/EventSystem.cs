using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PS.Event;

public class EventSystem : MonoBehaviour
{
    private class ProgressInfo
    {
        public int currentCutScene;
        public int currentAction;
        public int leftCount;
    }

    public List<PS.Event.Event> events;

    private List<string> nextEventQueue = new List<string>();

    private void Start()
    {
        foreach (var e in events)
        {
            StartCoroutine(Process(e));
        }
    }

    IEnumerator Process(PS.Event.Event data)
    {
        ProgressInfo info = new ProgressInfo();
        info.leftCount = data.playCount;

        while (data.playAmount == PlayAmount.Infinite || (data.playAmount == PlayAmount.Finite && info.leftCount-- > 0))
        {
            bool returnValue = false;
            while (!returnValue)
            {
                switch (data.triggerType)
                {
                    case TriggerType.Talk:
                        returnValue = TalkCheck();
                        break;
                    case TriggerType.Arrive:
                        returnValue = ArriveCheck();
                        break;
                    case TriggerType.Come:
                        returnValue = ComeCheck();
                        break;
                    case TriggerType.Next:
                        returnValue = NextCheck(data.name);
                        break;
                }
                yield return null;
            }

            info.currentCutScene = 0;
            info.currentAction = 0;

            while (true)
            {
                if (info.currentCutScene >= data.cutScenes.Count)
                    break;
                if (info.currentAction >= data.cutScenes[info.currentCutScene].actions.Count)
                {
                    info.currentAction = 0;
                    info.currentCutScene++;
                    continue;
                }

                var actionData = data.cutScenes[info.currentCutScene].actions[info.currentAction];
                switch (actionData.type)
                {
                    case ActionType.Dialog:
                        yield return StartCoroutine(Dialog(actionData.dialogName));
                        break;
                    case ActionType.CutScene:
                        break;
                    case ActionType.RestrictStart:
                        break;
                    case ActionType.RestrictEnd:
                        break;
                    case ActionType.Spawn:
                        yield return StartCoroutine(Spawn(actionData.unitName, actionData.spawnPos, actionData.count));
                        break;
                    case ActionType.CameraChange:
                        break;
                    case ActionType.CameraShake:
                        break;
                    case ActionType.CameraZoom:
                        break;
                    case ActionType.CharacterMove:
                        break;
                    case ActionType.CharacterAnimation:
                        break;
                    case ActionType.Effect:
                        break;
                    case ActionType.SFX:
                        break;
                    case ActionType.BGMChange:
                        break;
                    case ActionType.BGMEnd:
                        break;
                    case ActionType.BGMStart:
                        break;
                }

                info.currentAction++;
                yield return null;
            }


            nextEventQueue.Add(data.followingEvent);
        }

    }

    private bool TalkCheck()
    {
        return false;
    }
    private bool ArriveCheck()
    {
        return true;
    }
    private bool ComeCheck()
    {
        return false;
    }
    private bool NextCheck(string name)
    {
        bool returnValue = nextEventQueue.Contains(name);
        nextEventQueue.Remove(name);
        return returnValue;
    }
    private IEnumerator Dialog(string dialogName)
    {
        yield return null;
    }

    private IEnumerator CutScene(Sprite image)
    {
        yield return null;
    }

    private IEnumerator RestrictStart(Vector2Int leftDown, Vector2Int rightUp)
    {
        yield return null;
    }

    private IEnumerator RestrictEnd()
    {
        yield return null;
    }

    private IEnumerator Spawn(string unitName, Vector2Int spawnPos, int count)
    {
        Debug.Log(unitName + "스폰 명령이 넘어왔어요 ㅎ,,");

        yield break;
    }

    private IEnumerator CameraChange(string cameraName)
    {
        yield return null;
    }

    private IEnumerator CameraShake(CameraShakeIntensity intensity)
    {
        yield return null;
    }

    private IEnumerator CameraZoom(int cameraSize)
    {
        yield return null;
    }

    private IEnumerator CharacterMove(Vector2Int movePos)
    {
        yield return null;
    }

    private IEnumerator CharacterAnimation(string animationName)
    {
        yield return null;
    }

    private IEnumerator Effect(string effectName, Vector2Int effectPos)
    {
        yield return null;
    }

    private IEnumerator SFX(string sfxName)
    {
        yield return null;
    }

    private IEnumerator BGMChange(string bgmName)
    {
        yield return null;
    }

    private IEnumerator BGMEnd()
    {
        yield return null;
    }

    private IEnumerator BGMStart()
    {
        yield return null;
    }
}
