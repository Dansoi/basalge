using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class BasicInteractor : AbstractInteractor {

	public EventHandler mouseOver;
	public bool hoveredLastFrame;
	public EquationPart matchingNode;
	public CRList matchingRanges;

	public override void Update(){
		if (mouseOver == null && hoveredLastFrame) {
			hoveredLastFrame = false;
			AdaptingRule a = AdaptingRule.inst;
			if (a != null) {
				a.dict = new CNDictionary ();
				a.applyDictionary(false, true);
				a.repaint();
			}
			emphasizeMatch (Builder.inst.fadedClr);
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
		if(matchingNode != null) emphasizeMatch (Builder.inst.fadedClr);

		CNDictionary dict = setMatchingNodeAndGetDictionary (eh); //sets matchingNode field
		applyAndPaintAdaptingRule (dict);

		if (matchingNode == null) return;

		AdaptingRule.inst.applyDictionary (true, true);
		matchingRanges = matchingNode.createColoredRangeListLikeIn(AdaptingRule.inst.keyRanges, AdaptingRule.inst.getAppliedKey());
		Builder.emphasizeRange (matchingRanges.init.rangeStart, matchingRanges.init.rangeEnd, matchingRanges.init.clr);
		foreach (ColoredRange c in matchingRanges){
			Builder.emphasizeRange (c.rangeStart, c.rangeEnd, c.clr, true);
		}

	}

	public string listString(List<EquationPart> list){
		string str = "";
		foreach (EquationPart c in list) {
			str += c.ToString () + " | ";
		}
		return str;
	}

	public void applyAndPaintAdaptingRule(CNDictionary dict){
		if (dict == null) {
			AdaptingRule.inst.dict = new CNDictionary ();
			AdaptingRule.inst.applyDictionary (false, true);
			AdaptingRule.inst.repaint ();

			return;
		}

		AdaptingRule.inst.dict = dict;
		AdaptingRule.inst.applyDictionary(false, true);
		AdaptingRule.inst.repaint();
	}

	public override void mouseReEnter () {
		if (mouseOver != null) {
			mouseEnter (mouseOver);
		} else {
			AdaptingRule a = AdaptingRule.inst;
			if (a) {
				a.dict = new CNDictionary ();
				a.applyDictionary (false, true);
				a.repaint ();

			}
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

		emphasizeMatch (Builder.inst.fadedClr);

		//Klippe-klistre nodes
		List<BasicModel> matchList = new List<BasicModel> ();
		matchList.Add (matchingNode);
		EquationPart oldRoot = matchingNode.getRoot ();
		EquationPart clonedRoot = (EquationPart) oldRoot.clone ();
		EquationPart clonedMatch = (EquationPart) clonedRoot.createNodeListLikeIn (matchList, oldRoot)[0];
		EquationPart replacer = (EquationPart) AdaptingRule.inst.getAppliedReplacer().clone();
		EquationPart newRoot = clonedMatch.replaceWith (replacer).getRoot ();
		CRList newMatchingRanges = replacer.createColoredRangeListLikeIn (AdaptingRule.inst.replacerRanges, AdaptingRule.inst.getAppliedReplacer ());

		CRList usedRuleRanges;
		BasicModel ruleUsed = cannibalizeAdaptedRuleForUsedRule (out usedRuleRanges);
		GameObject ruleUsedObj = createHint (usedRuleRanges, matchingRanges, newMatchingRanges, ruleUsed);

		bool isTop = mouseOver.GetComponentInParent<EndChain> ().isTop;
		Worksheet.inst.nextStep (newRoot, ruleUsedObj, isTop);

		mouseOver = null;
		hoveredLastFrame = false;
		matchingNode = null;
	}

	public BasicModel cannibalizeAdaptedRuleForUsedRule(out CRList usedRuleRanges){
		//Stjæl hvad der kan bruges fra AdaptingRule til at lave workRule. Destroy derefter AdaptingRule
		AdaptingRule a = AdaptingRule.inst;
		a.dict = new CNDictionary ();
		a.applyDictionary (true, false);
		BasicModel ruleUsed = a.appliedRoot;

		bool isWorkingOnTop = mouseOver.GetComponentInParent<EndChain> ().isTop;
		if (isWorkingOnTop && a.isFlipped () || !isWorkingOnTop && !a.isFlipped ()) {
			ruleUsed.makeBackwards ();
		}

		usedRuleRanges = a.keyRanges;
		usedRuleRanges.AddRange (a.replacerRanges);
		usedRuleRanges.init = new ColoredRange(a.appliedRoot, null, Color.white);

		GameObject.Destroy(a.gameObject);
		AdaptingRule.inst = null;

		return ruleUsed;
	}


	//Returns the nearest adapting ancestor and a dictionary explaining how it adapts to adapterNode. Return null if no such ancestor exist
	public CNDictionary setMatchingNodeAndGetDictionary(EventHandler eh){
//		Debug.Log("split method up. One takes care of keyNode == Equation, one take care og keyNode == Expression. (use master-method, that calls one of two)");
		AdaptingRule a = AdaptingRule.inst;

		bool isWorkingOnTopChain = eh.GetComponentInParent<EndChain> ().isTop;

		if (isWorkingOnTopChain && !a.canKeyBePremise ()) a.flipApplyRepaint ();
		if (!isWorkingOnTopChain && !a.canKeyBeConclusion ()) a.flipApplyRepaint ();
			
		EquationPart keyNode = a.getKey ();
		EquationPart nodeToSearch = eh.GetComponentInParent<wNode> ().node;

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


	public GameObject createHint(CRList usedRuleRanges, CRList oldRangeList, CRList newRangeList, BasicModel ruleUsed){
		GameObject hintObj = ruleUsed.Instantiate ( Builder.worksheetFontsize, true);
		WorkRule.AddAsComponentTo(hintObj);
		hintObj.GetComponent<WorkRule> ().thisRangeList = usedRuleRanges;
		hintObj.GetComponent<WorkRule> ().towardsEdgeRangeList = oldRangeList;
		hintObj.GetComponent<WorkRule> ().towardsDotsRangeList = newRangeList;
		return hintObj;
	}
		
}
