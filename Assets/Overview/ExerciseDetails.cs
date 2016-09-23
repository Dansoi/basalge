using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ExerciseDetails : MonoBehaviour {

	public static ExerciseDetails inst;
	public iExercise ce;

	void Awake(){
		inst = this;
	}

	public void clickedButton(){
		CNDictionary varToArbitraryDict = new CNDictionary ();
		for (char c = 'x'; c <= 'z'; c++) {
			if (ce.varsToKeep.IndexOf (c) == -1) varToArbitraryDict.Add (c, new Element (-1f, (char) (c - 'x' + 'a'))  );
		}

		AbstractInteractor interactor = (Overview.inst.autoAssoc.isChecked () ? new AutoAssocInteractor () : new BasicInteractor ());

		EquationPart firstNode, targetNode;
		BasicModel model = ce.model;

		if (model is Equation) {
			firstNode = (EquationPart) model [0].clone ().apply (varToArbitraryDict);
			targetNode = (EquationPart)  model [1].clone ().apply (varToArbitraryDict);
		} else if (ce.varsToKeep != "" || !Overview.inst.impChain.isChecked ()) {
			bool rightWards = ((Implication)model).bothWays || ((Implication)model).rightWay;
			firstNode = (EquationPart)model [rightWards ? 1 : 0] [0].clone ().apply (varToArbitraryDict);
			targetNode = (EquationPart)model [rightWards ? 1 : 0] [1].clone ().apply (varToArbitraryDict);
			ContainerIAssumption.inst.setAssumption ((Equation) model [rightWards ? 0 : 1].clone ().apply (varToArbitraryDict));
		} else {
			bool rightWards = ((Implication)model).bothWays || ((Implication)model).rightWay;
			firstNode = (EquationPart) model [rightWards ? 0 : 1].clone ().apply (varToArbitraryDict);
			targetNode = (EquationPart) model [rightWards ? 1 : 0].clone ().apply (varToArbitraryDict);
		}

		Worksheet.inst.setExercise (interactor, ce, firstNode, targetNode);
		GameManager.inst.focusExercise ();
		Overview.inst.GetComponentInChildren<ContainerIAxiomIExercise>().rulesSetClickable(true); 

		doHide ();

	}


	public void doAppear (iExercise rule) {
		ce = rule;

		if (rule.detailedInfo == "") {
			clickedButton ();
			return;
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
		transform.GetChild (0).gameObject.SetActive (false);
	}
}
