using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitScript : MonoBehaviour
{
    float time;

    // Start is called before the first frame update
    void Start()
    {
        time = 0;
        GameManager.instance?.UIMng.ToggleUI(GameStateEnum.None);
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

        if(time > 5)
        {
            GameManager.instance?.ExitToMainMenu();
        }
    }
}
