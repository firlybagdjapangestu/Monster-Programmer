using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using Data;

public class QuestionGeneratorWindow : EditorWindow
{
    private TextAsset jsonFile;
    private string savePath = "Assets/Questions/";

    [MenuItem("Tools/Generate Questions From JSON")]
    public static void OpenWindow()
    {
        GetWindow<QuestionGeneratorWindow>("Question Generator");
    }

    private void OnGUI()
    {
        GUILayout.Label("JSON to ScriptableObject Generator", EditorStyles.boldLabel);

        jsonFile = (TextAsset)EditorGUILayout.ObjectField("JSON File", jsonFile, typeof(TextAsset), false);
        savePath = EditorGUILayout.TextField("Save Path", savePath);

        if (GUILayout.Button("Generate Questions"))
        {
            if (jsonFile == null)
            {
                Debug.LogWarning("Please assign a JSON file.");
                return;
            }

            GenerateQuestions();
        }
    }

    private void GenerateQuestions()
    {
        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }

        string json = jsonFile.text;
        List<QuestionData> questionList = JsonHelper.FromJson<QuestionData>(json);

        for (int i = 0; i < questionList.Count; i++)
        {
            QuestionData data = questionList[i];
            string assetPath = Path.Combine(savePath, $"Question_{i + 1}.asset");

            // Cek apakah file asset sudah ada
            Question qAsset = AssetDatabase.LoadAssetAtPath<Question>(assetPath);
            if (qAsset == null)
            {
                // Kalau belum ada, buat baru
                qAsset = ScriptableObject.CreateInstance<Question>();
                AssetDatabase.CreateAsset(qAsset, assetPath);
            }

            // Update value asset (pakai SerializedObject untuk field private)
            SerializedObject so = new SerializedObject(qAsset);
            so.FindProperty("questionText").stringValue = data.questionText;

            // Reset dan isi answers
            var answersProp = so.FindProperty("answers");
            answersProp.ClearArray();
            for (int j = 0; j < data.answers.Length; j++)
            {
                answersProp.InsertArrayElementAtIndex(j);
                answersProp.GetArrayElementAtIndex(j).stringValue = data.answers[j];
            }

            so.FindProperty("indexRightAnswer").intValue = data.indexRightAnswer;
            so.FindProperty("specializeType").enumValueIndex = GetTypeMonster(data.specializeType);

            so.ApplyModifiedProperties();
            EditorUtility.SetDirty(qAsset);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"{questionList.Count} Question assets created/updated successfully at {savePath}");
    }

    /*private void GenerateQuestions()
    {
        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }

        string json = jsonFile.text;
        List<QuestionData> questionList = JsonHelper.FromJson<QuestionData>(json);

        for (int i = 0; i < questionList.Count; i++)
        {
            QuestionData data = questionList[i];
            Question qAsset = ScriptableObject.CreateInstance<Question>();

            // Use reflection or serializedObject if private fields are not serialized directly
            SerializedObject so = new SerializedObject(qAsset);
            so.FindProperty("questionText").stringValue = data.questionText;
            so.FindProperty("answers").ClearArray();
            for (int j = 0; j < data.answers.Length; j++)
            {
                so.FindProperty("answers").InsertArrayElementAtIndex(j);
                so.FindProperty("answers").GetArrayElementAtIndex(j).stringValue = data.answers[j];
            }
            so.FindProperty("indexRightAnswer").intValue = data.indexRightAnswer;
            so.FindProperty("specializeType").enumValueIndex = GetTypeMonster(data.specializeType);
            so.ApplyModifiedProperties();

            string assetPath = Path.Combine(savePath, $"Question_{i + 1}.asset");
            AssetDatabase.CreateAsset(qAsset, assetPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"{questionList.Count} Question assets created successfully at {savePath}");
    }*/

    private int GetTypeMonster(string _mntr)
    {
        // Python, Java, CPlusPlus, None}
        switch (_mntr)
        {
            case "Python": return (int)MonsterType.Python;
            case "Java": return (int)MonsterType.Java;
            case "CPlusPlus": return (int)MonsterType.CPlusPlus;
            default: return (int)MonsterType.None;
        }
    }

    [System.Serializable]
    private class QuestionData
    {
        public string questionText;
        public string[] answers;
        public int indexRightAnswer;
        public string specializeType;
    }

    public static class JsonHelper
    {
        public static List<T> FromJson<T>(string json)
        {
            string wrappedJson = $"{{\"array\":{json}}}";
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(wrappedJson);
            return new List<T>(wrapper.array);
        }

        [System.Serializable]
        private class Wrapper<T>
        {
            public T[] array;
        }
    }
}
