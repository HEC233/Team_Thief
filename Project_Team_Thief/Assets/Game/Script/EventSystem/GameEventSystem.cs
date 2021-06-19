using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PS.Event;
using PS.Util.Tile;
using Cinemachine;

public class GameEventSystem : MonoBehaviour
{
    public List<PS.Event.Event> events;

    private List<string> nextEventQueue = new List<string>();
    private List<string> evnetQueue = new List<string>();

    GameObject _player;
    private void Start()
    {
        foreach (var e in events)
        {
            StartCoroutine(Process(e));
        }
        _player = GameObject.Find("Player");
    }

    private bool bRestrict;
    private Vector3 restrictLeftBottom;
    private Vector3 restrictRightTop;
    private void Update()
    {
        if (bRestrict)
        {
            if(_player)
            {
                _player.transform.position = new Vector3(Mathf.Clamp(_player.transform.position.x, restrictLeftBottom.x, restrictRightTop.x),
                    Mathf.Clamp(_player.transform.position.y, restrictLeftBottom.y, restrictRightTop.y), _player.transform.position.z);
            }
        }
    }

    public void AddQueue(string value)
    {
        evnetQueue.Add(value);
        
        GameManager.instance?.AddTextToDeveloperConsole(value + " GameEventSystem queue added");
    }

    IEnumerator Process(PS.Event.Event data)
    {
        int leftCount = data.playCount;

        // 이벤트가 남은 횟수만큼 반복
        while (data.playAmount == PlayAmount.Infinite || (data.playAmount == PlayAmount.Finite && leftCount-- > 0))
        {
            if (data.bFollowed)
            {
                while (!NextCheck(data.eventIndex))
                {
                    yield return null;
                }
                evnetQueue.RemoveAll(str => str == data.trigger.NPCname);
            }
            // 이벤트가 발동조건을 만족 할때까지 반복
            bool returnValue = false;
            while (!returnValue)
            {
                switch (data.triggerType)
                {
                    case TriggerType.Queue:
                        returnValue = TalkCheck(data.trigger);
                        break;
                    case TriggerType.TRUE:
                        returnValue = ArriveCheck();
                        break;
                    case TriggerType.Come:
                        returnValue = ComeCheck(data.trigger);
                        break;
                }
                yield return null;
            }

            // 이벤트가 사용할 값들 초기화
            int currentCutScene = 0;
            int currentAction = 0;
            float startTime = Time.realtimeSinceStartup;
            float passedTime = startTime;
            bool actionSkipPressed = false;
            bool skip = false;

            //if(data.stoptime)
            //{
            //    GameManager.instance.timeMng.StopTime();
            //}

            while (true)
            {
                //여기서 이벤트 자체 스킵 체크 해줘야 함 미치곘네

                // 컷신을 모두 보면 이번 이벤트를 끝냄
                if (currentCutScene >= data.cutScenes.Count)
                    break;

                actionSkipPressed = Input.GetKeyDown(KeyCode.Space);
                skip = skip || (actionSkipPressed && data.cutScenes[currentCutScene].skipable);
                // 스킵을 버튼이 눌렸는지
                // 연출이 모두 끝나면 다음 컷씬으로 넘어감
                if (currentAction >= data.cutScenes[currentCutScene].actions.Count)
                {
                    passedTime = Time.realtimeSinceStartup - startTime;
                    // 컷씬 시간이 남아있는지 그리고 스킵넘기기가 가능하며 스킵을 안헀는지 체크
                    if (data.cutScenes[currentCutScene].actionTime - passedTime >= 0 && !skip)
                    {
                        yield return null;
                        continue;
                    }
                    // 컷씬 넘기기를 했는지 체크
                    if (!data.cutScenes[currentCutScene].autopass && !actionSkipPressed)
                    {
                        yield return null;
                        continue;
                    }
                    // 다음 컷씬으로 넘어가는 처리
                    currentAction = 0;
                    currentCutScene++;
                    startTime = Time.realtimeSinceStartup;
                    skip = false;
                    continue;
                }

                float time;
                var actionData = data.cutScenes[currentCutScene].actions[currentAction];
                // 스킵을 했을때, 한프레임안에 다 처리해야 하고 다음 컷씬으로 넘어감.
                if (skip)
                    time = 0;
                // 스킵을 하지 않았을때, 여러프레임에 거쳐서 할수있음
                else
                    time = actionData.actionLength;
                
                switch (actionData.type)
                {
                    case ActionType.Dialog:
                        yield return StartCoroutine(Dialog(actionData.dialogName));
                        break;
                    case ActionType.CutScene:
                        break;
                    case ActionType.RestrictStart:
                        yield return StartCoroutine(RestrictStart(actionData.leftDown, actionData.rightUp));
                        break;
                    case ActionType.RestrictEnd:
                        yield return StartCoroutine(RestrictEnd());
                        break;
                    case ActionType.Spawn:
                        yield return StartCoroutine(Spawn(actionData.unitName, actionData.spawnPos, actionData.count));
                        break;
                    case ActionType.CameraChange:
                        yield return StartCoroutine(CameraChange(actionData.cameraName, actionData.actionLength));
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
                        yield return StartCoroutine(Effect(actionData.effectName, actionData.positionSetMethod, actionData.originPointObject, actionData.effectPos));
                        break;
                    case ActionType.SFX:
                        break;
                    case ActionType.BGMChange:
                        break;
                    case ActionType.BGMEnd:
                        break;
                    case ActionType.BGMStart:
                        break;
                    case ActionType.BGMStateChange:
                        yield return StartCoroutine(ChangeBGMState(actionData.bgmStateGroupName,
                            actionData.bgmStateName));
                        break;
                    case ActionType.BossActive:
                        yield return StartCoroutine(BossActive(actionData.unitName));
                        break;
                    case ActionType.LoadScene:
                        GameManager.instance.LoadGame(actionData.sceneName);
                        break;

                }
                currentAction++;
                if (skip)
                    continue;
                else
                    yield return null;
            }

            // 여기서 이벤트 종료조건 검사 해줘야 함
            if (data.stopCondition != string.Empty)
            {

            }

            //if (data.stoptime)
            //{
            //    GameManager.instance.timeMng.ResumeTime();
            //}

            // 이벤트가 끝나면 다음 따라올 이벤트를 넣어줌
            nextEventQueue.Add(data.followingEvent);
            evnetQueue.RemoveAll(str => str == data.trigger.NPCname);
        }

    }

    private bool TalkCheck(PS.Event.TriggerCondition trigger)
    {
        bool returnValue = evnetQueue.Contains(trigger.NPCname);
        if (returnValue)
            evnetQueue.RemoveAll(str => str == trigger.NPCname);
        return returnValue;
    }
    private bool ArriveCheck()
    {
        return true;
    }
    private bool ComeCheck(PS.Event.TriggerCondition trigger)
    {
        Unit player = GameManager.instance?.GetControlActor()?.GetUnit();
        if (player == null) return false;
        var pos = GameManager.instance.GetControlActor().GetUnit().transform.TileCoord();
        return ((trigger.xCmp == CmpType.Bigger && trigger.xValue < pos.x) ||
            (trigger.xCmp == CmpType.Equal && trigger.xValue == pos.x) ||
            (trigger.xCmp == CmpType.Smaller && trigger.xValue > pos.x) ||
            (trigger.xCmp == CmpType.None)) &&
            ((trigger.yCmp == CmpType.Bigger && trigger.yValue < pos.y) ||
            (trigger.yCmp == CmpType.Equal && trigger.yValue == pos.y) ||
            (trigger.yCmp == CmpType.Smaller && trigger.yValue > pos.y) ||
            (trigger.yCmp == CmpType.None));
    }
    private bool NextCheck(string name)
    {
        bool returnValue = nextEventQueue.Contains(name);
        if (returnValue)
            nextEventQueue.RemoveAll(str => str == name);
        return returnValue;
    }

    private IEnumerator Dialog(string dialogName)
    {
        GameManager.instance.dialogueSystem.StartDialogueWithName(dialogName);

        while (GameManager.instance.dialogueSystem.CheckRunning())
        {
            yield return null;
        }
    }

    private IEnumerator CutScene(Sprite image)
    {
        yield return null;
    }

    private IEnumerator RestrictStart(Vector2Int leftDown, Vector2Int rightUp)
    {
        restrictLeftBottom = new Vector3(leftDown.x, leftDown.y, 0);
        restrictRightTop = new Vector3(rightUp.x + 1, rightUp.y + 1, 0);
        bRestrict = true;

        yield return null;
    }

    private IEnumerator RestrictEnd()
    {
        bRestrict = false;

        yield return null;
    }

    private IEnumerator Spawn(string unitName, Vector2Int spawnPos, int count)
    {
        GameManager.instance?.spawner.SpawnMany(unitName, spawnPos.TileCoordToPosition3(), count);

        yield break;
    }

    private IEnumerator CameraChange(string cameraName, float actionLength)
    {
        var go = GameObject.Find(cameraName);
        if (go == null) yield break;

        GameManager.instance.cameraMng.AllCinemachineFollowChange(go.transform);

        yield return new WaitForSeconds(actionLength);
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

    private IEnumerator Effect(string effectName, PositionSetMethod method, string objectName, Vector2Int effectPos)
    {
        if (method == PositionSetMethod.Absolute)
            GameManager.instance?.FX.Play(effectName, effectPos.TileCoordToPosition3(), Quaternion.identity);
        else if (method == PositionSetMethod.Relative)
        {
            var obj = GameObject.Find(objectName);
            if(obj)
                GameManager.instance?.FX.Play(effectName, obj.transform.position + effectPos.TileCoordToPosition3(), Quaternion.identity);
        }

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
    
    private IEnumerator ChangeBGMState(string StateGroup, string bgmStateName)
    {
        WwiseSoundManager.instance.ChangeBGMState(StateGroup, bgmStateName);
        yield return null;
    }

    private IEnumerator BossActive(string bossName)
    {
        var go = GameObject.Find(bossName);

        if (go == null) yield break;
        var boss = go.GetComponentInChildren<IActor>();
        if (boss == null) yield break;
        boss.Transition(TransitionCondition.BossAwake);

        yield return null;
    }
}
