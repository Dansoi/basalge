using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ExerciseDetails : MonoBehaviour {

	public static ExerciseDetails inst;
	public bool equationModeUnlocked;
	public GameObject clickableDetailedPrefab;
	public Transform equationSpecifics;
	public Transform equationLayoutGroup;
	public Transform implicationSpecifics;
	public Transform implicationLayoutGroupLeft;
	public Transform implicationLayoutGroupRight;
	public CNDictionary varToArbitraryDict;

	void Awake(){
		inst = this;
	}
	private void addVariant(Transform addTo, ClickableExercise rule, BasicModel startingPoint, BasicModel target, BasicModel assumption = null, bool implicationLeftwards = false, string ruleName = ""){
		Debug.Assert ((startingPoint is Equation && target is Equation) || (startingPoint is Expression && target is Expression));

		EquationPart _startingPoint = (EquationPart)startingPoint;
		EquationPart _target = (EquationPart)target;
		Equation _assumption = (Equation)assumption;

		GameObject detailedObj = Instantiate (clickableDetailedPrefab);
		ClickableDetailed cd = detailedObj.GetComponent<ClickableDetailed> ();
		cd.originalExercise = rule;
		cd.model = _startingPoint.clone ().apply (varToArbitraryDict);
		cd.target = _target;
		cd.assumption = _assumption;
		cd.ruleName = ruleName;
		cd.implicationLeftwards = implicationLeftwards;

		detailedObj.transform.SetParent(addTo);

		if (rule.completionStage == 3 || (rule.completionStage == 2 && (rule.isLeftwayCompleted == implicationLeftwards))) {
			detailedObj.GetComponent<Image> ().sprite = HolderAxiomsAndExercises.inst.backgroundOnCompletion;
		}

	}


	public void doAppear (ClickableExercise rule) {
		varToArbitraryDict = new CNDictionary ();
		for (char c = 'x'; c <= 'z'; c++) {
			if (rule.varsToKeep.IndexOf (c) == -1) varToArbitraryDict.Add (c, new Element (-1f, (char) (c - 'x' + 'a'))  );
		}

		BasicModel model = rule.model;
		if (model is Equation) {
			equationSpecifics.gameObject.SetActive (true);
			implicationSpecifics.gameObject.SetActive (false);

			addVariant (equationLayoutGroup, rule, model [0], model [1]);
			addVariant (equationLayoutGroup, rule, model [1], model [0]);

		} else if (model is Implication && rule.varsToKeep != "") {
			equationSpecifics.gameObject.SetActive (true);
			implicationSpecifics.gameObject.SetActive (false);

			addVariant (equationLayoutGroup, rule, model [1][0], model [1][1], model[0]);
			addVariant (equationLayoutGroup, rule, model [1][1], model [1][0], model[0]);


		} else {
			Debug.Assert (model is Implication);
			equationSpecifics.gameObject.SetActive (false);
			implicationSpecifics.gameObject.SetActive (true);

			if(equationModeUnlocked) addVariant(implicationLayoutGroupRight, rule, model[0], model[1], null, false);
			addVariant(implicationLayoutGroupRight, rule, model[1][0], model[1][1], model[0], false);
			addVariant(implicationLayoutGroupRight, rule, model[1][1], model[1][0], model[0], false);

			if (((Implication)model).bothWays) {
				addVariant(implicationLayoutGroupLeft, rule, model[0][0], model[0][1], model[1], true);
				addVariant(implicationLayoutGroupLeft, rule, model[0][1], model[0][0], model[1], true);
				if(equationModeUnlocked) addVariant(implicationLayoutGroupLeft, rule, model[1], model[0], null, true);
			}
		}

		GetComponent<Image> ().enabled = true;
		GetComponent<Button> ().enabled = true;
		transform.GetChild (0).gameObject.SetActive (true);
		GetComponentInChildren<Text> ().text = rule.detailedInfo;
	}


	public void clickedOutsideButton(){
		doHide();
	}

	public void doHide() {
		GetComponent<Button> ().enabled = false;
		GetComponent<Image> ().enabled = false;
		foreach (Transform child in equationLayoutGroup) Destroy (child.gameObject);
		foreach (Transform child in implicationLayoutGroupLeft) Destroy (child.gameObject);
		foreach (Transform child in implicationLayoutGroupRight) Destroy (child.gameObject);
		transform.GetChild (0).gameObject.SetActive (false);
	}
}
