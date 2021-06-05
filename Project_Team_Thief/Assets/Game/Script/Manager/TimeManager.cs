using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Events;

public class TimeManager : MonoBehaviour
{
    // 단지 TimeScale 값만 넘기는 이벤트로 합칠 수 있을 것 같지만,
    // Hitstop과 bulletTime이 같이 발동 할 수 있을 것 같다는 가능성이 있어서 나눔.
    public event UnityAction<float> startHitstopEvent;
    public event UnityAction<float> endHitstopEvent;
    public event UnityAction<float> startBulletTimeEvent;
    public event UnityAction<float> endBulletTimeEvent;

    private bool _isBulletTime = false;

    public bool IsBulletTime => _isBulletTime;

    private bool _isHitStop = false;

    public bool IsHitStop => _isHitStop;

    [SerializeField]
    private float _timeScale = 1;
    public float TimeScale => _timeScale;

    [SerializeField]
    private float _prevTimeScale = 0;

    public float DeltaTime { get { return Time.deltaTime * _timeScale; } }
    public float FixedDeltaTime { get { return Time.fixedDeltaTime * _timeScale; } }

    private void Update()
    {
        // Test Code
        if (Input.GetKeyDown(KeyCode.Alpha1))
            BulletTime(0.2f, 0.3f);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            HitStop(1.0f);
    }

    public void BulletTime(float timeScale, float time)
    {
        _isBulletTime = true;
        _timeScale = timeScale;
        StartCoroutine(BulletTimeTimerCoroutine(time));
    }

    public void BulletTime(string unitName)
    {

    }

    public void UnbindAll()
    {
        Debug.Log("UNBIND ALL");
        startHitstopEvent = null;
        endHitstopEvent = null;
        startBulletTimeEvent = null;
        endBulletTimeEvent = null;
    }

    public void Reset()
    {
        endHitstopEvent?.Invoke(1.0f);
        endBulletTimeEvent?.Invoke(1.0f);
        ResetTime();
    }

    private bool _isStoped = false;
    public bool IsTimeStopped => _isStoped;
    private int _timeStopRequiredCount = 0;
    public void ResetTime()
    {
        _timeStopRequiredCount = 0;
        Debug.Log("timeStop require reset : " + _timeStopRequiredCount);
        _timeScale = 1;
        _prevTimeScale = 1;
    }
    public void StopTime()
    {

        _timeStopRequiredCount++;
        Debug.Log("timeStop required : " + _timeStopRequiredCount);
        if (_isStoped)
        {
            return;
        }
        startHitstopEvent?.Invoke(0);
        _isStoped = true;
    }
    public void ResumeTime()
    {
        _timeStopRequiredCount--;
        Debug.Log("timeResume required : " + _timeStopRequiredCount);
        if (_timeStopRequiredCount > 0)
        {
            Debug.Log("현재 이 부분때문에 정상 진행이 안됨 임시로 리턴 품");
            // 증가는 하는데 감소는 하지 않음 esc 연타시.
            return;
        }

        endHitstopEvent?.Invoke(1);
        _isStoped = false;
    }

    // 만약 공중에 있을 때 Hitstop이 걸린다면?? 우째??
    // 공격 간 시간을 허용해주는 타임은 이걸 적용 해야하나?
    // 리지드 바디의 프리즈를 이용해서 잠깐 폭력 멈춰두는건?
    public void HitStop(float time)
    {
        if(_isStoped == true)
            return;

        // 불릿 타임 중 히트 스탑이 호출 될 경우 히트 스탑이 끝난 뒤 불릿 타임으로 돌아가기 위해.
        if(_isBulletTime == true)
        {
            _prevTimeScale = _timeScale;
        }
        
        _isHitStop = true;
        _timeScale = 0;
        StartCoroutine(HitStopTimerCoroutine(time));
    }

    public void HitStop(string unitName)
    {

    }

    // 코루틴을 나눈 것 또한 독자적인 시간 카운팅이 필요할 가능성이 높아 보이기 때문.
    IEnumerator BulletTimeTimerCoroutine(float time)
    {
        yield return null; // 피격 FX 1프레임 출력을 위한 대기
        
        startBulletTimeEvent?.Invoke(_timeScale);

        float tick = 0.0f;

        while(tick <= time)
        {
            // 히트 스탑을 포함하기 위함.
            if (_isHitStop == false)
            {
                tick += Time.fixedDeltaTime;
            }

            yield return new WaitForFixedUpdate();
        }

        _isBulletTime = false;
        _timeScale = 1;
        endBulletTimeEvent?.Invoke(_timeScale);
    }

    //public delegate bool ReadyCheckFunc();
    //public List<ReadyCheckFunc> hitStopReadyCheckList = new List<ReadyCheckFunc>();

    IEnumerator HitStopTimerCoroutine(float time)
    {
        //bool isReady = false;
        //while(!isReady)
        //{
        //    isReady = true;
        //    foreach(var func in hitStopReadyCheckList)
        //    {
        //        if (!func())
        //            isReady = false;
        //    }
        //    yield return null;
        //}
        //hitStopReadyCheckList.Clear();

        yield return null;  // 피격 FX 1프레임 출력을 위한 대기
        yield return null;  // 피격 FX 1프레임 출력을 위한 대기

        startHitstopEvent?.Invoke(_timeScale);
        float tick = 0.0f;
        float prevTimeScale = _timeScale;
        while (tick <= time)
        {
            if (GameManager.instance.GameState != GameManager.GameStateEnum.Pause)
            {
                tick += Time.fixedDeltaTime;
            }

            yield return new WaitForFixedUpdate();
        }
        
        _isHitStop = false;

        if (_isBulletTime == true)
        {
            _timeScale = _prevTimeScale;
            endHitstopEvent?.Invoke(_timeScale);
        }
        else
        {
            _timeScale = 1;
            endHitstopEvent?.Invoke(_timeScale);
        }

    }
}
