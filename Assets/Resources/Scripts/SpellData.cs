using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpellData", menuName = "Scriptable Ojbect/SpellData")]
public class SpellData : ScriptableObject
{
    [Header("포커스를 사용하는 지 여부")]
    public bool isFocus;
    public Color focusColor; //포커스에서 보여줄 색상

    [Header("스펠 아이콘")]
    public Sprite spellIcon;

    [Header("스펠 이름")]
    public string spellName;

    public enum SpellType
    {
        Creature, Weapon
    }
    [Header("스펠 타입")]
    public SpellType spellType;


    [Header("스펠 비용")]
    public int spellValue;

    [Header("스펠 설명")]
    [TextArea]
    public string spellDesc;

    [Header("스펠 프레팹")]
    public GameObject spellPrefab;
}
