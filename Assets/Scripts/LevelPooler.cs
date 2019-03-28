using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class LevelPooler : MonoBehaviour
{
    EditorCamera cam;
    LevelData[] levels;
	LevelPiece[][] levelPieces;
    //int[] currLevels;
    int level1Index = 0;
    int level2Index = 0;
    int level3Index = 0;
    int[] prevIndexes;
    float startPosition = 0;

    void Awake()
    {
        cam = Camera.main.GetComponent<EditorCamera>();

        // load in all the data for all the levels
        DirectoryInfo dirInfo = new DirectoryInfo(Application.streamingAssetsPath + "\\Levels\\");
        FileInfo[] fileInfos = dirInfo.GetFiles("*.dat");
        BinaryFormatter bf = new BinaryFormatter();
        Dictionary<string, LevelData> levelDatas = new Dictionary<string, LevelData>();
        List<string> prefabIndexes = new List<string>();

        List<int[]> prefabTop3 = new List<int[]>();
        for (int i = 0; i < fileInfos.Length; i++) {
            FileStream file = fileInfos[i].Open(FileMode.Open);
            LevelData ld = (LevelData)bf.Deserialize(file);
            levelDatas.Add(ld.name, ld);
            file.Close();
            // go through prefabs of every level to assign integers instead of string names, for efficient searching
            // instantiate empty top 3 array for later
            foreach (PrefabData pd in ld.prefabDatas) {
                if (!prefabIndexes.Contains(pd.name)) {
                    prefabIndexes.Add(pd.name);
                    prefabTop3.Add(new int[]{0, 0, 0});
                }
                pd.index = prefabIndexes.IndexOf(pd.name);
            }
        }

        /******
        Dictionary<string, LevelData> levelDatas can now used here for seed generation
        String names may be useful for generator but makes it less efficient to search through later
        This will eventually be changed into simple LevelData array levels
        *******/

        levels = new LevelData[levelDatas.Count];
        int index = 0;
        foreach(KeyValuePair<string, LevelData> ld in levelDatas) {
            levels[index] = ld.Value;
            index++;
        }

        prevIndexes = new int[5] {-1, -1, -1, -1, -1};
        level1Index = GetNextLevel();
        level2Index = GetNextLevel();
        level3Index = GetNextLevel();

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
		levelPieces = new LevelPiece[prefabIndexes.Count][];
		Dictionary<string, GameObject> prefabs = PrefabLoader.LoadAllPrefabsOfType<MonoBehaviour>("Assets/Resources/LevelPieces");
		for (int i = 0; i < prefabIndexes.Count; i++) {
			levelPieces[i] = new LevelPiece[prefabTop3[i][0] + prefabTop3[i][1] + prefabTop3[i][2]];
			for (int j = 0; j < levelPieces[i].Length; j++) {
                levelPieces[i][j] = new LevelPiece();
				levelPieces[i][j].prefab = Instantiate(prefabs[prefabIndexes[i]]);
				levelPieces[i][j].prefab.SetActive(false);
                // when instantiated every name has (1), (2), etc so remove that
                levelPieces[i][j].prefab.name = levelPieces[i][j].prefab.name.Split('(')[0];
                // get references to transform and script so we don't have to get them each time
                levelPieces[i][j].transform = levelPieces[i][j].prefab.transform;
                levelPieces[i][j].script = levelPieces[i][j].prefab.GetComponent<LevelPieceScript>();
                levelPieces[i][j].parent = -1;
			}
		}

        LevelData level1 = levels[level1Index];
        LevelData level2 = levels[level2Index];
        LevelData level3 = levels[level3Index];

        for (int i = 0; i < level1.prefabDatas.Length; i++) {
            PrefabData pd = level1.prefabDatas[i];
            PlaceLevelPiece(level1Index, pd.index, 0, 0, pd.positionX, pd.positionY, pd.positionZ,
            pd.rotationX, pd.rotationY, pd.rotationZ, pd.scaleX, pd.scaleY, pd.scaleZ, pd.extraValues);
        }

        for (int i = 0; i < level2.prefabDatas.Length; i++) {
            PrefabData pd = level2.prefabDatas[i];
            PlaceLevelPiece(level2Index, pd.index, level1.endpointX, level1.endpointY, pd.positionX, pd.positionY, pd.positionZ,
            pd.rotationX, pd.rotationY, pd.rotationZ, pd.scaleX, pd.scaleY, pd.scaleZ, pd.extraValues);
        }

        for (int i = 0; i < level3.prefabDatas.Length; i++) {
            PrefabData pd = level3.prefabDatas[i];
            PlaceLevelPiece(level3Index, pd.index, level1.endpointX + level2.endpointX, level2.endpointY, pd.positionX,
            pd.positionY, pd.positionZ, pd.rotationX, pd.rotationY, pd.rotationZ, pd.scaleX, pd.scaleY, pd.scaleZ, pd.extraValues);
        }
    }

	void Update()
    {
        if (cam.hitbox.origin.x >= levels[level1Index].endpointX + startPosition) {
            startPosition += levels[level1Index].endpointX;
            for (int i = 0; i < levels[level1Index].prefabDatas.Length; i++) {
                PrefabData pd = levels[level1Index].prefabDatas[i];
                LevelPiece lp;
                for (int j = 0; j < levelPieces[pd.index].Length; j++) {
                    lp = levelPieces[pd.index][j];
                    if (lp.parent == level1Index) {
                        lp.parent = -1;
                        lp.transform.localPosition = new Vector3(0, 0, 0);
						lp.transform.localEulerAngles = new Vector3(0, 0, 0);
						lp.transform.localScale = new Vector3(1, 1, 1);
                        if (lp.script != null) {
                            lp.script.Reset(pd.extraValues);
                        }
                        lp.prefab.SetActive(false);
                        break;
                    }
                }
            }
            level1Index = level2Index;
            level2Index = level3Index;
            level3Index = GetNextLevel();
            Debug.Log(startPosition + ": " + prevIndexes[0] + ", " + prevIndexes[1] + ", " + prevIndexes[2] + ", " + prevIndexes[3]+ ", " + prevIndexes[4]);
        }
    }

    int GetNextLevel()
    {
        int level = Random.Range(0, levels.Length);
        while (level == prevIndexes[0] ||
        level == prevIndexes[1] ||
        level == prevIndexes[2] ||
        level == prevIndexes[3] ||
        level == prevIndexes[4]) {
            level = Random.Range(0, levels.Length);
        }
        prevIndexes[0] = prevIndexes[1];
        prevIndexes[1] = prevIndexes[2];
        prevIndexes[2] = prevIndexes[3];
        prevIndexes[3] = prevIndexes[4];
        prevIndexes[4] = level;
        return level;
    }

    void PlaceLevelPiece(int parent, int index, float offsetX, float offsetY, float pX, float pY, float pZ, float rX,
    float rY, float rZ, float sX, float sY, float sZ, float[] extraValues)
    {
        LevelPiece lp;
        for (int i = 0; i < levelPieces[index].Length; i++) {
            lp = levelPieces[index][i];
            if (!lp.prefab.activeSelf) {
                lp.transform.Translate(offsetX + pX, offsetY + pY, pZ);
                lp.transform.Rotate(rX, rY, rZ);
                lp.transform.localScale = new Vector3(sX, sY, sZ);
                if (extraValues != null) {
                    lp.script.SetExtraValues(extraValues);
                }
                lp.parent = parent;
                lp.prefab.SetActive(true);
                break;
            }
        }
    }
}
