using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LightWarriorUnit : MonsterUnit
{
    public override void HandleHit(in Damage inputDamage)
    {
        base.HandleHit(inputDamage);

        if (_hp <= 0)
        {
            dieEvent.Invoke();
        }
        else
        {
            if (GameManager.instance.FX)
            {
                string fxName = string.Empty;
                switch (inputDamage.additionalInfo)
                {
                    case 0:
                        fxName = "Hit1";
                        break;
                    case 1:
                        fxName = "Hit2";
                        break;
                    case 2:
                        fxName = "Hit3";
                        break;
                }
                var effect = GameManager.instance?.FX.Play(fxName, inputDamage.hitPosition);
                //GameManager.instance?.timeMng.hitStopReadyCheckList.Add(effect.IsPlaying);
            }

            if (GameManager.instance.shadow)
            {
                GameManager.instance.shadow.Burst(inputDamage.hitPosition, 10, 10, 5, true);
            }
            hitEvent.Invoke();
        }
    }
}
