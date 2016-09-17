using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ClickableDetailed : ClickableRule {
	public string ruleName;
	public ClickableExercise originalExercise;
	public bool implicationLeftwards;

	//Startingpoint is model from base class
	public EquationPart target;
	public Equation assumption;

	void Start(){
		draw (ruleName);
		GetComponent<LayoutElement> ().preferredWidth = GetComponent<RectTransform> ().sizeDelta.x;
		GetComponent<LayoutElement> ().preferredHeight = GetComponent<RectTransform> ().sizeDelta.y;
	}


	public override void clicked(){

		NodeInteractor interactor = (Overview.inst.autoAssoc.isChecked () ? new AutoAssocInteractor () : new BasicInteractor ());
		CNDictionary varToArbitraryDict = GetComponentInParent<ExerciseDetails> ().varToArbitraryDict;

		//model va already applied from the ExerciseDetails-Object (otherwise the buttons would have shown x's instead of a's)
		target = target.clone ().apply (varToArbitraryDict);

		if (assumption != null) {
			assumption = (Equation)assumption.clone ().apply (varToArbitraryDict);
			HolderAssumptions.inst.setAssumption (assumption);
		}

		Worksheet.inst.setExercise (interactor, originalExercise, (EquationPart) model, target, implicationLeftwards);
		GameManager.inst.focusExercise ();
		Overview.inst.GetComponentInChildren<HolderAxiomsAndExercises>().rulesSetClickable(true); 

		GetComponentInParent<ExerciseDetails> ().doHide ();
	}
}
