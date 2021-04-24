using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSkillMgr : MonoBehaviour
{
    private Queue<GameSkillObject> _skillObjectQueue = new Queue<GameSkillObject>();

    private bool ReserveQueue(int count)
    {
        for (int i = 0; i < count; ++i)
        {
            var skillObj = CreateSkillObject();
            if (skillObj == null)
                continue;

            skillObj.transform.SetParent(this.transform);
            skillObj.gameObject.SetActive(false);

            _skillObjectQueue.Enqueue(skillObj);
        }

        return true;
    }

    private GameSkillObject CreateSkillObject()
    {
        GameObject obj = new GameObject("skillObject");
        var skillObject = obj.AddComponent<GameSkillObject>();

        return skillObject;
    }

    public GameSkillObject GetSkillObject()
    {
        if (_skillObjectQueue.Count <= 0)
        {
            if (ReserveQueue(5) == false)
                return null;
        }

        var skillObject = _skillObjectQueue.Dequeue();
        skillObject.gameObject.SetActive(true);
        return skillObject;
    }

    public void EnqueueSkillObject(GameSkillObject gameSkillObject)
    {
        gameSkillObject.gameObject.SetActive(false);
        _skillObjectQueue.Enqueue(gameSkillObject);
    }
}
