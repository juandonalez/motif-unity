using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelPieceScript : MonoBehaviour {

	public virtual float[] GetExtraValues() {
		return new float[0];
	}

	public virtual void Reset(float[] ev) {}

	public virtual void SetExtraValues(float[] ev) {}

}
