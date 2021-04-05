using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS.FX
{
    public class EffectController : MonoBehaviour
    {
        public List<ParticleSystem> particleSystems = new List<ParticleSystem>();

        //private void Start()
        //{
        //    Bind();
        //}

        //private void Bind()
        //{
        //    GameManager.instance.timeMng.startHitstopEvent += StartHitStopEvent;
        //    GameManager.instance.timeMng.startHitstopEvent += EndHitStopEvent;
        //    GameManager.instance.timeMng.startHitstopEvent += StartBulletTimeEvent;
        //    GameManager.instance.timeMng.startHitstopEvent += EndBulletTimeEvent;
        //}

        //private void UnBind()
        //{
        //    GameManager.instance.timeMng.startHitstopEvent -= StartHitStopEvent;
        //    GameManager.instance.timeMng.startHitstopEvent -= EndHitStopEvent;
        //    GameManager.instance.timeMng.startHitstopEvent -= StartBulletTimeEvent;
        //    GameManager.instance.timeMng.startHitstopEvent -= EndBulletTimeEvent;
        //}

        //private void OnDestroy()
        //{
        //    // 만약에 모종의 이유로 삭제 될 수도 있으니
        //    UnBind();
        //}

        public bool isStopped
        {
            get
            {
                bool value = true;
                foreach (var ps in particleSystems)
                {
                    if (!ps.isStopped)
                    {
                        value = false;
                        break;
                    }
                }
                return value;
            }
        }

        //public bool IsPlaying()
        //{
        //    bool value = true;
        //    foreach (var ps in particleSystems)
        //    {
        //        if (ps.particleCount == 0)
        //        {
        //            value = false;
        //            break;
        //        }
        //    }
        //    return value;
        //}

        public void Play()
        {
            foreach (var ps in particleSystems)
            {
                ps.Play();
            }
        }

        public void SetSpeed(float speed)
        {
            foreach (var ps in particleSystems)
            {
                var main = ps.main;
                main.simulationSpeed = speed;
            }
        }

        //private void StartHitStopEvent(float timeScale)
        //{
        //    SetSpeed(timeScale);
        //}

        //private void EndHitStopEvent(float timeScale)
        //{
        //    SetSpeed(timeScale);
        //}

        //private void StartBulletTimeEvent(float timeScale)
        //{
        //    SetSpeed(timeScale);
        //}

        //private void EndBulletTimeEvent(float timeScale)
        //{
        //    SetSpeed(timeScale);
        //}
        
    }
}