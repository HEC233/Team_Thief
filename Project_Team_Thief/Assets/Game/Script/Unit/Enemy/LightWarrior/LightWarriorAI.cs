//#define TEST

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LWAIState;
using PS.Util.Tile;

public class LightWarriorAI : MonoBehaviour
{
    public LayerMask unMovableLayer;
    public bool isLookRight;

    public Unit target;
    private AIState _curState;
    public IActor actor;

    private Vector2Int _myCoord;
    private Vector2Int _targetCoord;

    public Search search = new Search();
    public Combat combat = new Combat();

    [Header("Sight")]
    [SerializeField] private int frontView;
    [SerializeField] private int rearView;
    [SerializeField] private int upperView;
    [SerializeField] private int underView;

    [Header("Movement")]
    [SerializeField] public Vector2 IdleTimeLength = Vector2.zero;
    [SerializeField] public Vector2 walkTimeLength = Vector2.zero;

    [Header("Combat")]
    [SerializeField] public int attackDistance;
    [SerializeField] public Vector2 followTimeLength = Vector2.zero;


#if (TEST)
    public TestColor color;
#endif

    private void Start()
    {
        isLookRight = true;
        actor = GetComponent<LightWarriorActor>();
        _curState = search;
        _curState.Enter(this);

        if(GameManager.instance.GetControlActor() != null)
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

        Debug.Log("이동 가능 한지의 여부 " + result);
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

namespace LWAIState
{
    public abstract class AIState
    {
        public abstract void Enter(LightWarriorAI ai);
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
        LightWarriorAI ai;
        private Vector2Int _lastCoord;
        private Vector2Int _curCoord;

        public override void Enter(LightWarriorAI ai)
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
            _timeCheck -= Time.deltaTime;
            if(_timeCheck <= 0)
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
#if (TEST)
            ai.color.Set(Color.blue);
#endif
            ai.actor.Transition(ai.isLookRight ? TransitionCondition.RightMove : TransitionCondition.LeftMove);
        }

        private void Stop()
        {
            ai.actor.Transition(TransitionCondition.StopMove);
#if (TEST)
            ai.color.Set(Color.white);
#endif
    }
}

    /*
     * 전투 상태!
     */
    public class Combat : AIState
    {
        LightWarriorAI ai;

        private float _AttackCool;
        private float _timeCheck;
        private Vector2Int _lastCoord;
        private Vector2Int _curCoord;

        private enum InnerState
        {
            Attack,Move,Wait,Reset
        }
        private InnerState _state;

        public override void Enter(LightWarriorAI ai)
        {
            this.ai = ai;
            _AttackCool = 0;
            _timeCheck = 0;
            _state = InnerState.Attack;
            _lastCoord = ai.transform.TileCoord();
#if TEST
            ai.color.Set(Color.red);
#endif
        }
        public override void Exit()
        {

#if TEST
            ai.color.Set(Color.white);
#endif
        }
        public override void Process()
        {
            switch (_state)
            {
                // 스킬 사용 후 스킬이 끝날때까지 기다려주는 처리 필요
                //-----------------------------------------
                case InnerState.Attack:
                    if (ai.GetDistance() > ai.attackDistance)
                    {

                    }
                    else
                    {
                        if (_AttackCool <= 0)
                        {
                            ai.actor.Transition(TransitionCondition.StopMove);
                            _AttackCool = 2.0f;
#if TEST
                            ai.color.Set(Color.magenta);
#endif
                            //ai.actor.Transition(ai.GetDistance(false) > 0 ? TransitionCondition.SetAttackBoxRight : TransitionCondition.SetAttackBoxLeft);
                            ai.actor.Transition(TransitionCondition.Attack);

                            _state = InnerState.Reset;
                            _timeCheck = 0.5f;
                            break;
                        }
                    }
                    _state = InnerState.Move;
                    _timeCheck = Random.Range(ai.followTimeLength.x, ai.followTimeLength.y);
                    break;
                //-----------------------------------------
                case InnerState.Move:
                    ai.isLookRight = ai.transform.position.x < ai.target.transform.position.x;

                    //_curCoord = ai.transform.TileCoord();
                    //if (_lastCoord != _curCoord)
                    //{
                    //    _lastCoord = _curCoord;
                    if (!ai.CheckMovable(ai.isLookRight))
                    {
                        _state = InnerState.Wait;
                    }
                    //}
                    else if (ai.GetDistance(true) > 1)
                    {
                        ai.actor.Transition(ai.isLookRight ? TransitionCondition.RightMove : TransitionCondition.LeftMove);
                    }
                    else
                    {
                        ai.actor.Transition(TransitionCondition.StopMove);
                    }

                    if (_timeCheck <= 0)
                    {
                        if (!ai.CheckSight())
                            ai.ChangeState(ai.search);
                        else
                            _state = InnerState.Attack;
                    }
                    break;
                //-----------------------------------------
                case InnerState.Wait:

                    ai.actor.Transition(TransitionCondition.StopMove);

                    if (_timeCheck <= 0)
                    {
                        _state = InnerState.Move;
                    }
                    break;
                //-----------------------------------------
                case InnerState.Reset:

                    ai.actor.Transition(TransitionCondition.Idle);

                    if (_timeCheck <= 0)
                    {
#if TEST
                        ai.color.Set(Color.red);
#endif

                        if (!ai.CheckSight())
                            ai.ChangeState(ai.search);
                        else
                            _state = InnerState.Attack;
                    }
                    break;
                 //-----------------------------------------
            }

            _AttackCool -= Time.deltaTime;
            _timeCheck -= Time.deltaTime;

        }
    }
}
