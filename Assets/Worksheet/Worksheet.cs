﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;



public class Worksheet : MonoBehaviour {
	public static Worksheet inst; //instance

	public EndChain topChain;
	public MiddleDots middleDots;
	public EndChain bottomChain;
	public GameObject targetReachedObj;

	iExercise exercise;
	public AbstractInteractor interactor;


	const float lowerScrollLimit = -120f;
	const float upperScrollLimit = 120f;


	void Awake(){
		inst = this;
		//interactor = new BasicInteractor ();
	}



	void Update() {
		if(interactor != null) interactor.Update ();

		if (GetComponentInParent<Canvas> ().worldCamera.ScreenToViewportPoint (Input.mousePosition).y > 1f) return;
		float scrollWheel = Input.GetAxis ("Mouse ScrollWheel");
		if (scrollWheel != 0f) {
			move (-200f * scrollWheel);
		}
	}

	public void clear(){
		topChain.Clear ();
		bottomChain.Clear ();
		ContainerIAssumption.inst.Clear ();
		targetReachedObj.GetComponent<Text> ().enabled = false;
		targetReachedObj.GetComponent<Button> ().enabled = false;
	}

	public void setExercise(AbstractInteractor interactor, iExercise exercise, EquationPart firstNode, EquationPart targetNode){
		this.interactor = interactor;
		this.exercise = exercise;

		RectTransform thisRT = GetComponent<RectTransform> ();
		thisRT.pivot = new Vector2 (0.5f, 1f);
		thisRT.anchoredPosition = new Vector2 (0f, 100f);


		topChain.setEdge (firstNode);
		middleDots.setup (firstNode is Expression ? "=" : "\u21d3", firstNode is Equation);
		Builder.emphasize(middleDots.transform, Builder.inst.fadedClr);
		bottomChain.setEdge (targetNode);
	}


	public void nextStep(EquationPart newNode, GameObject ruleUsed, bool addToTop){
		Builder.emphasize (ruleUsed.transform, Builder.inst.fadedClr);

		EndChain thisChain = (addToTop ? topChain : bottomChain);
		EndChain otherChain = (addToTop ? bottomChain : topChain);

		EquationPart otherLast = otherChain.getLastStep ().GetComponent<wNode> ().node;
		if (otherLast.Equals( newNode )) {

			CRList newThisMatches = ruleUsed.GetComponent<WorkRule> ().towardsDotsRangeList;
			CRList newOtherMatches = otherLast.createColoredRangeListLikeIn (newThisMatches, newNode);
			ruleUsed.GetComponent<WorkRule> ().towardsDotsRangeList = newOtherMatches;

			thisChain.addHintAndFreeze (ruleUsed);
			middleDots.Clear ();

			otherChain.freeze();
			ContainerIAssumption.inst.Clear ();
			targetReachedObj.GetComponent<Text> ().enabled = true;
			targetReachedObj.GetComponent<Button> ().enabled = true;
			exercise.completed ();

			return;
		}


		float extraHeight = thisChain.add (newNode, ruleUsed);

		RectTransform thisRT = GetComponent<RectTransform>();
		float anchorToDotsOld = thisRT.offsetMax.y + middleDots.GetComponent<RectTransform>().anchoredPosition.y; //old is part of name because the ContentFitter-component hasn't worked out the layout yet.


		//Positioning ("work down" (pushing bottom eq's down) if working on the top chain. "work up" if working on the bottom chain)
		float oldPivotYRect = thisRT.rect.height * thisRT.pivot.y;
		float newPivotYRect = (addToTop ? thisRT.rect.height : 0f);
		if (oldPivotYRect != newPivotYRect) {
			thisRT.anchoredPosition += new Vector2(0f, newPivotYRect - oldPivotYRect);
			thisRT.pivot = new Vector2 (0.5f, addToTop ? 1f : 0f);
		}
		//Will the dots get to high/low? Then adjust it back.
		if (addToTop && anchorToDotsOld - extraHeight < lowerScrollLimit) {
			thisRT.anchoredPosition += new Vector2(0f, lowerScrollLimit - anchorToDotsOld + extraHeight);
		} else if(!addToTop && anchorToDotsOld + extraHeight > upperScrollLimit) {
			thisRT.anchoredPosition += new Vector2(0f, upperScrollLimit - anchorToDotsOld - extraHeight);
		}

	}

	//Changes made (to the layout) after calling move will ruin the layout (So make changes before calling move())
	public void move(float amount){
		RectTransform thisRT = GetComponent<RectTransform>();
		if (thisRT.offsetMax.y + amount < lowerScrollLimit) amount = lowerScrollLimit - thisRT.offsetMax.y;
		if (thisRT.offsetMin.y + amount > upperScrollLimit) amount = upperScrollLimit - thisRT.offsetMin.y;

		thisRT.anchoredPosition += new Vector2 (0f, amount);
	}
}

/*
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Worksheet : MonoBehaviour {
	public static Worksheet inst; //instance

	public GameObject undoObj;
	public GameObject targetReachedObj;
	ClickableExercise exercise;
	bool implicationLeftwards;

	public NodeInteractor interactor;
	int numberOfSteps;
	EquationPart target;

	void Awake(){
		inst = this;
		//interactor = new BasicInteractor ();
	}


	void Update() {
		if(interactor != null) interactor.Update ();
		
		if (GetComponentInParent<Canvas> ().worldCamera.ScreenToViewportPoint (Input.mousePosition).y > 1f) return;
		float scrollWheel = Input.GetAxis ("Mouse ScrollWheel");
		if (scrollWheel != 0f) {
			move (-200f * scrollWheel);
		}
	}
	public void move(float amount){
		Vector3 newPos = transform.position + Vector3.up * amount;
		if (newPos.y < -100f) newPos.y = -100f;
		float height = GetComponent<RectTransform> ().sizeDelta.y;
		if (newPos.y - height > 130f) newPos.y = height + 130f;
		transform.position = newPos;
	}

	public void clear(){
		GetComponentInChildren<ColumnLinks> ().clear ();
		GetComponentInChildren<ColumnSteps> ().clear ();
		GetComponentInChildren<Target> ().clear ();
		GetComponentInChildren<ColumnRules> ().clear ();
		HolderAssumptions.inst.Clear ();
		targetReachedObj.GetComponent<Text> ().enabled = false;
		targetReachedObj.GetComponent<Button> ().enabled = false;
	}
	public void fixSize(float newHeight){
		float stepWidth = GetComponentInChildren<ColumnSteps> ().GetComponent<RectTransform> ().sizeDelta.x;
		float rulesWidth = GetComponentInChildren<ColumnRules> ().GetComponent<RectTransform> ().sizeDelta.x;

		GetComponent<RectTransform>().sizeDelta = new Vector2(stepWidth + 60f + rulesWidth, newHeight);
	}
		
	public void setExercise(NodeInteractor interactor, ClickableExercise exercise, EquationPart firstNode, EquationPart target, bool implicationLeftwards){
		this.interactor = interactor;
		this.exercise = exercise;
		this.target = target;
		this.implicationLeftwards = implicationLeftwards;

		transform.localPosition = new Vector3 (0f, 100f, 0f);

		GameObject firstStep = firstNode.Instantiate (Builder.worksheetFontsize, true);
		WorkNode.AddAsComponentTo (firstStep, firstNode);
		firstStep.name = "Step 0";
		float firstStepHeight = firstStep.GetComponent<RectTransform> ().sizeDelta.y;

		GameObject firstLink = Builder.InstantiateAtomic (Builder.worksheetFontsize, " ", "Link 0");
		GetComponentInChildren<ColumnLinks> ().addLink(firstLink, 0.5f*firstStepHeight);

		GetComponentInChildren<ColumnSteps> ().addStep (firstStep, 0f, firstNode is Expression);

		fixSize (firstStepHeight);

		numberOfSteps = 1;
		showUndoAndSetInteractable ();


		GetComponentInChildren<Target> ().SetTarget (target, Builder.worksheetFontsize, target is Expression ? "=" : "\u21d2");
	}

	public void nextStep(EquationPart newNode, GameObject ruleUsed){
		GetComponentInChildren<ColumnSteps> ().stepSetActive ("Step " + (numberOfSteps - 1), false);
		bool reached = newNode.Equals(target);

		GameObject newStep = newNode.Instantiate (Builder.worksheetFontsize, true);
		newStep.name = "Step " + numberOfSteps;
		if(!reached) WorkNode.AddAsComponentTo (newStep, newNode);

		float thisOldHeight = GetComponent<RectTransform> ().sizeDelta.y;
		float newStepHeight = newStep.GetComponent<RectTransform> ().sizeDelta.y;

		float spaceBetweenSteps = getSpacing (
			                          GetComponentInChildren<ColumnSteps> ().transform.FindChild ("Step " + (numberOfSteps - 1)).GetComponent<RectTransform> ().sizeDelta.y, //previous step height
			                          newStepHeight,
			                          ruleUsed.GetComponent<RectTransform> ().sizeDelta.y); //rule used height

		ruleUsed.name = "Used Rule " + numberOfSteps.ToString();
		GetComponentInChildren<ColumnRules> ().addRule (ruleUsed, -thisOldHeight - 0.5f*spaceBetweenSteps);

		string linkStr = (newNode is Expression ? "=" : "\u21d2");
		GameObject newLink = Builder.InstantiateAtomic (Builder.worksheetFontsize, linkStr, "Link " + numberOfSteps.ToString());
		GetComponentInChildren<ColumnLinks> ().addLink (newLink, -thisOldHeight - spaceBetweenSteps - 0.5f*newStepHeight);

		GetComponentInChildren<ColumnSteps> ().addStep (newStep, -thisOldHeight - spaceBetweenSteps, newNode is Expression);
		++numberOfSteps;

		float thisNewHeight = thisOldHeight + spaceBetweenSteps + newStepHeight;
		fixSize (thisNewHeight);

		if (transform.position.y - thisNewHeight < -100f) {
			move (thisNewHeight - transform.position.y - 100f);
		}

		if (reached) {
			GetComponentInChildren<Target> ().clear ();
			hideUndo ();
			HolderAssumptions.inst.Clear ();
			targetReachedObj.GetComponent<Text> ().enabled = true;
			targetReachedObj.GetComponent<Button> ().enabled = true;
			exercise.completed (implicationLeftwards);


		} else {
			showUndoAndSetInteractable ();
		}

	}

	private float getSpacing(float aHeight, float bHeight, float ruleHeight){
		float minABHeight = (aHeight < bHeight ? aHeight : bHeight);

		float space = ruleHeight - minABHeight;
		if (space < Builder.spaceBetweenWorklines)
			space = Builder.spaceBetweenWorklines;
		return space;
	}

	public void undo(){
		if (numberOfSteps <= 1) return;
		--numberOfSteps;



		GetComponentInChildren<ColumnLinks> ().removeLink ("Link " + numberOfSteps.ToString());
		float deletedStepHeight = GetComponentInChildren<ColumnSteps> ().removeStep ("Step " + numberOfSteps.ToString());
		float deletedRuleHeight = GetComponentInChildren<ColumnRules> ().removeRule ("Used Rule " + numberOfSteps.ToString());

		string nowLastStepID = "Step " + (numberOfSteps - 1).ToString();
		GetComponentInChildren<ColumnSteps> ().stepSetActive (nowLastStepID, true);
		float nowLastStepHeight = GetComponentInChildren<ColumnSteps> ().transform.FindChild (nowLastStepID).GetComponent<RectTransform> ().sizeDelta.y;

		float spacing = getSpacing (nowLastStepHeight, deletedStepHeight, deletedRuleHeight);
		float thisNewHeight = GetComponent<RectTransform> ().sizeDelta.y - deletedStepHeight - spacing;

		fixSize (thisNewHeight);
		move (0f); //If undoing caused the last visible expression to dissappear, scroll up a bit through this method, to reveal the now-latest expression


		showUndoAndSetInteractable ();
	}
	public void hideUndo(){
		undoObj.GetComponent<Text> ().enabled = false;
		undoObj.GetComponent<Button> ().enabled = false;
	}
	public void showUndoAndSetInteractable(){
		undoObj.GetComponent<Text> ().enabled = true;
		undoObj.GetComponent<Button> ().enabled = true;
		undoObj.GetComponent<Button> ().interactable = (numberOfSteps >= 2);
	}

}
*/