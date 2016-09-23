using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class EndChain : MonoBehaviour {

	public EndChain otherEnd;
	public bool isTop;
	int numberOfSteps;

	public void Clear () {
		GetComponentInChildren<wLinksGroup> ().clear ();
		GetComponentInChildren<wStepsGroup> ().clear ();
		GetComponentInChildren<wRulesGroup> ().clear ();
		numberOfSteps = 0;
	}

	public void setEdge(EquationPart edgeNode){

		GameObject edgeStep = edgeNode.Instantiate (Builder.worksheetFontsize, true);
		Builder.emphasize(edgeStep.transform, Builder.inst.fadedClr);

		wNode.AddAsComponentTo (edgeStep, edgeNode);
		edgeStep.name = "Step 0";
		float edgeHeight = edgeStep.GetComponent<RectTransform> ().sizeDelta.y;
		float edgeYPos = isTop ? -(0.5f * edgeHeight) : (0.5f * edgeHeight);

		if (!isTop) {
			string linkStr = edgeNode is Expression ? "=" : "\u21d3";

			GameObject edgeLink = wLink.InstantiateWith(Builder.worksheetFontsize, linkStr, "Link 1"); //Link 1 because isTop is false
			Builder.emphasize(edgeLink.transform, Color.white);

			float linkYPos = (edgeNode is Expression) ? edgeYPos : edgeHeight + Builder.spaceBetweenWorklines*0.5f;
			GetComponentInChildren<wLinksGroup> ().addLink (edgeLink, linkYPos);
		}
		GetComponentInChildren<wStepsGroup> ().addStep(edgeStep, edgeYPos, edgeNode is Expression);

		GetComponentInChildren<LayoutElement> ().preferredHeight = edgeHeight;

		numberOfSteps = 1;
	}

	//Returns how much the preferredHeight has grown
	public float add (EquationPart newNode, GameObject newHint){
		GetComponentInChildren<wStepsGroup> ().stepSetActive ("Step " + (numberOfSteps - 1), false);

		GameObject newStep = newNode.Instantiate (Builder.worksheetFontsize, true);
		Builder.emphasize(newStep.transform, Builder.inst.fadedClr);

		float thisOldHeight = GetComponent<LayoutGroup>().preferredHeight;
		float newStepHeight = newStep.GetComponent<RectTransform> ().sizeDelta.y;
		float spaceBetweenSteps = getSpacing (
			GetComponentInChildren<wStepsGroup> ().getHeightOf("Step " + (numberOfSteps - 1)), //previous step height
			newStepHeight,
			newHint.GetComponent<RectTransform> ().sizeDelta.y); //rule used height

		float stepYPos = thisOldHeight + spaceBetweenSteps + 0.5f*newStepHeight;
		float ruleYPos = thisOldHeight + 0.5f * spaceBetweenSteps;
		if (isTop) {
			stepYPos = -stepYPos;
			ruleYPos = -ruleYPos;
		}

		float linkYPos = stepYPos;
		if (newNode is Equation) linkYPos += 0.5f * newStepHeight + 0.5f * Builder.spaceBetweenWorklines;


		string linkStr = (newNode is Expression ? "=" : "\u21d3");
		string linkName = "Link " + (isTop ? numberOfSteps : numberOfSteps + 1);
		GameObject newLink = wLink.InstantiateWith(Builder.worksheetFontsize, linkStr, linkName);
		Builder.emphasize(newLink.transform, Color.white);
		GetComponentInChildren<wLinksGroup> ().addLink (newLink, linkYPos);

		wNode.AddAsComponentTo (newStep, newNode);
		newStep.name = "Step " + numberOfSteps;
		GetComponentInChildren<wStepsGroup> ().addStep (newStep, stepYPos, newNode is Expression);

		newHint.name = "Used Rule " + numberOfSteps;
		GetComponentInChildren<wRulesGroup> ().addRule (newHint, ruleYPos);


		++numberOfSteps;
		GetComponentInChildren<LayoutElement> ().preferredHeight = thisOldHeight + spaceBetweenSteps + newStepHeight;

		return spaceBetweenSteps + newStepHeight;
	}

	public void undo(int stepNo){
		foreach(Transform link in GetComponentInChildren<wLinksGroup>().transform){
			if (int.Parse (link.name.Substring (5)) >= (isTop ? stepNo : stepNo+1)) Destroy (link.gameObject);
		}
		foreach(Transform step in GetComponentInChildren<wStepsGroup>().transform){
			if (int.Parse (step.name.Substring (5)) >= stepNo) Destroy (step.gameObject);
		}
		foreach(Transform rule in GetComponentInChildren<wRulesGroup>().transform){
			if (int.Parse (rule.name.Substring (10)) >= stepNo) Destroy (rule.gameObject);
		}
		GetComponentInChildren<wStepsGroup> ().stepSetActive ("Step " + (stepNo - 1), true);
		numberOfSteps = stepNo;
		shrinkHeight ();
	}

	private void shrinkHeight(){
		RectTransform lastStep = GetComponentInChildren<wStepsGroup> ().transform.FindChild ("Step " + (numberOfSteps - 1)).GetComponent<RectTransform>();
		float newHeight;
		if (isTop) {
			newHeight = -lastStep.anchoredPosition.y + lastStep.rect.height * 0.5f;
		} else {
			newHeight = lastStep.anchoredPosition.y + lastStep.rect.height * 0.5f;
		}
		GetComponentInChildren<LayoutElement> ().preferredHeight = newHeight;
	}

	private float getSpacing(float aHeight, float bHeight, float ruleHeight){
		float minABHeight = (aHeight < bHeight ? aHeight : bHeight);

		float space = ruleHeight - minABHeight;
		if (space < Builder.spaceBetweenWorklines)
			space = Builder.spaceBetweenWorklines;
		return space;
	}

	public GameObject getLastStep(){
		return GetComponentInChildren<wStepsGroup> ().transform.FindChild ("Step " + (numberOfSteps - 1)).gameObject;
	}

	public void addHintAndFreeze(GameObject newHint){
		GetComponentInChildren<wStepsGroup> ().stepSetActive ("Step " + (numberOfSteps - 1), false);

		float thisOldHeight = GetComponent<LayoutGroup>().preferredHeight;
		float thisStepHeight = GetComponentInChildren<wStepsGroup> ().getHeightOf ("Step " + (numberOfSteps - 1));
		float otherStepHeight = otherEnd.getLastStep ().GetComponent<RectTransform> ().sizeDelta.y;
		float ruleHeight = newHint.GetComponent<RectTransform> ().sizeDelta.y;

		float spaceBetweenSteps = getSpacing (thisStepHeight, otherStepHeight, ruleHeight);
		float ruleYPos = thisOldHeight + 0.5f * spaceBetweenSteps;
		if (isTop) ruleYPos = -ruleYPos;

		newHint.name = "Used Rule " + numberOfSteps;
		GetComponentInChildren<wRulesGroup> ().addRule (newHint, ruleYPos);

		GetComponentInChildren<LayoutElement> ().preferredHeight = thisOldHeight + spaceBetweenSteps;

		GetComponentInChildren<wLinksGroup> ().freeze();
	}

	public void freeze(){
		GetComponentInChildren<wStepsGroup> ().stepSetActive ("Step " + (numberOfSteps - 1), false);
		GetComponentInChildren<wLinksGroup> ().freeze();
	}




	/*
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
	*/

}
