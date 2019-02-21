using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class EditorLevelPooler : MonoBehaviour
{
  public string levelName = "Test";
  Transform[] levelParents;
  Dictionary<string, LevelData> levelDatas;
  LevelData currLevel;
  LevelPiece[] levelPieces;

  void Awake()
  {
		levelParents = new Transform[]{new GameObject("First").transform, new GameObject("Second").transform,
		new GameObject("Third").transform};

		DirectoryInfo dirInfo = new DirectoryInfo(Application.dataPath + "\\Levels\\");
        FileInfo[] fileInfos = dirInfo.GetFiles("*.dat");
		BinaryFormatter bf = new BinaryFormatter();
		levelDatas = new Dictionary<string, LevelData>();
		List<string> prefabIndexes = new List<string>();
		for (int i = 0; i < fileInfos.Length; i++) {
			FileStream file = fileInfos[i].Open(FileMode.Open);
			LevelData ld = (LevelData)bf.Deserialize(file);
			levelDatas.Add(ld.name, ld);
			file.Close();
			// go through prefabs of every level to assign integers instead of string names, for efficient searching
			foreach (PrefabData pd in ld.prefabDatas) {
				if (!prefabIndexes.Contains(pd.name)) {
					prefabIndexes.Add(pd.name);
				}
				pd.index = prefabIndexes.IndexOf(pd.name);
			}
		}

		currLevel = levelDatas[levelName];

		// go through each level to count how many prefabs of each type
		int[] prefabCount = new int[prefabIndexes.Count];
		for (int i = 0; i < prefabCount.Length; i++) {
			prefabCount[i] = 0;
		}

		foreach(KeyValuePair<string, LevelData> ld in levelDatas) {
			foreach (PrefabData pd in levelDatas[ld.Key].prefabDatas) {
				prefabCount[pd.index]++;
			}
		}

		/*
		// instantiate all prefabs at corresponding index
		GameObject[][] prefabs = new GameObject[prefabIndexes.Count][];
		Dictionary<string, GameObject> gameObjects = PrefabLoader.LoadAllPrefabsOfType<MonoBehaviour>("Assets/Prefabs");
		for (int i = 0; i < prefabIndexes.Count; i++) {
			prefabs[i] = new GameObject[prefabCount[i]*3];
			for (int j = 0; j < prefabs[i].Length; j++) {
				prefabs[i][j] = Instantiate(gameObjects[prefabIndexes[i]]);
				prefabs[i][j].SetActive(false);
				prefabs[i][j].name = prefabs[i][j].name.Split('(')[0];
			}
		}

		objects = o;
		endpointPositions = new Vector3[3];
		transforms = new Transform[o.Length][];
		scripts = new GameObjectExt[o.Length][];
		for (int i = 0; i < o.Length; i++) {
			transforms[i] = new Transform[o[i].Length];
			scripts[i] = new GameObjectExt[o[i].Length];
			if (o[i][0].name == "Endpoint") {
				endpointIndex = i;
			}
			for (int j = 0; j < o[i].Length; j++) {
				transforms[i][j] = o[i][j].transform;
				scripts[i][j] = o[i][j].GetComponent<GameObjectExt>();
			}
		}
		*/
	
		// instantiate all prefabs at corresponding index
		LevelPiece[][] levelPieces = new LevelPiece[prefabIndexes.Count][];
		Dictionary<string, GameObject> gameObjects = PrefabLoader.LoadAllPrefabsOfType<MonoBehaviour>("Assets/Prefabs");
		for (int i = 0; i < prefabIndexes.Count; i++) {
			levelPieces[i] = new LevelPiece[prefabCount[i]*3];
			for (int j = 0; j < levelPieces[i].Length; j++) {
				levelPieces[i][j] = new LevelPiece();
				levelPieces[i][j].prefab = Instantiate(gameObjects[prefabIndexes[i]]);
				levelPieces[i][j].prefab.SetActive(false);
				levelPieces[i][j].prefab.name = levelPieces[i][j].prefab.name.Split('(')[0];
				levelPieces[i][j].transform = levelPieces[i][j].prefab.transform;
				levelPieces[i][j].script = levelPieces[i][j].prefab.GetComponent<GameObjectExt>();
			}
		}
	}

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
