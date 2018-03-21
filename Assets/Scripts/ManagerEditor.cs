using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class ManagerEditor : MonoBehaviour {

	public static ManagerEditor instance = null;

	public string levelName = "Test";

	Dictionary<string, LevelData> levelDatas;
	ObjectPooler pooler;
	LevelData currLevel;
	Transform[] levelParents;

	void Awake() {
		if (instance == null) {
			instance = this;
		}
		else if (instance != this) {
			Destroy(gameObject);
		}

		pooler = GetComponent<ObjectPooler>();
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

		// instantiate all prefabs at corresponding index
		GameObject[][] prefabs = new GameObject[prefabIndexes.Count][];
		Dictionary<string, GameObject> gameObjects = PrefabLoader.LoadAllPrefabsOfType<MonoBehaviour>("Assets/Prefabs");
		for (int i = 0; i < prefabIndexes.Count; i++) {
			prefabs[i] = new GameObject[prefabCount[i]*3];
			for (int j = 0; j < prefabs[i].Length; j++) {
				prefabs[i][j] = Instantiate(gameObjects[prefabIndexes[i]]);
				prefabs[i][j].SetActive(false);
			}
		}

		pooler.Initiate(prefabs);

		for (int i = 0; i < currLevel.prefabDatas.Length; i++) {
			PrefabData pd = currLevel.prefabDatas[i];
			pooler.ActivateObject(levelParents[0], pd.index, pd.positionX, pd.positionY, pd.positionZ,
			pd.rotationX, pd.rotationY, pd.rotationZ, pd.scaleX, pd.scaleY, pd.scaleZ, pd.extraValues);
			pooler.ActivateObject(levelParents[1], pd.index, pd.positionX, pd.positionY, pd.positionZ,
			pd.rotationX, pd.rotationY, pd.rotationZ, pd.scaleX, pd.scaleY, pd.scaleZ, pd.extraValues);
			pooler.ActivateObject(levelParents[2], pd.index, pd.positionX, pd.positionY, pd.positionZ,
			pd.rotationX, pd.rotationY, pd.rotationZ, pd.scaleX, pd.scaleY, pd.scaleZ, pd.extraValues);
		}
	}

	void Update() {
		
	}

}