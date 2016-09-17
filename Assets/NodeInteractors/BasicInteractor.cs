using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class BasicInteractor : NodeInteractor {

	public EventHandler mouseOver;
	public bool hoveredLastFrame;
	public EquationPart matchingNode;

	public override void Update(){
		if (mouseOver == null && hoveredLastFrame) {
			hoveredLastFrame = false;
			AdaptingRule a = AdaptingRule.inst;
			if (a != null) {
				a.clearAndApply (false, true);
			}
			emphasizeMatch (Color.white);
		}
	}

	public virtual void emphasizeMatch(Color clr){
		if (matchingNode != null) {
			Builder.emphasize (matchingNode.nodeRectTrn, clr);
		}
	}


	public override void mouseEnter (EventHandler eh) {
		if (AdaptingRule.inst == null) return;
		mouseOver = eh;
		hoveredLastFrame = true;
		if(matchingNode != null) emphasizeMatch (Color.white);

		EquationPart hoverNode = eh.GetComponentInParent<WorkNode> ().node;
		CNDictionary dict = setMatchingNodeAndGetDictionary (hoverNode); //sets matchingNode field
		applyAndPaintAdaptingRule (dict);

		emphasizeMatch(Color.red);
	}

	public void applyAndPaintAdaptingRule(CNDictionary dict){
		if (dict == null) {
			AdaptingRule.inst.clearAndApply (false, true);
			return;
		}

		AdaptingRule.inst.dict = dict;
		AdaptingRule.inst.applyDictionary();
		AdaptingRule.inst.repaint();
	}

	public override void mouseReEnter () {
		if (mouseOver != null) {
			mouseEnter (mouseOver);
		} else {
			AdaptingRule a = AdaptingRule.inst;
			if (a) a.clearAndApply (false, true);
		}
	}


	//It could be that onMouseExit(nodeA) and onMouseEnter(nodeB) is called between the same two frames. Which goes first? I dont know, we have to make it work no matter which goes first. This way, hoverNode will be the correct node after both methods are called, regardless of order.
	public override void mouseExit(EventHandler eh){
		if (mouseOver == eh) {
			mouseOver = null;
		}
	}

	public override void mouseClick (){
		if (matchingNode == null) return;
		emphasizeMatch (Color.white);

		//Klippe-klistre nodes
		List<EquationPart> matchList = new List<EquationPart> ();
		matchList.Add (matchingNode);
		EquationPart oldRoot = matchingNode.getRoot ();
		EquationPart clonedRoot = oldRoot.clone ();
		EquationPart clonedMatch = clonedRoot.createNodeListLikeIn (matchList, oldRoot)[0];
		EquationPart replacer = AdaptingRule.inst.appliedRoot[1].clone(); //[1] because we need the 'replacer'-side
		EquationPart newRoot = clonedMatch.replaceWith (replacer).getRoot ();
		//Færdig med klippe-klistre. Nu har vi newRoot.


		BasicModel ruleUsed = cannibalizeAdaptedRuleForUsedRule ();
		GameObject ruleUsedObj = createHint (matchingNode, replacer, ruleUsed);
		Worksheet.inst.nextStep (newRoot, ruleUsedObj);

		mouseOver = null;
		hoveredLastFrame = false;
		matchingNode = null;
	}

	public BasicModel cannibalizeAdaptedRuleForUsedRule(){
		//Stjæl hvad der kan bruges fra AdaptingRule til at lave workRule. Destroy derefter AdaptingRule
		AdaptingRule.inst.clearAndApply(true, false);

		BasicModel ruleUsed = AdaptingRule.inst.appliedRoot; //This way keyNode is left, and replaceNode is right (in contrast to just cloning the ruleequation)
		GameObject.Destroy(AdaptingRule.inst.gameObject);
		AdaptingRule.inst = null;

		return ruleUsed;
	}


	//Returns the nearest adapting ancestor and a dictionary explaining how it adapts to adapterNode. Return null if no such ancestor exist
	public CNDictionary setMatchingNodeAndGetDictionary(EquationPart nodeToSearch){
//		Debug.Log("split method up. One takes care of keyNode == Equation, one take care og keyNode == Expression. (use master-method, that calls one of two)");
		EquationPart keyNode = AdaptingRule.inst.getKeyNode ();
		CNDictionary dict = keyNode.createDictionaryTo (nodeToSearch);
		while (dict == null) {
			if (nodeToSearch.parent == null || !(nodeToSearch.parent is EquationPart)) {
				matchingNode = null;
				return null;
			}
			nodeToSearch = (EquationPart) nodeToSearch.parent;
			dict = keyNode.createDictionaryTo (nodeToSearch);
		}
		matchingNode = nodeToSearch;
		return dict;
	}


	private GameObject createHint(EquationPart oldNode, EquationPart newNode, BasicModel ruleUsed){
		GameObject hintObj = ruleUsed.Instantiate ( Builder.worksheetFontsize, true);
		WorkRule.AddAsComponentTo(hintObj);
		hintObj.GetComponent<WorkRule> ().addListener(oldNode, oldNode);
		hintObj.GetComponent<WorkRule> ().addListener(newNode, newNode);
		return hintObj;
	}
		
}
