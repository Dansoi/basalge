using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ContainerIAxiomIExercise : MonoBehaviour {
	public static ContainerIAxiomIExercise inst;
	public Sprite backgroundOnCompletion;
	bool updateLayoutOnUpdate;

	public void Awake(){
		inst = this;
	}

	void Update () {
		if (!updateLayoutOnUpdate) return;
		updateLayoutOnUpdate = false;
		setLayout ();
	}
	public void setLayoutOnUpdate () {
		updateLayoutOnUpdate = true;
	}

	public void setLayout () {
		//Maybe layout the rules automatically at some point...
	}

	public void updateUnlockables(){
		bool mustUnlock;

		//Unlock exercises
		foreach (iExercise e in GetComponentsInChildren<iExercise>(true)) {
			if (e.completionStage != 0) continue;
			mustUnlock = true;
			foreach (iExercise prerequisite in e.becomesPlayableOnCompletionOf) {
				if (prerequisite.completionStage != 2) mustUnlock = false;
			}
			if (mustUnlock) e.completionStage = 1;
		}

		//Unlock AutoAssocToggler
		if(!Overview.inst.autoAssoc.isUnlocked){
			mustUnlock = true;
			foreach (iExercise prerequisite in Overview.inst.exercisesToUnlockAutoAssoc) {
				if (prerequisite.completionStage != 2) mustUnlock = false;
			}
			if(mustUnlock) Overview.inst.autoAssoc.unlock ();
		}

		//Unlock impChainToggler
		if(!Overview.inst.impChain.isUnlocked){
			mustUnlock = true;
			foreach (iExercise prerequisite in Overview.inst.exercisesToUnlockImpChain) {
				if (prerequisite.completionStage != 2) mustUnlock = false;
			}
			if(mustUnlock) Overview.inst.impChain.unlock ();
		}

	}

	public void rulesSetClickable(bool exerciseMode){
		foreach (iAxiom a in GetComponentsInChildren<iAxiom>()) {
			a.GetComponent<Button> ().interactable = exerciseMode;
		}

		Toggler autoAssoc = Overview.inst.autoAssoc;
		autoAssoc.GetComponentInParent<Button> ().interactable = !autoAssoc.isChecked() && exerciseMode;
		autoAssoc.gameObject.SetActive (!exerciseMode && autoAssoc.isUnlocked);

		Toggler impChain = Overview.inst.impChain;
		impChain.gameObject.SetActive(!exerciseMode && impChain.isUnlocked);


		foreach (iExerciseParser e in GetComponentsInChildren<iExerciseParser>()) {
			iExercise ce = e.GetComponent<iExercise> ();
			ce.setupForMode (exerciseMode);
		}


	}
}
