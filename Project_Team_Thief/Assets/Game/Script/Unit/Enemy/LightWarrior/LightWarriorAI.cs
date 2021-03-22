#define TEST

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LWAIState;

public class LightWarriorAI : MonoBehaviour
{
    public LayerMask groundLayer;
    public bool isLookRight;

    public Unit target;
    private AIState _curState;
    public IActor actor;

    private Vector2Int _myCoord;
    private Vector2Int _targetCoord;

    public Search search = new Search();
    public Combat combat = new Combat();

#if (TEST)
    public TestColor color;
#endif

    private void Start()
    {
        isLookRight = true;
        actor = GetComponent<LightWarriorActor>();
        _curState = search;
        _curState.Enter(this);
#if (TEST)
        color = GetComponentInParent<LightWarriorUnit>().GetComponentInChildren<TestColor>();
#endif
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

        if (viewBox.x >= -5 && viewBox.x <= 10 && viewBox.y <= 3 && viewBox.y >= -2)
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
                Debug.DrawLine((_myCoord + new Vector2Int(-5, 3)).TileCoordToPosition3(), (_myCoord + new Vector2Int(-5, -2)).TileCoordToPosition3(), Color.red);
                Debug.DrawLine((_myCoord + new Vector2Int(-5, -2)).TileCoordToPosition3(), (_myCoord + new Vector2Int(10, -2)).TileCoordToPosition3(), Color.red);
                Debug.DrawLine((_myCoord + new Vector2Int(10, -2)).TileCoordToPosition3(), (_myCoord + new Vector2Int(10, 3)).TileCoordToPosition3(), Color.red);
                Debug.DrawLine((_myCoord + new Vector2Int(10, 3)).TileCoordToPosition3(), (_myCoord + new Vector2Int(-5, 3)).TileCoordToPosition3(), Color.red);
            }
            else
            {
                Debug.DrawLine((_myCoord + new Vector2Int(5, 3)).TileCoordToPosition3(), (_myCoord + new Vector2Int(5, -2)).TileCoordToPosition3(), Color.red);
                Debug.DrawLine((_myCoord + new Vector2Int(5, -2)).TileCoordToPosition3(), (_myCoord + new Vector2Int(-10, -2)).TileCoordToPosition3(), Color.red);
                Debug.DrawLine((_myCoord + new Vector2Int(-10, -2)).TileCoordToPosition3(), (_myCoord + new Vector2Int(-10, 3)).TileCoordToPosition3(), Color.red);
                Debug.DrawLine((_myCoord + new Vector2Int(-10, 3)).TileCoordToPosition3(), (_myCoord + new Vector2Int(5, 3)).TileCoordToPosition3(), Color.red);
            }
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
                !nowTileCoord.CheckObjectExist(groundLayer, 1, 1) &
                !nowTileCoord.CheckObjectExist(groundLayer, 1, 0) &
                nowTileCoord.CheckObjectExist(groundLayer, 1, -1));
        }
        else
        {
            result = (
                !nowTileCoord.CheckObjectExist(groundLayer, -1, 1) &
                !nowTileCoord.CheckObjectExist(groundLayer, -1, 0) &
                nowTileCoord.CheckObjectExist(groundLayer, -1, -1));
        }

        Debug.Log("이동 가능 한지의 여부 " + result);
        return result;
    }

    // ai와 타겟유닛 사이의 거리를 반환
    public int GetDistance()
    {
        if (target != null)
        {
            return Mathf.Abs(target.transform.TileCoord().x - transform.TileCoord().x);
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

        public override void Enter(LightWarriorAI ai)
        {
            _timeCheck = 0;
            _isMoving = false;
            this.ai = ai;
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
                    _timeCheck = Random.Range(1, 3);
                }
                else
                {
                    _isMoving = false;
                    _timeCheck = Random.Range(1, 5);
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
            if (!ai.CheckMovable(ai.isLookRight))
            {
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

        private float _jumpAttackCool;
        private float _swingAttackCool;
        private float _timeCheck;

        private enum InnerState
        {
            Attack,Move,Wait,Reset
        }
        private InnerState _state;

        public override void Enter(LightWarriorAI ai)
        {
            this.ai = ai;
            _jumpAttackCool = 0;
            _swingAttackCool = 0;
            _timeCheck = 0;
            _state = InnerState.Attack;
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
                    if (ai.GetDistance() > 5)
                    {
                        if (_jumpAttackCool <= 0)
                        {
                            _jumpAttackCool = 2.0f;
#if TEST
                            ai.color.Set(Color.magenta);
#endif
                            Debug.Log("광전사 점프 어택 발생");
                            /*
                             * 여기서 전이조건을 넘겨줄 것이다.
                             * 그런데 특정 유닛만 콕 집어서 공격하고 싶다면 어떻게 해야할까?
                             * 플레이어와 적 유닛이 겹쳐있을때 적유닛의 공격에 적유닛도 피해를 입는것은 광역기가 아닌 이상 이상할 것이다.
                             * 그렇다면, 공격하고 싶은 유닛만 선택해 줘야하는데 이걸 전이조건으로 일일히 넘겨주는 것은 사실상 불가능하다.
                             * 그렇기에 어느 유닛을 공격하고 싶은것인지 인수(패러미터)넘겨주어야 할것 같은데 그럴려면 인터페이스를 수정해야 한다.
                             */
                            ai.actor.Transition(TransitionCondition.Skill1);

                            _state = InnerState.Reset;
                            _timeCheck = 0.5f;
                            break;
                        }
                    }
                    else
                    {
                        if (_swingAttackCool <= 0)
                        {
                            _swingAttackCool = 2.0f;
#if TEST
                            ai.color.Set(Color.magenta);
#endif
                            Debug.Log("광전사 스윙 어택 발생");
                            ai.actor.Transition(TransitionCondition.Skill2);

                            _state = InnerState.Reset;
                            _timeCheck = 0.5f;
                            break;
                        }
                    }
                    _state = InnerState.Move;
                    _timeCheck = Random.Range(0.5f, 1.0f);
                    break;
                //-----------------------------------------
                case InnerState.Move:
                    ai.isLookRight = ai.transform.position.x < ai.target.transform.position.x;

                    if (!ai.CheckMovable(ai.isLookRight))
                        _state = InnerState.Wait;
                    else
                        ai.actor.Transition(ai.isLookRight ? TransitionCondition.RightMove : TransitionCondition.LeftMove);

                    if (_timeCheck <= 0)
                    {
                        _state = InnerState.Attack;
                        ai.actor.Transition(TransitionCondition.Idle);
                    }
                    break;
                //-----------------------------------------
                case InnerState.Wait:

                    ai.actor.Transition(TransitionCondition.Idle);

                    if (_timeCheck <= 0)
                    {
                        _state = InnerState.Attack;
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

            _jumpAttackCool -= Time.deltaTime;
            _swingAttackCool -= Time.deltaTime;
            _timeCheck -= Time.deltaTime;

        }
    }
}
