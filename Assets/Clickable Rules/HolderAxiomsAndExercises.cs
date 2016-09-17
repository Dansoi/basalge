using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class HolderAxiomsAndExercises : RuleHolderAbstract {
	public static HolderAxiomsAndExercises inst;
	public Sprite backgroundOnCompletion;
	public List<ClickableExercise> exercisesToUnlockAutoAssoc;
	public List<ClickableExercise> exercisesToUnlockEquationMode;

	public void Awake(){
		inst = this;
	}

	public override void setLayout () {
		//Maybe layout the rules automatically at some point...
	}

	public void updateUnlockables(){
		bool mustUnlock;

		//Unlock exercises
		foreach (ClickableExercise e in GetComponentsInChildren<ClickableExercise>()) {
			if (e.completionStage != 0) continue;
			mustUnlock = true;
			foreach (ClickableExercise prerequisite in e.becomesPlayableOnCompletionOf) {
				if (prerequisite.completionStage != 3) mustUnlock = false;
			}
			if (mustUnlock) e.completionStage = 1;
		}

		//Unlock AutoAssocToggler
		if(!Overview.inst.autoAssoc.isUnlocked){
			mustUnlock = true;
			foreach (ClickableExercise prerequisite in exercisesToUnlockAutoAssoc) {
				if (prerequisite.completionStage != 3) mustUnlock = false;
			}
			if(mustUnlock) Overview.inst.autoAssoc.unlock ();
		}

		//Unlock Equation-mode
		if(!ExerciseDetails.inst.equationModeUnlocked){
			mustUnlock = true;
			foreach (ClickableExercise prerequisite in exercisesToUnlockEquationMode) {
				if (prerequisite.completionStage != 3) mustUnlock = false;
			}
			if (mustUnlock) ExerciseDetails.inst.equationModeUnlocked = true;
		}
	}

	public void rulesSetClickable(bool exerciseMode){
		foreach (ClickableAxiom a in GetComponentsInChildren<ClickableAxiom>()) {
			a.GetComponent<Button> ().interactable = exerciseMode;
		}

		Toggler autoAssoc = Overview.inst.autoAssoc;
		autoAssoc.setEnable (!exerciseMode && autoAssoc.isUnlocked);
		autoAssoc.GetComponentInParent<Button> ().interactable = !autoAssoc.isChecked() && exerciseMode;


		foreach (ClickableExercise e in GetComponentsInChildren<ClickableExercise>()) {
			bool interactable = true;
			if (exerciseMode) {
				if (!e.isUsable) interactable = false;
				if (e.completionStage < 2) interactable = false;
				if (e.model is Implication && !ExerciseDetails.inst.equationModeUnlocked) interactable = false;

			} else {
				if (e.completionStage < 1) interactable = false;

			}

			e.GetComponent<Button> ().interactable = interactable;
		}


	}
}
