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

    [Header("½ºÆç Å¸ÀÔ")]
    public SpellType singleType;

    [Header("½ºÆç ¾ÆÀÌÄÜ")]
    public Sprite spellIcon;

    [Header("½ºÆç ºñ¿ë")]
    public int spellValue;

    [Header("½ºÆç ¼³¸í")]
    [TextArea]
    public string spellDesc;

    [Header("½ºÆç ÇÁ·¹ÆÕ")]
    public GameObject spellPrefab;


}
