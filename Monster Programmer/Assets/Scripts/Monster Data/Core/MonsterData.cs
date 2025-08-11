using Data;
using System.Collections.Generic;
using UnityEngine;


public enum MonsterType { Python, Java, CPlusPlus, None}
public enum Rarity { Common, Rare, Legendary }

public enum MapMonster
{
    Encapsulation,
    Inheritance,
    Abstraction,
    Polymorphism
}

[CreateAssetMenu(fileName = "MonsterData", menuName = "Scriptable Objects/MonsterData")]
public class MonsterData : ScriptableObject
{
    [Header("[Identity]")]
    public string monsterID;
    public string monsterName;
    public Sprite frontSpriteMonster;
    public Sprite backSpriteMonster;
    public MonsterType type;
    public Rarity rarity;

    [Header("[SpawnMap]")]
    public MapMonster spawnMap;

    [Header("[Status]")]
    public int attack;
    public int defense;
    public int speed;


    public List<Question> questionData = new List<Question>(); // List of questions for this monster
}
