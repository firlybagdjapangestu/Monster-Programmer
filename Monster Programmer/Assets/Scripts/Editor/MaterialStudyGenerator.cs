using Data;
using System.IO;
using UnityEditor;
using UnityEngine;

public class MaterialStudyGenerator : EditorWindow
{
    private TextAsset jsonFile; // file JSON yang bisa di-drag & drop
    private string outputPath = "Assets/Data/MaterialStudyAssets"; // default path output

    [MenuItem("Tools/Generate Material Study")]
    public static void ShowWindow()
    {
        GetWindow<MaterialStudyGenerator>("Material Study Generator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Material Study Generator", EditorStyles.boldLabel);

        jsonFile = (TextAsset)EditorGUILayout.ObjectField("JSON File", jsonFile, typeof(TextAsset), false);
        outputPath = EditorGUILayout.TextField("Output Path", outputPath);

        if (GUILayout.Button("Generate Material Study") && jsonFile != null)
        {
            GenerateMaterialStudyFromJson(jsonFile, outputPath);
        }
    }

    private void GenerateMaterialStudyFromJson(TextAsset json, string savePath)
    {
        if (!AssetDatabase.IsValidFolder(savePath))
        {
            Debug.LogError("Output path is invalid or doesn't exist: " + savePath);
            return;
        }

        MaterialStudyJson parsed = JsonUtility.FromJson<MaterialStudyJson>(json.text);
        if (!System.Enum.TryParse(parsed.typeSubject, out MapMonster subjectType))
        {
            Debug.LogError($"Invalid typeSubject in JSON: {parsed.typeSubject}");
            return;
        }

        MaterialStudy asset = ScriptableObject.CreateInstance<MaterialStudy>();

        // Isi data ke SO
        SerializedObject so = new SerializedObject(asset);
        so.FindProperty("typeSubject").enumValueIndex = (int)subjectType;

        SerializedProperty arrayProp = so.FindProperty("allSubject");
        arrayProp.ClearArray();
        for (int i = 0; i < parsed.allSubject.Length; i++)
        {
            arrayProp.InsertArrayElementAtIndex(i);
            arrayProp.GetArrayElementAtIndex(i).stringValue = parsed.allSubject[i];
        }

        so.ApplyModifiedProperties();

        // Simpan SO ke path
        string fileName = $"{subjectType}_MaterialStudy.asset";
        string fullPath = Path.Combine(savePath, fileName);
        AssetDatabase.CreateAsset(asset, fullPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Material Study '{subjectType}' created at: {fullPath}");
    }
}

[System.Serializable]
public class MaterialStudyJson
{
    public string typeSubject;
    public string[] allSubject;
}

