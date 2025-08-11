using System;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData Instance { get; private set; }

    public string selectedMonsterIndex;
    public static MonsterData selectedPlayerMonster;
    public static MonsterData selectedEnemyMonster;
    public int level = 1;
    public int currentXp = 0;
    public int captureBalls = 0;
    public int Coins = 0;

    public List<MonsterData> availableMonsters = new List<MonsterData>();
    public List<MonsterData> ownedMonsters = new List<MonsterData>();

    [Header("[Default Monster]")]
    public MonsterData defaultMonster;

    // KEY
    const string COIN_KEY = "coins";
    const string XP_KEY = "current_xp";

    public event Action OnDataChange;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        LoadGame();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.S))
        {
            SaveGame();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadGame();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetSave();
        }
    }

    public void SetDefaultMonster()
    {
        ownedMonsters.Add(defaultMonster);
    }

    public MonsterData SelectPlayerMonster(string id)
    {
        selectedPlayerMonster = availableMonsters.Find(monster => monster.monsterID == id);
        if (selectedPlayerMonster != null)
        {
            selectedMonsterIndex = id;
            Debug.Log($"Monster selected: {selectedPlayerMonster.monsterName}");
        }
        else
        {
            Debug.LogWarning($"Monster with ID {id} not found in available monsters.");
        }
        return selectedPlayerMonster;
    }

    public void SelectEnemyMonster(string id)
    {
        selectedEnemyMonster = availableMonsters.Find(monster => monster.monsterID == id);
        if (selectedEnemyMonster != null)
        {
            
            Debug.Log($"Enemy monster selected: {selectedEnemyMonster.monsterName}");
        }
        else
        {
            Debug.LogWarning($"Enemy monster with ID {id} not found in available monsters.");
        }
    }



    public void AddMonster(MonsterData monster)
    {
        if (!ownedMonsters.Contains(monster))
        {
            ownedMonsters.Add(monster);
        }
    }

    public void RemoveMonster(MonsterData monster)
    {
        if (ownedMonsters.Contains(monster))
        {
            ownedMonsters.Remove(monster);
        }
    }

    public void SaveGame()
    {
        PlayerPrefs.SetInt("Level", level);
        PlayerPrefs.SetInt("CaptureBalls", captureBalls);
        PlayerPrefs.SetInt(COIN_KEY, Coins);
        PlayerPrefs.SetInt(XP_KEY, currentXp);

        // Simpan ID monster yang dimiliki dalam bentuk string  
        List<string> ownedMonsterIDs = new List<string>();
        foreach (MonsterData monster in ownedMonsters)
        {
            ownedMonsterIDs.Add(monster.monsterID);
        }
        string joinedOwnedIDs = string.Join(",", ownedMonsterIDs);
        PlayerPrefs.SetString("OwnedMonsterIDs", joinedOwnedIDs);

        // Simpan ID monster yang telah dihapus dalam bentuk string  
        List<string> removedMonsterIDs = new List<string>();
        foreach (MonsterData monster in availableMonsters)
        {
            if (!ownedMonsters.Contains(monster))
            {
                removedMonsterIDs.Add(monster.monsterID);
            }
        }
        string joinedRemovedIDs = string.Join(",", removedMonsterIDs);
        PlayerPrefs.SetString("RemovedMonsterIDs", joinedRemovedIDs);

        OnDataChange?.Invoke();
        PlayerPrefs.Save();
        Debug.Log("Game Saved!");
    }

    public void LoadGame()
    {
        level = PlayerPrefs.GetInt("Level", 1);
        captureBalls = PlayerPrefs.GetInt("CaptureBalls", 10);
        Coins = PlayerPrefs.GetInt(COIN_KEY, 100);
        currentXp = PlayerPrefs.GetInt(XP_KEY, 0);

        string savedIDs = PlayerPrefs.GetString("OwnedMonsterIDs", "");
        ownedMonsters.Clear();

        if (!string.IsNullOrEmpty(savedIDs))
        {
            string[] ids = savedIDs.Split(',');
            foreach (string id in ids)
            {
                // Cari monster berdasarkan ID di daftar monster yang tersedia
                MonsterData found = availableMonsters.Find(m => m.monsterID == id);
                if (found != null)
                {
                    ownedMonsters.Add(found);
                }
            }
        }

        Debug.Log($"Game Loaded! Level: {level}, Capture Balls: {captureBalls}");
        foreach (MonsterData monster in ownedMonsters)
        {
            Debug.Log($"Owned Monster: {monster.monsterName}");
        }

        Debug.Log("Game Loaded!");

        // add default monster is not have anyone
        if (ownedMonsters.Count <=  0 && defaultMonster != null)
        {
            ownedMonsters.Add(defaultMonster);
        }

        OnDataChange?.Invoke();

    }

    public void ResetSave()
    {
        PlayerPrefs.DeleteAll();
        ownedMonsters.Clear();
        level = 1;
        captureBalls = 1;
        Debug.Log("Save reset.");

        OnDataChange?.Invoke();

    }
}
