using UnityEngine;
using UnityEditor;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

public class LevelBuilderWindow : EditorWindow
{
    private string basePath = Application.streamingAssetsPath + "\\Levels\\";

    // Add menu named "My Window" to the Window menu
    [MenuItem("Window/Level Builder")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        LevelBuilderWindow window = (LevelBuilderWindow)EditorWindow.GetWindow(typeof(LevelBuilderWindow));
        window.Show();
    }

    void OnGUI()
    {
        if (GUILayout.Button("Save Level")) {
            GameObject obj =  (GameObject)EditorUtility.InstanceIDToObject(Selection.activeObject.GetInstanceID());
            LevelData ld = new LevelData();
            ld.name = obj.name;
            ld.prefabDatas = new PrefabData[obj.transform.childCount];
            Dictionary<string, int> prefabIndexes = GetPrefabIndexes();
            int count = 0;
            bool hasEndpoint = false;
            foreach (Transform c in obj.transform) {
                c.name = c.name.Split(' ')[0];
                ld.prefabDatas[count] = new PrefabData();
                ld.prefabDatas[count].name = c.name;
                ld.prefabDatas[count].index = prefabIndexes[c.name];
                ld.prefabDatas[count].positionX = c.localPosition.x;
                ld.prefabDatas[count].positionY = c.localPosition.y;
                ld.prefabDatas[count].positionZ = c.localPosition.z;
                ld.prefabDatas[count].rotationX = c.localEulerAngles.x;
                ld.prefabDatas[count].rotationY = c.localEulerAngles.y;
                ld.prefabDatas[count].rotationZ = c.localEulerAngles.z;
                ld.prefabDatas[count].scaleX = c.localScale.x;
                ld.prefabDatas[count].scaleY = c.localScale.y;
                ld.prefabDatas[count].scaleZ = c.localScale.z;
                LevelPieceScript script = c.GetComponent<LevelPieceScript>();
                if (script != null) {
                    ld.prefabDatas[count].extraValues = script.GetExtraValues();
                }
                if (c.name == "Endpoint") {
                    hasEndpoint = true;
                    ld.endpointX = c.localPosition.x;
                    ld.endpointY = c.localPosition.y;
                }
                count++;
            }
            if (!File.Exists(basePath + obj.name + ".dat") ||
            EditorUtility.DisplayDialog("File already exists", "Do you want to overwrite?", "Yes", "No")) {
                if (!hasEndpoint) {
                    EditorUtility.DisplayDialog("Error", "This level has no endpoint", "OK");
                }
                else {
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
        if (GUILayout.Button("Load Level")) {
            string path = EditorUtility.OpenFilePanel("Choose level", "Assets\\StreamingAssets\\Levels", "dat");
            if (path.Length != 0) {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(path, FileMode.Open);
                LevelData ld = (LevelData)bf.Deserialize(file);
                file.Close();
                GameObject level = new GameObject(ld.name);
                Dictionary<string, GameObject> prefabs = PrefabLoader.LoadAllPrefabsOfType<MonoBehaviour>("Assets/Resources/LevelPieces");
                for (int i = 0; i < ld.prefabDatas.Length; i++) {
                    PrefabData pd = ld.prefabDatas[i];
                    GameObject prefab = (GameObject)PrefabUtility.InstantiatePrefab(prefabs[pd.name]);
                    prefab.transform.parent = level.transform;
                    prefab.transform.Translate(pd.positionX, pd.positionY, pd.positionZ);
                    prefab.transform.Rotate(pd.rotationX, pd.rotationY, pd.rotationZ);
                    prefab.transform.localScale = new Vector3(pd.scaleX, pd.scaleY, pd.scaleZ);
                    if (pd.extraValues != null) {
                        LevelPieceScript script = prefab.GetComponent<LevelPieceScript>();
                        script.SetExtraValues(pd.extraValues);
                    }
                }
            }
        }
    }

    Dictionary<string, int> GetPrefabIndexes() {
        DirectoryInfo dirInfo = new DirectoryInfo(System.IO.Directory.GetCurrentDirectory() + "\\Assets\\Resources\\LevelPieces\\");
        FileInfo[] fileInfos = dirInfo.GetFiles("*.prefab");
        Dictionary<string, int> indexes = new Dictionary<string, int>();
        for (int i = 0; i < fileInfos.Length; i++) {
            indexes.Add(fileInfos[i].Name.Replace(".prefab", ""), i);
        }
        return indexes;
    }
}