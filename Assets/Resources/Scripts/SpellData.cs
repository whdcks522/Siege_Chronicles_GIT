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

    [Header("Ω∫∆Á ≈∏¿‘")]
    public SpellType singleType;

    [Header("Ω∫∆Á æ∆¿Ãƒ‹")]
    public Sprite spellIcon;

    [Header("Ω∫∆Á ¿Ã∏ß")]
    public string spellName;

    [Header("Ω∫∆Á ∫ÒøÎ")]
    public int spellValue;

    [Header("Ω∫∆Á º≥∏Ì")]
    [TextArea]
    public string spellDesc;

    [Header("Ω∫∆Á «¡∑π∆’")]
    public GameObject spellPrefab;


}
