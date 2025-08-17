using Manager;
using Manager.Spawner;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MonsterController : MonoBehaviour, IInteractable
{
    [Header("[Setting]")]
    [SerializeField] private bool debugMode;
    [SerializeField] private bool iniiFromSpawner;

    [Header("[Reference]")]
    [SerializeField] private SpriteRenderer frontSpriteRenderer;
    [SerializeField] private MonsterData selectedMonsterEnemy;

    [Header("Rarity Weights")]
    [SerializeField] private float commonWeight = 60f;
    [SerializeField] private float rareWeight = 10f;
    [SerializeField] private float legendaryWeight = 5f;

    private void Start()
    {
        if (debugMode)
        {
            List<MonsterData> debugMonsters = new List<MonsterData>();
            debugMonsters.Add(selectedMonsterEnemy);
            
            GetRandomMonster(debugMonsters);
            return;
        }

        if (iniiFromSpawner)
            return;

        GetRandomMonster();
    }

    public void Interact()
    {
        Debug.Log("Interacting with monster: " + frontSpriteRenderer.sprite.name);
        GameData.Instance.SelectEnemyMonster(selectedMonsterEnemy.monsterID);

        //set data last world
        PlayerSpawnPoint.RepositionPlayer(PlayerController.Main.transform.position);
        FightSceneManager.LastWorld = SceneManager.GetActiveScene().name;

        //change scene
        SceneManager.LoadScene("FightScene");
        // Tambahkan logika interaksi di sini jika perlu
    }

    public void GetRandomMonster(List<MonsterData> canSpawnMonsters)
    {
        var allMonsters = canSpawnMonsters;
        if (allMonsters == null || allMonsters.Count == 0)
        {
            Debug.LogWarning("No monsters found in GameData!");
            return;
        }

        float totalWeight = 0f;

        foreach (var monster in allMonsters)
        {
            totalWeight += GetRarityWeight(monster.rarity);
        }

        float roll = Random.Range(0f, totalWeight);
        float cumulativeWeight = 0f;

        foreach (var monster in allMonsters)
        {
            cumulativeWeight += GetRarityWeight(monster.rarity);
            if (roll <= cumulativeWeight)
            {
                selectedMonsterEnemy = monster;
                frontSpriteRenderer.sprite = selectedMonsterEnemy.frontSpriteMonster;
                frontSpriteRenderer.flipX = Random.Range(0, 2) == 0;

                return;
            }
        }
    }

    public void GetRandomMonster()
    {
        var allMonsters = GameData.Instance.availableMonsters;
        if (allMonsters == null || allMonsters.Count == 0)
        {
            Debug.LogWarning("No monsters found in GameData!");
            return;
        }

        float totalWeight = 0f;

        foreach (var monster in allMonsters)
        {
            totalWeight += GetRarityWeight(monster.rarity);
        }

        float roll = Random.Range(0f, totalWeight);
        float cumulativeWeight = 0f;

        foreach (var monster in allMonsters)
        {
            cumulativeWeight += GetRarityWeight(monster.rarity);
            if (roll <= cumulativeWeight)
            {
                selectedMonsterEnemy = monster;
                frontSpriteRenderer.sprite = selectedMonsterEnemy.frontSpriteMonster;
                return;
            }
        }
    }

    private float GetRarityWeight(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Common: return commonWeight;
            case Rarity.Rare: return rareWeight;
            case Rarity.Legendary: return legendaryWeight;
            default: return 1f;
        }
    }
}
