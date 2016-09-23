using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class iExercise : iRule {

	/*introText
	 * 
--- Zero is unique ---

How many numbers has the property, that when you add it to any number, you get that same number? The number zero has this property, but are there any other?

Lets assume that there is a non-zero number with the property, and call it o. It then turns out that 0 and o are equal. This means that o is zero, and this contradict our assumption. So zero is the only number with the property!

Now showing that 0=o is up to you...

--- Inverses are unique ---

Given a number a, how many inverses exists? a' is one since a+a'=0, but are there others? Lets assume so, and call it â. Show that â=a'
	 * 
	 * */

	public string exerciseName;

	[TextArea(3,10)]
	public string detailedInfo;
	public iExercise[] becomesPlayableOnCompletionOf;
	[Range(0, 2)]
	public int completionStage; //0 = not playable, 1 = playable but not completed, 2 = completely solved
	public string varsToKeep;
	public bool isUsable;

	public virtual void Start(){
		draw (exerciseName);
	}

	public override void clicked(){

		if (GameManager.inst.exerciseMode) {
			base.clicked ();
		} else {
			if (completionStage == 0) return;

			Overview.inst.GetComponentInChildren<ExerciseDetails> ().doAppear(this);
		}
	}

	public void completed(){
		completionStage = 2;

		GetComponent<Image> ().sprite = GetComponentInParent<ContainerIAxiomIExercise> ().backgroundOnCompletion;
		GetComponentInParent<ContainerIAxiomIExercise> ().updateUnlockables ();

		iEquivalence biimp = GetComponentInParent<iEquivalence> ();
		if (biimp != null && !(this is iEquivalence)) biimp.tryComplete ();
	}

	public virtual void setupForMode(bool exerciseMode){
		bool interactable = true;
		if (exerciseMode) {
			if (!isUsable) interactable = false;
			if (completionStage != 2) interactable = false;
			if (model is Implication && !Overview.inst.impChain.isChecked()) interactable = false;

		} else {
			if (completionStage == 0) interactable = false;

		}

		GetComponent<Button> ().interactable = interactable;

	}

}
