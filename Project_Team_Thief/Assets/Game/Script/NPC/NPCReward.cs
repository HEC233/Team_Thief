using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NPCReward : NPCController
{
    private bool _acted = false;
    private bool _acting = false;
    private NPCRewardType _type;

    [SerializeField]
    private Sprite[] _npcSprites = new Sprite[5];
    [SerializeField]
    private SpriteRenderer _spriteRenderer;

    private UnityEvent actEventer = new UnityEvent();
    public void AddActListener(UnityAction action)
    {
        actEventer.AddListener(action);
    }

    public void SetRewardType(NPCRewardType type)
    {
        _type = type;
        _spriteRenderer.sprite = _npcSprites[((int)type)];
    }

    public override bool Act()
    {
        if (_acted || _acting)
        {
            return false;
        }

        StartCoroutine(ActCoroutine());
        return true;
    }

    private IEnumerator ActCoroutine()
    {
        _acting = true;

        switch (_type)
        {
            case NPCRewardType.skill:
                yield return GameManager.instance.UIMng.SkillSelectCoroutine();
                break;

            case NPCRewardType.hp:

                break;

            case NPCRewardType.encroachment:

                break;

            case NPCRewardType.shop:

                break;

            case NPCRewardType.coin:

                break;
        }

        _acting = false;
        _acted = true;
        actEventer.Invoke();
        yield break;
    }
}
