using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpellData", menuName = "Scriptable Ojbect/SpellData")]
public class SpellData : ScriptableObject
{
    

    public enum SpellType
    {
        Creature, Weapon
    }

    [Header("���� Ÿ��")]
    public SpellType singleType;

    [Header("���� ������")]
    public Sprite spellIcon;

    [Header("���� ���")]
    public int spellValue;

    [Header("���� ����")]
    [TextArea]
    public string spellDesc;

    [Header("���� ������")]
    public GameObject spellPrefab;


}
