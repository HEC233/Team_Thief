using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DandelionAI : MonoBehaviour
{
    public IActor actor;
    [SerializeField]
    private float waitTime;

    private float timeCheck = 0;

    void Start()
    {
        actor = GetComponent<DandelionActor>();
    }

    // Update is called once per frame
    void Update()
    {
        if(timeCheck >= waitTime)
        {
            actor.Transition(TransitionCondition.Attack);
            timeCheck = 0;
        }

        timeCheck += GameManager.instance.TimeMng.DeltaTime;
    }
}
