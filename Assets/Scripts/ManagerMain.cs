using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class ManagerMain : MonoBehaviour {

	public static ManagerMain instance = null;

	LevelData[] levels;
	GameObject[][] prefabs;

	void Awake() {
		if (instance == null) {
			instance = this;
		}
		else if (instance != this) {
			Destroy(gameObject);
		}
		DontDestroyOnLoad(gameObject);

		DirectoryInfo dirInfo = new DirectoryInfo(Application.dataPath + "\\Levels\\");
        FileInfo[] fileInfos = dirInfo.GetFiles("*.dat");
		levels = new LevelData[fileInfos.Length];
		BinaryFormatter bf = new BinaryFormatter();
		for (int i = 0; i < levels.Length; i++) {
			FileStream file = fileInfos[i].Open(FileMode.Open);
			levels[i] = (LevelData)bf.Deserialize(file);
			file.Close();
		}

		// go through prefabs of every level to assign integers instead of string names, for efficient searching
		// instantiate empty top 3 array for later
		List<string> prefabIndexes = new List<string>();
		List<int[]> prefabTop3 = new List<int[]>();
		foreach (LevelData ld in levels) {			
			foreach (PrefabData pd in ld.prefabDatas) {
				if (!prefabIndexes.Contains(pd.name)) {
					prefabIndexes.Add(pd.name);
					prefabTop3.Add(new int[]{0, 0, 0});
				}
				pd.index = prefabIndexes.IndexOf(pd.name);
			}
		}

		// go through every level to see how many of each prefab we need
		// only three levels will ever be active at one time so get top 3 amounts of each prefab
		foreach (LevelData ld in levels) {
			int[] prefabCount = new int[prefabIndexes.Count];
			for (int i = 0; i < prefabCount.Length; i++) {
				prefabCount[i] = 0;
			}
			foreach (PrefabData pd in ld.prefabDatas) {
				prefabCount[pd.index]++;
			}
			for (int i = 0; i < prefabCount.Length; i++) {
				int first = prefabTop3[i][0];
				int second = prefabTop3[i][1];
				int third = prefabTop3[i][2];
				int smallest = Mathf.Min(first, Mathf.Min(second, third));
				if (prefabCount[i] > smallest) {
					if (smallest == first) {
						prefabTop3[i][0] = prefabCount[i];
					}
					else if (smallest == second) {
						prefabTop3[i][1] = prefabCount[i];
					}
					else {
						prefabTop3[i][2] = prefabCount[i];
					}
				}
			}
		}

		// instantiate all prefabs at corresponding index
		prefabs = new GameObject[prefabIndexes.Count][];
		Dictionary<string, GameObject> gameObjects = PrefabLoader.LoadAllPrefabsOfType<MonoBehaviour>("Assets/Prefabs");
		for (int i = 0; i < prefabIndexes.Count; i++) {
			prefabs[i] = new GameObject[prefabTop3[i][0] + prefabTop3[i][1] + prefabTop3[i][2]];
			for (int j = 0; j < prefabs[i].Length; j++) {
				prefabs[i][j] = Instantiate(gameObjects[prefabIndexes[i]]);
				prefabs[i][j].SetActive(false);
			}
		}
	}

}
