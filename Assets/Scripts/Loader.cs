using UnityEngine;
using System.Collections;

public class Loader : MonoBehaviour 
{
	public GameObject managerMain;

	void Awake() {
		if (ManagerMain.instance == null) {
			Instantiate(managerMain);
		}
	}
}