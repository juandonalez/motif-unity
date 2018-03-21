using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelData {
    public string name;
    public float height;
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
