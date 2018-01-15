using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : GameObjectExt {

	public float extraValue1 = 0;
	public float extraValue2 = 0;

	public override float[] GetExtraValues() {
		return new float[]{extraValue1, extraValue2};
	}

	public override void SetExtraValues(float[] ev) {
		extraValue1 = ev[0];
		extraValue2 = ev[1];
	}

}
