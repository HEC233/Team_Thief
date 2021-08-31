using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class BossAlterActor : MonoBehaviour, IActor
{
    private BossAlterUnit unit;
    [SerializeField]
    private AnimationCtrl animCtrl;
    [SerializeField]
    private WwiseSoundCtrl wWsieSoundCtrl;

    private int wave = 1;
    [SerializeField]
    private List<SpawnData> wave1Data;
    [SerializeField]
    private List<SpawnData> wave2Data;
    [SerializeField]
    private List<SpawnData> wave3Data;

    private List<GameObject> spawnedMonsters = new List<GameObject>();
    private bool bSpawning = false;
    Coroutine EnemyDeadCheck;

    public float lightPillarInterval;
    public float lightExplosionInterval;
    public Damage pillarDamage;
    public Damage explosionDamage;

    private void Awake()
    {
        unit = GetComponentInParent<BossAlterUnit>();
        Assert.IsNotNull(unit);
        unit.hitEvent.AddListener(HitTransition);
        unit.dieEvent.AddListener(DieTransition);
    }

    private void Start()
    {
        StopAllCoroutines();
        unit.SetInvincibility(true);
        wave = 1;
        bSpawning = false;
    }

    public LightPillarPattern[] lightPillarPatterns;

    private IEnumerator LightPillarAttack(int index)
    {
        var pattern = lightPillarPatterns[index];
        int count = 0;
        bool[] actived = new bool[pattern.times.Length];
        float timeCheck = 0;

        while(count < pattern.times.Length)
        {
            for(int i = 0; i < pattern.times.Length; i++)
            {
                if(!actived[i] && pattern.times[i] < timeCheck)
                {
                    GameManager.instance.FX.Play("BossAttack1", pattern.positions[i].position);
                    StartCoroutine(Pattern1Attck(pattern.positions[i].position.x));
                    count++;
                    actived[i] = true;
                }
            }

            timeCheck += GameManager.instance.TimeMng.DeltaTime;
            yield return null;
        }
    }

    private IEnumerator Pattern1Attck(float x)
    {
        yield return new WaitForSeconds(1);

        var player = GameManager.instance.PlayerActor.GetUnit();
        if (player.tag != "Player")
            yield break;

        if (Mathf.Abs(player.transform.position.x - x) < 3)
        {
            player.HandleHit(pillarDamage);
        }
    }
    private IEnumerator Pattern2Attck(Vector2 center)
    {
        yield return new WaitForSeconds(2);

        var player = GameManager.instance.PlayerActor.GetUnit();
        if (player.tag != "Player")
            yield break;

        if ((center - new Vector2(player.transform.position.x, player.transform.position.y)).sqrMagnitude < 5)
        {
            player.HandleHit(explosionDamage);
        }
    }

    private IEnumerator Pattern1Coroutine()
    {
        //WwiseSoundManager.instance.ChangeBGMState("Boss_Phase", "Phase2");
        GameObject.Find("GameEventSystem")?.GetComponent<GameEventSystem>()?.AddQueue("BOSS_PHASE2");

        float timeCheck = 0;
        while (true)
        {
            if (timeCheck > lightPillarInterval)
            {
                StartCoroutine(LightPillarAttack(Random.Range(0, lightPillarPatterns.Length)));
                timeCheck = 0;
            }
            timeCheck += GameManager.instance.TimeMng.DeltaTime;
            yield return null;
        }
    }

    private IEnumerator Pattern2Coroutine()
    {
        GameObject.Find("GameEventSystem")?.GetComponent<GameEventSystem>()?.AddQueue("BOSS_PHASE3");
        float timeCheck = 0;
        while(true)
        {
            if (timeCheck > lightExplosionInterval)
            {
                GameManager.instance.FX.Play("BossAttack2", this.transform.position + Vector3.up * 4.5f);
                StartCoroutine(Pattern2Attck(this.transform.position + Vector3.up * 4.5f));
                timeCheck = 0;
            }
            timeCheck += GameManager.instance.TimeMng.DeltaTime;
            yield return null;
        }
    }

    private void Spawn()
    {
        List<SpawnData> spawnData = new List<SpawnData>();
        switch (wave)
        {
            case 1:
                spawnData = wave1Data;
                break;
            case 2:
                spawnData = wave2Data;
                break;
            case 3:
                spawnData = wave3Data;
                break;
        }
        foreach (var data in spawnData)
        {
            StartCoroutine(SpawnCoroutine(data));
        }
        if(spawnData.Count == 0)
        {
            SetBossAttackable();
        }
    }

    private IEnumerator SpawnCoroutine(SpawnData data)
    {
        int curCount = 0;
        float timeCheck = 0;

        while (curCount < data.count)
        {
            bSpawning = true;
            if (timeCheck >= data.enterDelay + data.interval * curCount)
            {
                spawnedMonsters.Add(GameManager.instance.Spawner.Spawn(data.unitName, data.position.position));
                curCount++;
            }

            timeCheck += GameManager.instance.TimeMng.DeltaTime;

            yield return null;
        }
        bSpawning = false;
        if (EnemyDeadCheck == null)
        {
            EnemyDeadCheck = StartCoroutine(EnemyDeadCheckCoroutine());
        }
    }

    private IEnumerator EnemyDeadCheckCoroutine()
    {
        while (bSpawning)
        {
            yield return null;
        }

        while(spawnedMonsters.Count != 0)
        {
            for(int i = spawnedMonsters.Count - 1; i >= 0; i--)
            {
                if(spawnedMonsters[i] == null)
                {
                    spawnedMonsters.RemoveAt(i);
                }
            }

            yield return null;
        }

        SetBossAttackable();
        EnemyDeadCheck = null;
    }

    private void SetBossAttackable()
    {
        if (wave == 3)
        {
            GameManager.instance.UIMng.SetBossHPColor(Color.red, Color.gray);
        }
        else
        {
            GameManager.instance.UIMng.SetBossHPColor(Color.blue, Color.red);
        }

        GameManager.instance.UIMng.InitBossHP(unit);
        GameManager.instance.UIMng.BossHPUpdate();
        unit.SetInvincibility(false);
    }

    public Unit GetUnit()
    {
        return unit;
    }

    public bool Transition(TransitionCondition condition, object param = null)
    {
        if (!this.gameObject.activeSelf)
            return false;
        switch (condition)
        {
            case TransitionCondition.BossAwake:
                unit.SetInvincibility(true);
                Spawn();
                break;
            case TransitionCondition.Hit:
                GameManager.instance.UIMng.BossHPUpdate();
                animCtrl.PlayAni(AniState.Hit);
                StartCoroutine(IdleAnimation());
                break;
            case TransitionCondition.Die:
                GameObject.Find("GameEventSystem")?.GetComponent<GameEventSystem>()?.AddQueue(unit.GetUnitName());
                GameManager.instance.UIMng.BossDie();
                unit.SetInvincibility(true);
                animCtrl.PlayAni(AniState.Die);
                break;
        }


        return false;
    }

    IEnumerator IdleAnimation()
    {
        yield return new WaitForSeconds(0.21f);
        animCtrl.PlayAni(AniState.Idle);
    }

    public void HitTransition()
    {
        Transition(TransitionCondition.Hit);
    }

    public void DieTransition()
    {
        wave++;

        if (3 < wave)
        {
            Transition(TransitionCondition.Die);
            GameObject.Find("GameEventSystem")?.GetComponent<GameEventSystem>()?.AddQueue("DEAD");
            StopAllCoroutines();
        }
        else
        {
            unit.ResetHP();
            GameManager.instance.UIMng.BossDie();
            unit.SetInvincibility(true);
            Spawn();

            if (wave == 2)
            {
                StartCoroutine(Pattern1Coroutine());
            }

            if (wave == 3)
            {
                StartCoroutine(Pattern2Coroutine());
            }
        }
    }
}


[System.Serializable]
public struct SpawnData
{
    public Transform position;
    public string unitName;
    public int count;
    public float enterDelay;
    public float interval;
}


[System.Serializable]
public struct LightPillarPattern
{
    public Transform[] positions;
    public float[] times;
}
