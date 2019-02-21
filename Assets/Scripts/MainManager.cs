using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainManager : MonoBehaviour {

	public static MainManager instance = null;

	void Awake() {
		// creates a singleton manager that persists between scenes
		if (instance == null) {
			instance = this;
		}
		else if (instance != this) {
			Destroy(gameObject);
		}
		DontDestroyOnLoad(gameObject);
	}

	void Start () {
		
	}
	
	void Update () {
		
	}
}
