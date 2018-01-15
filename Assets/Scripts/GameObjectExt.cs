using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectExt : MonoBehaviour {

	public virtual float[] GetExtraValues() {
		return new float[0];
	}

	public virtual void SetExtraValues(float[] ev) {}

}
