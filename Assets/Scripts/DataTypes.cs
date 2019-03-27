using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelData {
    public string name;
    public float endpointX;
    public float endpointY;
    public PrefabData[] prefabDatas;
}

[System.Serializable]
public class PrefabData {
    public string name;
    public int index;
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

public class LevelPiece {
    public GameObject prefab;
    public Transform transform;
    public LevelPieceScript script;
    public int parent;
}
