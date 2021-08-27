using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillMagicMissileData", menuName = "ScriptableObject/SkillMagicMissileData")]
public class SkillMagicMissileData : SkillDataBase
{
    [SerializeField] 
    private GameObject _magicMissileGO;
    public GameObject MagicMissileGO => _magicMissileGO;

    [SerializeField] 
    private float _spawnInterval;
    public float SpawnInterval => _spawnInterval;

    public override SkillControllerBase GetSkillController(GameSkillObject skillObject, Unit unit)
    {
        return new SkillMagicMissileController(skillObject, this, unit);
    }
}
