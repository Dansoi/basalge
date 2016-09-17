using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ClickableExercise : ClickableRule {

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
	public string ruleName;
	public string varsToKeep;
	public string modelStr;
	public bool isUsable;
	[TextArea(3,10)]
	public string detailedInfo;
	public ClickableExercise[] becomesPlayableOnCompletionOf;

	[Range(0, 3)]
	public int completionStage; //0 = not playable, 1 = playable but nothing completed, 2 = something completed for use as rule, 3 = completely solved

	public bool isLeftwayCompleted;
	public bool isRightwayCompleted;



	void Start() {
		model = BasicModel.parse (modelStr);
		draw (ruleName);
	}

	public override void clicked(){

		if (GameManager.inst.exerciseMode) {
			base.clicked ();
		} else {
			if (completionStage == 0) return;

			Overview.inst.GetComponentInChildren<ExerciseDetails> ().doAppear(this);
		}
	}

	public void completed( bool implicationLeftwards){
		if (model is Implication && ((Implication)model).bothWays) {
			if (implicationLeftwards) isLeftwayCompleted = true;
			else isRightwayCompleted = true;
		
			if (isLeftwayCompleted && isRightwayCompleted) completionStage = 3;
			else completionStage = 2;
		} else {
			completionStage = 3;
		}

		if (completionStage == 3) {
			GetComponent<Image> ().sprite = GetComponentInParent<HolderAxiomsAndExercises> ().backgroundOnCompletion;
			GetComponentInParent<HolderAxiomsAndExercises> ().updateUnlockables ();
		}

	}

}
