using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelPooler : MonoBehaviour {

	public Camera cam;
	public GameObject[][] objects;
	LevelPiece[] levelPieces;
	Transform[][] transforms;
	GameObjectExt[][] scripts;
	int endpointIndex = 2;
	Vector3[] endpointPositions;

	void Update() {
		for (int k = 0; k < endpointPositions.Length; k++) {
			Vector3 viewPos = cam.WorldToViewportPoint(endpointPositions[k]);
			if (viewPos.x < 0) {
				endpointPositions[k].x = 10000;
				endpointPositions[k].y = 0;
				endpointPositions[k].z = 0;
				for (int i = 0; i < objects.Length; i++) {
					for (int j = 0; j < objects[i].Length; j++) {
						if (objects[i][j].activeSelf) {
							viewPos = cam.WorldToViewportPoint(transforms[i][j].position);
							if (viewPos.x < -0.0000001) {
								Debug.Log(viewPos.x);
								transforms[i][j].localPosition = new Vector3(0, 0, 0);
								transforms[i][j].localEulerAngles = new Vector3(0, 0, 0);
								transforms[i][j].localScale = new Vector3(1, 1, 1);
								transforms[i][j].parent = null;
								objects[i][j].SetActive(false);
							}
						}
					}
				}
			}
		}
	}

	public void Initiate(GameObject[][] o) {
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
	}

	public void ActivateObject(Transform parent, int index, float pX, float pY, float pZ, float rX,
	float rY, float rZ, float sX, float sY, float sZ, float[] extraValues) {
		// if it is an endpoint, take a note of its position
		if (index == endpointIndex) {
			for (int i = 0 ; i < endpointPositions.Length; i++) {
				if (endpointPositions[i].x == 0 && endpointPositions[i].y == 0) {
					endpointPositions[i].x = pX;
					endpointPositions[i].y = pY;
					endpointPositions[i].z = pZ;
					break;
				}
			}
		}
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
