using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class MonsterDataGenerator : EditorWindow
{
    private TextAsset jsonFile;
    private string savePath = "Assets/Scripts/Monster Data/Abstraction"; // Sesuaikan folder tujuan

    [MenuItem("Tools/Generate MonsterData From JSON")]
    public static void ShowWindow()
    {
        GetWindow<MonsterDataGenerator>("MonsterData Generator");
    }

    void OnGUI()
    {
        GUILayout.Label("Generate MonsterData ScriptableObjects", EditorStyles.boldLabel);
        jsonFile = (TextAsset)EditorGUILayout.ObjectField("JSON File", jsonFile, typeof(TextAsset), false);
        savePath = EditorGUILayout.TextField("Save Path", savePath);

        if (GUILayout.Button("Generate"))
        {
            if (jsonFile == null)
            {
                Debug.LogError("Please assign a JSON file.");
                return;
            }

            GenerateFromJSON();
        }
    }

    private void GenerateFromJSON()
    {
        if (!Directory.Exists(savePath))
            Directory.CreateDirectory(savePath);

        MonsterJsonWrapper[] monsterDatas = JsonHelper.FromJson<MonsterJsonWrapper>(jsonFile.text);

        foreach (var data in monsterDatas)
        {
            MonsterType _type = GetTypeMonsterFromString(data.type);
            Rarity _rarity = GetRarityFromString(data.rarity);

            MonsterData asset = ScriptableObject.CreateInstance<MonsterData>();
            asset.monsterID = data.monsterID;
            asset.monsterName = data.monsterName;
            asset.type = _type;
            asset.rarity = _rarity;
            asset.attack = data.attack;
            asset.defense = data.defense;
            asset.speed = data.speed;

            // Buat nama file sesuai format singkatan
            string typeShort = ShortenType(_type);
            string rarityShort = ShortenRarity(_rarity);
            string fileName = $"{typeShort}_{rarityShort}_{data.monsterName}.asset";
            string path = $"{savePath}/{fileName}";

            AssetDatabase.CreateAsset(asset, path);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("MonsterData created successfully.");
    }

    MonsterType GetTypeMonsterFromString(string type)
    {
        switch (type)
        {
            case "CPlusPlus": return MonsterType.CPlusPlus;
            case "Java": return MonsterType.Java;
            case "Python": return MonsterType.Python;
            default: return MonsterType.CPlusPlus;
        }
    }

    Rarity GetRarityFromString(string type)
    {
        switch (type)
        {
            case "Common": return Rarity.Common;
            case "Rare": return Rarity.Rare;
            case "Legendary": return Rarity.Legendary;
            default: return Rarity.Common;
        }
    }

    string ShortenType(MonsterType type)
    {
        switch (type)
        {
            case MonsterType.Python: return "P";
            case MonsterType.Java: return "J";
            case MonsterType.CPlusPlus: return "C";
            default: return type.ToString();
        }
    }

    string ShortenRarity(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Common:return "C";
            case Rarity.Rare: return "R";
            case Rarity.Legendary: return "L";
            default: return rarity.ToString();
        }
    }

    // Wrapper untuk serialisasi JSON
    [System.Serializable]
    private class MonsterJsonWrapper
    {
        public string monsterID;
        public string monsterName;
        public string type;
        public string rarity;
        public int attack;
        public int defense;
        public int speed;
    }

    // JSON array helper
    public static class JsonHelper
    {
        public static T[] FromJson<T>(string json)
        {
            string newJson = "{ \"array\": " + json + "}";
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
            return wrapper.array;
        }

        [System.Serializable]
        private class Wrapper<T>
        {
            public T[] array;
        }
    }
}