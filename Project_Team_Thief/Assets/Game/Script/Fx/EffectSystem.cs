using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace PS.FX
{
    [Serializable]
    public struct Effect
    {
        public string name;
        public GameObject prefab;
    }


    public class EffectSystem : MonoBehaviour
    {
        public List<Effect> effects = new List<Effect>();
        Dictionary<string, List<EffectController>> effectPool = new Dictionary<string, List<EffectController>>();
        Dictionary<string, List<EffectController>> activeEffectPool = new Dictionary<string, List<EffectController>>();

        private void Start()
        {
            foreach(var fx in effects)
            {
                effectPool.Add(fx.name, new List<EffectController>());
                activeEffectPool.Add(fx.name, new List<EffectController>());
            }

            Bind();

            StartCoroutine(EndEffectChecker());
        }

        public void Bind()
        {
            var timeMng = GameManager.instance.TimeMng;
            if (timeMng)
            {
                timeMng.startBulletTimeEvent += TimeScaleChangeCallback;
                timeMng.endBulletTimeEvent += TimeScaleChangeCallback;
                timeMng.startHitstopEvent += TimeScaleChangeCallback;
                timeMng.endHitstopEvent += TimeScaleChangeCallback;
            }
        }

        private void TimeScaleChangeCallback(float customTimeScale)
        {
            foreach (var e in effects)
            {
                var checkPool = activeEffectPool[e.name];
                foreach(var efx in checkPool)
                {
                    efx.SetSpeed(customTimeScale);
                }
            }
        }


        public EffectController Play(string effectName, Vector3 position, Quaternion quaternion)
        {
            if(!effectPool.ContainsKey(effectName))
            {
                return null;
            }

            var pool = effectPool[effectName];
            EffectController effect = null;

            for(int i = 0; i < pool.Count; i++)
            {
                if(!pool[i].gameObject.activeSelf)
                {
                    effect = pool[i];
                    pool.Remove(effect);
                }
            }
            if(effect == null)
            {
                foreach(var e in effects)
                {
                    if (e.name == effectName)
                    {
                        effect = Instantiate(e.prefab, transform).GetComponent<EffectController>();
                        break;
                    }
                }
            }

            var particle = effect;
            if (particle)
            {
                effect.gameObject.SetActive(true);
                effect.transform.position = position;
                effect.transform.rotation = quaternion;
                
                particle.Play();

                activeEffectPool[effectName].Add(effect);
            }
            return particle;
        }

        public EffectController Play(string effectName, Vector3 position)
        {
            return Play(effectName, position, Quaternion.identity);
        }

        IEnumerator EndEffectChecker()
        {
            while(true)
            {
                foreach(var e in effects)
                {
                    var checkPool = activeEffectPool[e.name];
                    int iter = 0;
                    while(iter < checkPool.Count)
                    {
                        if(checkPool[iter].isStopped)
                        {
                            checkPool[iter].SetSpeed(1);
                            checkPool[iter].gameObject.SetActive(false);
                            effectPool[e.name].Add(checkPool[iter]);
                            checkPool.RemoveAt(iter);
                        }
                        else
                        {
                            iter++;
                        }
                    }
                }

                yield return new WaitForSeconds(1);
            }
        }
    }
}