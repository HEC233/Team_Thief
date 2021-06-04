using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightWarriorDeath : MonoBehaviour
{
    private void OnDestroy()
    {
        GameObject.Find("GameEventSystem")?.GetComponent<GameEventSystem>().AddQueue("TUTORIAL_LIGHTWARRIOR");
    }
}
