using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LWAIState;

public class LightWarriorAI : MonoBehaviour
{
    public LayerMask layerMask;

    public Unit target;
    private AIState _curState;
    public IActor actor;

    private Vector2Int tileCoord;

    public Search search = new Search();
    public Combat combat = new Combat();

    private void Start()
    {
        actor = GetComponent<LightWarriorActor>();
        _curState = search;
        _curState.Enter(this);
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

    public bool CheckSight()
    {
        tileCoord = transform.position.TileCoord();
        return false;
    }

    // 움직일 수 있는 지 판별
    public bool CheckMovable(Direction direction)
    {
        Vector2Int nowTileCoord = transform.position.TileCoord();
        bool result;
        if (direction == Direction.right)
        {
            result = (
                !nowTileCoord.CheckObjectExist(layerMask, 1, 1) &
                !nowTileCoord.CheckObjectExist(layerMask, 1, 0) &
                nowTileCoord.CheckObjectExist(layerMask, 1, -1));
        }
        else
        {
            result = (
                !nowTileCoord.CheckObjectExist(layerMask, -1, 1) &
                !nowTileCoord.CheckObjectExist(layerMask, -1, 0) &
                nowTileCoord.CheckObjectExist(layerMask, -1, -1));
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
    public enum Direction
    {
        right, left
    }

    public abstract class AIState
    {
        public abstract void Enter(LightWarriorAI ai);
        public abstract void Process();
        public abstract void Exit();
    }

    public class Search : AIState
    {
        float timeCheck;
        Direction curDir;
        bool isMoving;
        LightWarriorAI ai;

        public override void Enter(LightWarriorAI ai)
        {
            timeCheck = 0;
            isMoving = false;
            this.ai = ai;
        }
        public override void Exit()
        {
        }
        public override void Process()
        {
            timeCheck -= Time.deltaTime;
            if(timeCheck <= 0)
            {
                if (Random.value > 0.5f)
                {
                    isMoving = true;
                    if (Random.value > 0.5f)
                        curDir = Direction.right;
                    else
                        curDir = Direction.left;
                    timeCheck = Random.Range(1, 3);
                }
                else
                {
                    isMoving = false;
                    timeCheck = Random.Range(1, 5);
                }
            }

            if (isMoving)
                Move();
            else
                Idle();

            if (ai.CheckSight())
                ai.ChangeState(ai.combat);
        }

        private void Move()
        {
            if(!ai.CheckMovable(curDir))
            {
                curDir = (curDir == Direction.right ? Direction.left : Direction.right);
            }

            ai.actor.Transition(curDir == Direction.right ? TransitionCondition.RightMove : TransitionCondition.LeftMove);
        }

        private void Idle()
        {
            ai.actor.Transition(TransitionCondition.Idle);
        }
    }

    public class Combat : AIState
    {
        LightWarriorAI ai;
        public override void Enter(LightWarriorAI ai)
        {
            this.ai = ai;
        }
        public override void Exit()
        {
        }
        public override void Process()
        {
            if (!ai.CheckSight())
                ai.ChangeState(ai.combat);
        }
    }
}
