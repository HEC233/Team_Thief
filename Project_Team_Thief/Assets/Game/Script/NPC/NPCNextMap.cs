using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCNextMap : NPCController
{
    [SerializeField]
    private Sprite skillSprite;
    [SerializeField]
    private Sprite hpSprite;
    [SerializeField]
    private Sprite coinSprite;
    [SerializeField]
    private Sprite encroachSprite;

    private bool _acted = false;

    [SerializeField]
    private SpriteRenderer _leftSprite;
    [SerializeField]
    private SpriteRenderer _rightSprite;

    public override bool Act()
    {
        if(_acted)
        {
            _sendQueue = false;
            return false;
        }

        return true;
    }

    public void SetRewardType(NPCRewardType[] type)
    {
        if (type.Length < 2)
        {
            return;
        }

        switch (type[0])
        {
            case NPCRewardType.skill:
                _leftSprite.sprite = skillSprite;
                break;
            case NPCRewardType.hp:
                _leftSprite.sprite = hpSprite;
                break;
            case NPCRewardType.coin:
                _leftSprite.sprite = coinSprite;
                break;
            case NPCRewardType.encroachment:
                _leftSprite.sprite = encroachSprite;
                break;
        }
        switch (type[1])
        {
            case NPCRewardType.skill:
                _rightSprite.sprite = skillSprite;
                break;
            case NPCRewardType.hp:
                _rightSprite.sprite = hpSprite;
                break;
            case NPCRewardType.coin:
                _rightSprite.sprite = coinSprite;
                break;
            case NPCRewardType.encroachment:
                _rightSprite.sprite = encroachSprite;
                break;
        }
    }
}
