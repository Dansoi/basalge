using UnityEngine;
using System.Collections;

public class iExerciseParser : MonoBehaviour {

	public string modelStr;

	void Awake () {
		iExercise ce = GetComponent<iExercise>();
		BasicModel model = BasicModel.parse (modelStr);
		ce.model = model;

		if (ce is iEquivalence) {
			iEquivalence casted = (iEquivalence)ce;
			casted.rightwards.model = new Implication (model.childList, false, true);
			casted.leftwards.model = new Implication (model.childList, false, false);
		}
	}
}
