using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class EditorLevelPooler : MonoBehaviour
{
    EditorCamera cam;
    public string levelName = "Test";
    Dictionary<string, LevelData> levelDatas;	// raw data for every level
    LevelData currLevel;	// the level being tested
    LevelPiece[][] levelPieces;	// an array of the prefabs that make up each level, sorted by type
    int firstParent = 1;

    void Awake()
    {
        cam = Camera.main.GetComponent<EditorCamera>();

        // load in all the data for all the levels
        DirectoryInfo dirInfo = new DirectoryInfo(System.IO.Directory.GetCurrentDirectory() + "\\Assets\\Levels\\");
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
        levelPieces = new LevelPiece[prefabIndexes.Count][];
        Dictionary<string, GameObject> prefabs = PrefabLoader.LoadAllPrefabsOfType<MonoBehaviour>("Assets/Resources/LevelPieces");
        for (int i = 0; i < prefabIndexes.Count; i++) {
            levelPieces[i] = new LevelPiece[prefabCount[i]*3];
            for (int j = 0; j < levelPieces[i].Length; j++) {
                levelPieces[i][j] = new LevelPiece();
                levelPieces[i][j].prefab = Instantiate(prefabs[prefabIndexes[i]]);
                levelPieces[i][j].prefab.SetActive(false);
                // when instantiated every name has (1), (2), etc so remove that
                levelPieces[i][j].prefab.name = levelPieces[i][j].prefab.name.Split('(')[0];
                // get references to transform and script so we don't have to get them each time
                levelPieces[i][j].transform = levelPieces[i][j].prefab.transform;
                levelPieces[i][j].script = levelPieces[i][j].prefab.GetComponent<GameObjectExt>();
                levelPieces[i][j].parent = 0;
            }
        }

        // place 3 levels
        for (int i = 0; i < currLevel.prefabDatas.Length; i++) {
            PrefabData pd = currLevel.prefabDatas[i];
            PlaceLevelPiece(1, pd.index, 0, 0, pd.positionX, pd.positionY, pd.positionZ,
            pd.rotationX, pd.rotationY, pd.rotationZ, pd.scaleX, pd.scaleY, pd.scaleZ, pd.extraValues);

            PlaceLevelPiece(2, pd.index, currLevel.endpointX, currLevel.endpointY, pd.positionX, pd.positionY, pd.positionZ,
            pd.rotationX, pd.rotationY, pd.rotationZ, pd.scaleX, pd.scaleY, pd.scaleZ, pd.extraValues);

            PlaceLevelPiece(3, pd.index, currLevel.endpointX*2, currLevel.endpointY, pd.positionX, pd.positionY, pd.positionZ,
            pd.rotationX, pd.rotationY, pd.rotationZ, pd.scaleX, pd.scaleY, pd.scaleZ, pd.extraValues);
        }
    }

    void Update()
    {
        if (cam.hitbox.origin.x >= currLevel.endpointX*firstParent) {
            for (int i = 0; i < currLevel.prefabDatas.Length; i++) {
                PrefabData pd = currLevel.prefabDatas[i];
                LevelPiece lp;
                for (int j = 0; j < levelPieces[pd.index].Length; j++) {
                    lp = levelPieces[pd.index][j];
                    if (lp.parent == firstParent) {
                        lp.parent = 0;
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
                int nextParent = firstParent + 3;
                float nextOffsetX = nextParent - 1f;
                PlaceLevelPiece(nextParent, pd.index, currLevel.endpointX*nextOffsetX, currLevel.endpointY, pd.positionX,
                pd.positionY, pd.positionZ, pd.rotationX, pd.rotationY, pd.rotationZ, pd.scaleX, pd.scaleY, pd.scaleZ, pd.extraValues);
            }
            firstParent++;
        }
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
