using UnityEngine;
using System.Collections;

public class Loader : MonoBehaviour 
{
	public GameObject mainManager;

	void Awake() {
		if (MainManager.instance == null) {
			Instantiate(mainManager);
		}
	}
}