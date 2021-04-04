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
        Dictionary<string, List<GameObject>> effectPool = new Dictionary<string, List<GameObject>>();
        Dictionary<string, List<GameObject>> activeEffectPool = new Dictionary<string, List<GameObject>>();

        private void Start()
        {
            foreach(var fx in effects)
            {
                effectPool.Add(fx.name, new List<GameObject>());
                activeEffectPool.Add(fx.name, new List<GameObject>());
            }

            var timeMng = GameManager.instance.timeMng;
            if (timeMng)
            {
                timeMng.startBulletTimeEvent += TimeScaleChangeCallback;
                timeMng.endBulletTimeEvent += TimeScaleChangeCallback;
                timeMng.startHitstopEvent += TimeScaleChangeCallback;
                timeMng.endHitstopEvent += TimeScaleChangeCallback;
            }

            StartCoroutine(EndEffectChecker());
        }

        private void TimeScaleChangeCallback(float customTimeScale)
        {
            foreach (var e in effects)
            {
                var checkPool = activeEffectPool[e.name];
                foreach(var efx in checkPool)
                {
                    efx.GetComponent<EffectController>().SetSpeed(customTimeScale);
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
            GameObject effect = null;

            for(int i = 0; i < pool.Count; i++)
            {
                if(!pool[i].activeSelf)
                {
                    effect = pool[i];
                }
                pool.Remove(effect);
            }
            if(effect == null)
            {
                foreach(var e in effects)
                {
                    if (e.name == effectName)
                    {
                        effect = Instantiate(e.prefab, transform);
                        break;
                    }
                }
            }

            var particle = effect.GetComponent<EffectController>();
            if (particle)
            {
                effect.SetActive(true);
                effect.transform.position = position;
                effect.transform.rotation = quaternion;
                
                particle.Play();

                activeEffectPool[effectName].Add(effect);

                return particle;
            }

            return null;
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
                        if(checkPool[iter].GetComponent<EffectController>().isStopped)
                        {
                            checkPool[iter].SetActive(false);
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