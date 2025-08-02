using System.Collections.Generic;
using UnityEngine;


public enum MonsterType { Python, Java, CPlusPlus }
public enum Rarity { Common, Rare, Legendary }

[CreateAssetMenu(fileName = "MonsterData", menuName = "Scriptable Objects/MonsterData")]
public class MonsterData : ScriptableObject
{
    public string monsterID;
    public string monsterName;
    public Sprite frontSpriteMonster;
    public Sprite backSpriteMonster;
    public MonsterType type;
    public Rarity rarity;

    public int attack;
    public int defense;
    public int speed;


    public List<QuestionData> questionData = new List<QuestionData>(); // List of questions for this monster
}
