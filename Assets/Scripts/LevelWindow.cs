﻿using UnityEngine;
using UnityEditor;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

public class LevelWindow : EditorWindow
{
    private string basePath = "C:\\Users\\John\\Documents\\code\\motif-unity\\Assets\\Levels\\";

    // Add menu named "My Window" to the Window menu
    [MenuItem("Window/Level Window")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        LevelWindow window = (LevelWindow)EditorWindow.GetWindow(typeof(LevelWindow));
        window.Show();
    }

    void OnGUI()
    {
        if (GUILayout.Button("Generate Level")) {
            GameObject obj =  (GameObject)EditorUtility.InstanceIDToObject(Selection.activeObject.GetInstanceID());
            LevelData ld = new LevelData();
            ld.name = obj.name;
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
        if (GUILayout.Button("Load Level")) {
            string path = EditorUtility.OpenFilePanel("Choose level", "Assets\\Levels", "dat");
            if (path.Length != 0) {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(path, FileMode.Open);
                LevelData ld = (LevelData)bf.Deserialize(file);
                file.Close();
                GameObject level = new GameObject(ld.name);
                Dictionary<string, GameObject> prefabs = PrefabLoader.LoadAllPrefabsOfType<MonoBehaviour>("Assets/Prefabs");
                for (int i = 0; i < ld.prefabDatas.Length; i++) {
                    PrefabData pd = ld.prefabDatas[i];
                    GameObject prefab = (GameObject)PrefabUtility.InstantiatePrefab(prefabs[pd.name]);
                    prefab.transform.parent = level.transform;
                    prefab.transform.Translate(new Vector3(pd.positionX, pd.positionY, pd.positionZ));
                    prefab.transform.Rotate(new Vector3(pd.rotationX, pd.rotationY, pd.rotationZ));
                    prefab.transform.localScale = new Vector3(pd.scaleX, pd.scaleY, pd.scaleZ);
                }
            }
        }
    }
}

[System.Serializable]
public class LevelData {
    public string name;
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

public static class PrefabLoader
{
    //So, there's no "load all assets in directory" function in unity. 
    //I guess this is to avoid people using Prefabs as "data blobs". 
    //They'd rather you use ScriptableObjects... which is fine in some cases, 
    //but sometimes the thing you're blobbing is just a bunch of child transforms anyway,
    //so it's huge buckets of sweat to reproduce the scene tools unity already has. 
    
    //The "AssetsDatabase.LoadAllAssetsAtPath" refers to /compound/ raw assets,
    //like maya files. But it doesn't care about humble prefabs, 
    //which are more like compounds of assets, rather than raw assets.
    
    //This function collates all the the behaviours you want in the directory you point it at. The path is relative to your Assets.
    //i.e. "Assets/MyDirectory/"
    //It returns the Prefab References, remember! Not instantiated scene objects! 
    //So it's only used for *editor-side* tools. Not run time.
    //Quite useful in conjunction with "PrefabUtility".
    public static Dictionary<string, GameObject> LoadAllPrefabsOfType<T>(string path) where T : MonoBehaviour
    {
        if (path != "")
        {
            if (path.EndsWith("/"))
            {
                path = path.TrimEnd('/');
            }
        }

        DirectoryInfo dirInfo = new DirectoryInfo(path);
        FileInfo[] fileInf = dirInfo.GetFiles("*.prefab");

        //loop through directory loading the game object and checking if it has the component you want
        Dictionary<string, GameObject> prefabComponents = new Dictionary<string, GameObject>();
        foreach (FileInfo fileInfo in fileInf)
        {
            string fullPath = fileInfo.FullName.Replace(@"\","/");
            string assetPath = "Assets" + fullPath.Replace(Application.dataPath, "");
            GameObject prefab = AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject)) as GameObject;

            if(prefab!= null)
            {
                prefabComponents.Add(prefab.name, prefab);
            }
        }
        return prefabComponents;
    }

}