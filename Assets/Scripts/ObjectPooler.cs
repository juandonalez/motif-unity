using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour {

	public GameObject[][] objects;
	Transform[][] transforms;
	GameObjectExt[][] scripts;

	public void Initiate(GameObject[][] o) {
		objects = o;
		transforms = new Transform[o.Length][];
		scripts = new GameObjectExt[o.Length][];
		for (int i = 0; i < o.Length; i++) {
			transforms[i] = new Transform[o[i].Length];
			scripts[i] = new GameObjectExt[o[i].Length];
			for (int j = 0; j < o[i].Length; j++) {
				transforms[i][j] = o[i][j].transform;
				scripts[i][j] = o[i][j].GetComponent<GameObjectExt>();
			}
		}
	}

	public void ActivateObject(Transform parent, int index, float pX, float pY, float pZ, float rX,
	float rY, float rZ, float sX, float sY, float sZ, float[] extraValues) {
		for (int i = 0; i < objects[index].Length; i++) {
			if (!objects[index][i].activeSelf) {
				transforms[index][i].parent = parent;
				transforms[index][i].Translate(pX, pY, pZ);
				transforms[index][i].Rotate(rX, rY, rZ);
				transforms[index][i].localScale = new Vector3(sX, sY, sZ);
				if (extraValues != null) {
					scripts[index][i].SetExtraValues(extraValues);
				}
				objects[index][i].SetActive(true);
				break;
			}
		}
	}

}
