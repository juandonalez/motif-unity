using UnityEngine;
using UnityEditor;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class LevelEditor : EditorWindow
{
    private string basePath = "C:\\Users\\John\\Documents\\code\\motif-unity\\Assets\\Levels\\";

    // Add menu named "My Window" to the Window menu
    [MenuItem("Window/Level Editor")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        LevelEditor window = (LevelEditor)EditorWindow.GetWindow(typeof(LevelEditor));
        window.Show();
    }

    void OnGUI()
    {
        if (GUILayout.Button("Generate Level")) {
            GameObject obj =  (GameObject)EditorUtility.InstanceIDToObject(Selection.activeObject.GetInstanceID());
            LevelData ld = new LevelData();
            ld.prefabDatas = new PrefabData[obj.transform.childCount];
            int count = 0;
            foreach (Transform c in obj.transform) {
                ld.prefabDatas[count] = new PrefabData();
                ld.prefabDatas[count].name = c.name;
                ld.prefabDatas[count].positionX = c.localPosition.x;
                ld.prefabDatas[count].positionY = c.localPosition.y;
                ld.prefabDatas[count].positionZ = c.localPosition.z;
                ld.prefabDatas[count].rotationX = c.localEulerAngles.x;
                ld.prefabDatas[count].rotationY = c.localEulerAngles.y;
                ld.prefabDatas[count].rotationZ = c.localEulerAngles.z;
                ld.prefabDatas[count].scaleX = c.localScale.x;
                ld.prefabDatas[count].scaleY = c.localScale.y;
                ld.prefabDatas[count].scaleZ = c.localScale.z;
                GameObjectExt script = c.GetComponent<GameObjectExt>();
                if (script != null) {
                    ld.prefabDatas[count].extraValues = script.GetExtraValues();
                }
                count++;
            }
            if (!File.Exists(basePath + obj.name + ".dat") ||
            EditorUtility.DisplayDialog("File already exists", "Do you want to overwrite?", "Yes", "No")) {
                XmlSerializer serializer = new XmlSerializer(typeof(LevelData));
                StreamWriter writer = new StreamWriter(basePath + "xml\\" + obj.name + ".xml");
                serializer.Serialize(writer.BaseStream, ld);
                writer.Close();
                BinaryFormatter bf = new BinaryFormatter();
                FileStream stream = File.Create(basePath + obj.name + ".dat");
                bf.Serialize(stream, ld);
                stream.Close();
            }
        }
    }
}

[System.Serializable]
public class LevelData {
    public PrefabData[] prefabDatas;
}

[System.Serializable]
public class PrefabData {
    public string name;
    public float positionX;
    public float positionY;
    public float positionZ;
    public float rotationX;
    public float rotationY;
    public float rotationZ;
    public float scaleX;
    public float scaleY;
    public float scaleZ;
    public float[] extraValues;
}