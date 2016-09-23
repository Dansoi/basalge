using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class iEquivalence : iExercise {

	public iExercise rightwards;
	public iExercise leftwards;


	public override void Start(){
		draw (exerciseName);
		rightwards.transform.SetAsLastSibling ();
		leftwards.transform.SetAsLastSibling ();
	}

	public void onMouseOver(){
		if(!GameManager.inst.exerciseMode) childrenDoAppear ();
	}

	void childrenDoAppear(){
		leftwards.gameObject.SetActive (true);
		rightwards.gameObject.SetActive (true);
	}

	public void onMouseExit(){
		if(!GameManager.inst.exerciseMode) childrenDoHide ();
	}

	void childrenDoHide(){
		leftwards.gameObject.SetActive (false);
		rightwards.gameObject.SetActive (false);
	}

	public void tryComplete(){ //Complete if both childs are completed
		if (rightwards.completionStage == 2 && leftwards.completionStage == 2) completed ();
	}

	public override void setupForMode(bool exerciseMode){
		leftwards.GetComponent<iExercise> ().setupForMode (exerciseMode);
		rightwards.GetComponent<iExercise> ().setupForMode (exerciseMode);

		bool interactable = true;
		if (exerciseMode) {
			if (!isUsable || !Overview.inst.impChain.isChecked () || completionStage == 0) {
				interactable = false;
				childrenDoHide ();
			} else if (completionStage == 2) {
				interactable = true;
				childrenDoHide ();
			} else if (leftwards.completionStage == 2 || rightwards.completionStage == 2) {
				interactable = false;
				childrenDoAppear ();
			} else {
				interactable = false;
				childrenDoHide ();
			}
				

		} else {
			if (completionStage == 0) interactable = false;
			childrenDoHide ();
		}

		GetComponent<Button> ().interactable = interactable;


	}	
}
