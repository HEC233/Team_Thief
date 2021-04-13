using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PS.Enemy.Seraphim.AI;
using PS.Util.Tile;

public class SeraphimAI : MonoBehaviour
{
    public LayerMask unMovableLayer;
    public bool isLookRight;

    public Unit target;
    private AIState _curState;
    public IActor actor;

    private Vector2Int _myCoord;
    private Vector2Int _targetCoord;

    private bool isActionEnd = false;
    public bool IsActionEnd 
    { 
        get 
        { 
            if (isActionEnd) 
            { 
                isActionEnd = false; 
                return true; 
            } 
            else 
                return false; 
        } 
    }
    public void ActionEndCallback() { isActionEnd = true; }

    public Search search = new Search();
    public Combat combat = new Combat();

    [Header("Sight")]
    [SerializeField] private int frontView;
    [SerializeField] private int rearView;
    [SerializeField] private int upperView;
    [SerializeField] private int underView;

    [Header("Movement")]
    public Vector2 IdleTimeLength = Vector2.zero;
    public Vector2 walkTimeLength = Vector2.zero;

    [Header("Combat")]
    public int attackDistance;
    public Vector2 keepDistanceTimeLength = Vector2.zero;
    public float shotCoolTime;
    public float backStepCoolTime;    
    public float backStep = 0.8f;

    private void Start()
    {
        isLookRight = true;
        actor = GetComponent<SeraphimActor>();
        _curState = search;
        _curState.Enter(this);

        StartCoroutine(TargetSetCoroutine());
    }

    IEnumerator TargetSetCoroutine()
    {
        yield return null; 
        if (GameManager.instance.GetControlActor() != null)
        {
            SetTarget(GameManager.instance.GetControlActor().GetUnit());
        }
    }

    public void ChangeState(AIState newState)
    {
        _curState.Exit();
        _curState = newState;
        _curState.Enter(this);
    }

    public void SetTarget(Unit newTarget)
    {
        target = newTarget;
    }

    // 시야에 목표가 있는지 판단
    public bool CheckSight()
    {
        if (target == null)
            return false;

        _myCoord = transform.TileCoord();
        _targetCoord = target.transform.TileCoord();

        // 시야 박스
        Vector2Int viewBox = _targetCoord - _myCoord;
        if (!isLookRight)
            viewBox.x = -viewBox.x;

        if (viewBox.x >= rearView && viewBox.x <= frontView && viewBox.y <= upperView && viewBox.y >= underView)
            return true;
        else
            return false;
    }

    private void OnDrawGizmos()
    {
        if (target != null)
        {
            if (isLookRight)
            {
                Debug.DrawLine((_myCoord + new Vector2Int(rearView, upperView)).TileCoordToPosition3(), (_myCoord + new Vector2Int(rearView, underView)).TileCoordToPosition3(), Color.red);
                Debug.DrawLine((_myCoord + new Vector2Int(rearView, underView)).TileCoordToPosition3(), (_myCoord + new Vector2Int(frontView, underView)).TileCoordToPosition3(), Color.red);
                Debug.DrawLine((_myCoord + new Vector2Int(frontView, underView)).TileCoordToPosition3(), (_myCoord + new Vector2Int(frontView, upperView)).TileCoordToPosition3(), Color.red);
                Debug.DrawLine((_myCoord + new Vector2Int(frontView, upperView)).TileCoordToPosition3(), (_myCoord + new Vector2Int(rearView, upperView)).TileCoordToPosition3(), Color.red);
            }
            else
            {
                Debug.DrawLine((_myCoord + new Vector2Int(-rearView, upperView)).TileCoordToPosition3(), (_myCoord + new Vector2Int(-rearView, underView)).TileCoordToPosition3(), Color.red);
                Debug.DrawLine((_myCoord + new Vector2Int(-rearView, underView)).TileCoordToPosition3(), (_myCoord + new Vector2Int(-frontView, underView)).TileCoordToPosition3(), Color.red);
                Debug.DrawLine((_myCoord + new Vector2Int(-frontView, underView)).TileCoordToPosition3(), (_myCoord + new Vector2Int(-frontView, upperView)).TileCoordToPosition3(), Color.red);
                Debug.DrawLine((_myCoord + new Vector2Int(-frontView, upperView)).TileCoordToPosition3(), (_myCoord + new Vector2Int(-rearView, upperView)).TileCoordToPosition3(), Color.red);
            }
        }

        if (isLookRight)
        {
            Gizmos.DrawCube((transform.TileCoord() + new Vector2Int(1, 1)).TileCoordToPosition3(), new Vector3(0.5f, 0.5f, 1));
            Gizmos.DrawCube((transform.TileCoord() + new Vector2Int(1, 0)).TileCoordToPosition3(), new Vector3(0.5f, 0.5f, 1));
            Gizmos.DrawCube((transform.TileCoord() + new Vector2Int(1, -1)).TileCoordToPosition3(), new Vector3(0.5f, 0.5f, 1));
        }
        else
        {
            Gizmos.DrawCube((transform.TileCoord() + new Vector2Int(-1, 1)).TileCoordToPosition3(), new Vector3(0.5f, 0.5f, 1));
            Gizmos.DrawCube((transform.TileCoord() + new Vector2Int(-1, 0)).TileCoordToPosition3(), new Vector3(0.5f, 0.5f, 1));
            Gizmos.DrawCube((transform.TileCoord() + new Vector2Int(-1, -1)).TileCoordToPosition3(), new Vector3(0.5f, 0.5f, 1));
        }
    }

    // 움직일 수 있는 지 판별
    public bool CheckMovable(bool isGoingRight)
    {
        Vector2Int nowTileCoord = transform.position.TileCoord();
        bool result;
        if (isGoingRight)
        {
            result = (
                !nowTileCoord.CheckObjectExist(unMovableLayer, 1, 1) &
                !nowTileCoord.CheckObjectExist(unMovableLayer, 1, 0) &
                nowTileCoord.CheckObjectExist(unMovableLayer, 1, -1));
        }
        else
        {
            result = (
                !nowTileCoord.CheckObjectExist(unMovableLayer, -1, 1) &
                !nowTileCoord.CheckObjectExist(unMovableLayer, -1, 0) &
                nowTileCoord.CheckObjectExist(unMovableLayer, -1, -1));
        }

        //Debug.Log("이동 가능 한지의 여부 " + result);
        return result;
    }

    // ai와 타겟유닛 사이의 거리를 반환
    public int GetDistance(bool isAbsolute = true)
    {
        if (target != null)
        {
            int result = target.transform.TileCoord().x - transform.TileCoord().x;
            return isAbsolute ? Mathf.Abs(result) : result;
        }

        return int.MaxValue;
    }

    private void Update()
    {
        _curState.Process();
    }
}

namespace PS.Enemy.Seraphim.AI
{
    public abstract class AIState
    {
        public abstract void Enter(SeraphimAI ai);
        public abstract void Process();
        public abstract void Exit();
    }

    /*
     * 순찰 상태!
     */
    public class Search : AIState
    {
        private float _timeCheck;
        private bool _isMoving;
        SeraphimAI ai;
        private Vector2Int _lastCoord;
        private Vector2Int _curCoord;

        public override void Enter(SeraphimAI ai)
        {
            _timeCheck = 0;
            _isMoving = false;
            this.ai = ai;
            _lastCoord = ai.transform.TileCoord();
        }
        public override void Exit()
        {
        }
        public override void Process()
        {
            _timeCheck -= GameManager.instance.timeMng.DeltaTime;
            if (_timeCheck <= 0)
            {
                if (Random.value > 0.5f)
                {
                    _isMoving = true;
                    if (Random.value > 0.5f)
                        ai.isLookRight = true;
                    else
                        ai.isLookRight = false;
                    _timeCheck = Random.Range(ai.walkTimeLength.x, ai.walkTimeLength.y);
                }
                else
                {
                    _isMoving = false;
                    _timeCheck = Random.Range(ai.IdleTimeLength.x, ai.IdleTimeLength.y);
                }
            }

            if (_isMoving)
                Move();
            else
                Stop();

            if (ai.CheckSight())
                ai.ChangeState(ai.combat);
        }

        private void Move()
        {
            _curCoord = ai.transform.TileCoord();
            if (_lastCoord != _curCoord)
            {
                _lastCoord = _curCoord;
                if (!ai.CheckMovable(ai.isLookRight))
                    ai.isLookRight = !ai.isLookRight;
            }
            ai.actor.Transition(ai.isLookRight ? TransitionCondition.RightMove : TransitionCondition.LeftMove);
        }

        private void Stop()
        {
            ai.actor.Transition(TransitionCondition.StopMove);
        }
    }

    /*
     * 전투 상태!
     */
    public class Combat : AIState
    {
        SeraphimAI ai;

        private float _shotCool;
        private float _backStepCool;
        private float _timeCheck;

        private enum InnerState
        {
            Shot, BackStep, KeepDistance, Wait, Rest, CheckDistance
        }
        private InnerState _state;

        public override void Enter(SeraphimAI ai)
        {
            this.ai = ai;
            _shotCool = 0;
            _backStepCool = 0;
            _timeCheck = 0;
            _state = InnerState.Wait;
        }

        public override void Exit()
        {
        }

        public override void Process()
        {
            switch (_state)
            {
                case InnerState.CheckDistance:

                    if(ai.GetDistance(false) > 0)
                    {
                        ai.actor.Transition(TransitionCondition.LookRight);
                    }
                    else
                    {
                        ai.actor.Transition(TransitionCondition.LookLeft);
                    }
                    if(!ai.CheckSight())
                    {
                        ai.ChangeState(ai.search);
                        return;
                    }
                    if (ai.GetDistance(true) > ai.attackDistance)
                    {
                        _state = InnerState.KeepDistance;
                    }
                    else if (ai.GetDistance(true) > 3)
                    {
                        if (_shotCool <= 0)
                        {;
                            ai.actor.Transition(TransitionCondition.StopMove);
                            if (ai.actor.Transition(TransitionCondition.Attack))
                            {
                                _state = InnerState.Shot;
                                _shotCool = ai.shotCoolTime;
                            }
                        }
                        else
                        {
                            _state = InnerState.KeepDistance;
                        }
                    }
                    else
                    {
                        
                        if(Random.value >= ai.backStep)
                        {
                            if (_shotCool <= 0)
                            {
                                ai.actor.Transition(TransitionCondition.StopMove);
                                if (ai.actor.Transition(TransitionCondition.Attack))
                                {
                                    _state = InnerState.Shot;
                                    _shotCool = ai.shotCoolTime;
                                }
                            }
                            else
                            {
                                _state = InnerState.KeepDistance;
                            }
                        }
                        else
                        {
                            if (_backStepCool <= 0)
                            {
                                bool result;
                                ai.actor.Transition(TransitionCondition.StopMove);
                                if (ai.transform.position.x < ai.target.transform.position.x)
                                    result = ai.actor.Transition(TransitionCondition.BackStepLeft);
                                else
                                    result = ai.actor.Transition(TransitionCondition.BackStepRight);
                                if (result)
                                {
                                    _state = InnerState.BackStep;
                                    _backStepCool = ai.backStepCoolTime;
                                }
                            }
                            else
                            {
                                _state = InnerState.KeepDistance;
                            }
                        }
                    }
                    if(_state == InnerState.KeepDistance)
                    {
                        _timeCheck = Random.Range(ai.keepDistanceTimeLength.x, ai.keepDistanceTimeLength.y);
                    }
                    break;
                case InnerState.Shot:

                    if (ai.IsActionEnd)
                    {
                        _state = InnerState.Rest;
                        _timeCheck = 0.5f;
                    }
                    break;
                case InnerState.BackStep:

                    if (ai.IsActionEnd)
                    {
                        ai.actor.Transition(TransitionCondition.StopMove);
                        if (ai.actor.Transition(TransitionCondition.Attack))
                        {
                            _state = InnerState.Shot;
                        }
                        else
                        {
                            _state = InnerState.CheckDistance;
                        }
                    }

                    break;
                case InnerState.KeepDistance:

                    ai.isLookRight = ai.transform.position.x < ai.target.transform.position.x;
                    bool moveRight = ai.isLookRight;

                    if (ai.GetDistance() != ai.attackDistance)
                    {
                        if (ai.GetDistance() < ai.attackDistance)
                        {
                            moveRight = !moveRight;
                        }

                        if (!ai.CheckMovable(moveRight))
                        {
                            _state = InnerState.Wait;
                        }
                        else
                        {
                            ai.actor.Transition(moveRight ? TransitionCondition.RightMove : TransitionCondition.LeftMove);
                        }
                    }
                    else
                    {
                        ai.actor.Transition(TransitionCondition.StopMove);
                    }

                    if (_timeCheck <= 0)
                        _state = InnerState.CheckDistance;

                    break;
                case InnerState.Wait:

                    ai.actor.Transition(TransitionCondition.StopMove);

                    if (_timeCheck <= 0)
                        _state = InnerState.CheckDistance;

                    break;
                case InnerState.Rest:

                    ai.actor.Transition(TransitionCondition.StopMove);

                    if (_timeCheck <= 0)
                    {
                        _state = InnerState.CheckDistance;
                    }

                    break;
            }

            _shotCool -= GameManager.instance.timeMng.DeltaTime;
            _backStepCool -= GameManager.instance.timeMng.DeltaTime;
            _timeCheck -= GameManager.instance.timeMng.DeltaTime;
        }
    }
}