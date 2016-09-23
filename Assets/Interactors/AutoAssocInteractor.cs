using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AutoAssocInteractor : BasicInteractor {

	EquationPart lastMatchingNode;


	public override void emphasizeMatch(Color clr){
		if (lastMatchingNode == null) {
			base.emphasizeMatch (clr);
		} else {
			Debug.Assert (matchingNode != null);
			Debug.Assert (matchingNode.parent == lastMatchingNode.parent);
			Builder.emphasizeRange (matchingNode, lastMatchingNode, clr);
		}
	}

	public override void mouseEnter(EventHandler eh){
		if (AdaptingRule.inst == null) return;

		mouseOver = eh;
		hoveredLastFrame = true;
		if(matchingNode != null) emphasizeMatch (Builder.inst.fadedClr);

		CNDictionary dict = setMatchingNodeAndGetDictionary (eh); //sets matchingNode fields

		applyAndPaintAdaptingRule(dict);

		if (matchingNode == null) return;

		if (lastMatchingNode == null) {
			AdaptingRule.inst.applyDictionary (true, true);
			matchingRanges = matchingNode.createColoredRangeListLikeIn (AdaptingRule.inst.keyRanges, AdaptingRule.inst.getAppliedKey ());
			Builder.emphasizeRange (matchingRanges.init.rangeStart, matchingRanges.init.rangeEnd, matchingRanges.init.clr);
			foreach (ColoredRange c in matchingRanges) {
				Builder.emphasizeRange (c.rangeStart, c.rangeEnd, c.clr, true);
			}
		} else {
			Debug.Assert (matchingNode != lastMatchingNode);
			Sum fosterSum = ((Sum) matchingNode.parent).getFosterSum(
				((Expression) matchingNode).indexNo(),
				((Expression) lastMatchingNode).indexNo() - ((Expression) matchingNode).indexNo()+1);
			AdaptingRule.inst.applyDictionary (true, true);
//			Debug.Log (fosterSum.ToString ());
//			Debug.Log (AdaptingRule.inst.getAppliedKey ().ToString ());
//			Debug.Log (AdaptingRule.inst.keyRanges.ToString());
			matchingRanges = fosterSum.createColoredRangeListLikeIn (AdaptingRule.inst.keyRanges, AdaptingRule.inst.getAppliedKey ());
			matchingRanges.init.rangeStart = matchingNode;
			matchingRanges.init.rangeEnd = lastMatchingNode;

			Builder.emphasizeRange (matchingRanges.init.rangeStart, matchingRanges.init.rangeEnd, matchingRanges.init.clr);
			foreach (ColoredRange c in matchingRanges) {
				Builder.emphasizeRange (c.rangeStart, c.rangeEnd, c.clr, true);
			}

		}
	}


	public override void mouseClick () {
		if (matchingNode == null) return;

		if (lastMatchingNode == null) {

			//if keyNode is sum, it cannot replace matchingNode if that is a term in a sum (in that case it must be embedded). But otherwise BasicInteractor will do just fine
			if (!(matchingNode.parent is Sum) || !(AdaptingRule.inst.getAppliedReplacer() is Sum)) {
				base.mouseClick ();
				return;
			}
		}

		emphasizeMatch (Builder.inst.fadedClr);
		if (lastMatchingNode == null) lastMatchingNode = matchingNode; //We make an exception to the rule, that if were not dealing with a range, then lastMatchingNode is null. We're not using method that depends on this convention. We set lastMatchingNode to null again in the end of this method

		EquationPart newRoot;
		CRList newMatchingRanges;


		{
			//Klippe-klistre nodes
			Expression replacer = (Expression) AdaptingRule.inst.getAppliedReplacer().clone();
			newMatchingRanges = replacer.createColoredRangeListLikeIn (AdaptingRule.inst.replacerRanges, AdaptingRule.inst.getAppliedReplacer ());
			if (replacer is Sum) {
				//Replacer skal merges ind længere nede. Derfor må vi ikke refererer til replacer direkte, kun dens childs
				newMatchingRanges.init.rangeStart = replacer [0];
				newMatchingRanges.init.rangeEnd = replacer [replacer.Count () - 1];

				if (newMatchingRanges.Count == 1 && newMatchingRanges [0].rangeStart == replacer) {
					newMatchingRanges[0].rangeStart = replacer [0];
					newMatchingRanges[0].rangeEnd = replacer [replacer.Count () - 1];
				}
			}

			int firstMatchIndex = ((Expression)matchingNode).indexNo ();
			int matchingTerms = ((Expression)lastMatchingNode).indexNo () - firstMatchIndex + 1;

			List<BasicModel> firstMatchList = new List<BasicModel> (); firstMatchList.Add (matchingNode);
			EquationPart oldRoot = matchingNode.getRoot ();
			EquationPart clonedRoot = (EquationPart) oldRoot.clone ();
			Expression clonedMatchNode = (Expression) clonedRoot.createNodeListLikeIn (firstMatchList, oldRoot)[0];

			newRoot = (clonedMatchNode.parent as Sum).replaceTerms (firstMatchIndex, matchingTerms, replacer).getRoot();
		}

		CRList usedRuleRanges;
		BasicModel ruleUsed = cannibalizeAdaptedRuleForUsedRule (out usedRuleRanges);
		GameObject ruleUsedObj = createHint (usedRuleRanges, matchingRanges, newMatchingRanges, ruleUsed);
		bool isTop = mouseOver.GetComponentInParent<EndChain> ().isTop;
		Worksheet.inst.nextStep (newRoot, ruleUsedObj, isTop);

		mouseOver = null;
		hoveredLastFrame = false;
		matchingNode = lastMatchingNode = null;
	}
	/*
	private GameObject createHint(Expression oldFirstNode, Expression oldLastNode, Expression newFirstNode, Expression newLastNode, BasicModel ruleUsed){
		
		GameObject hintObj = ruleUsed.Instantiate ( Builder.worksheetFontsize, true);
		WorkRule.AddAsComponentTo(hintObj);
		hintObj.GetComponent<WorkRule> ().addLeftRange (oldFirstNode, oldLastNode, Color.red);
		hintObj.GetComponent<WorkRule> ().addRightRange (newFirstNode, newLastNode, Color.red);
		return hintObj;
		return null;
	}
	*/




	public new CNDictionary setMatchingNodeAndGetDictionary(EventHandler eh){
		bool isWorkingOnTopChain = eh.GetComponentInParent<EndChain> ().isTop;

		AdaptingRule a = AdaptingRule.inst;
		if (isWorkingOnTopChain && !a.canKeyBePremise ()) a.flipApplyRepaint ();
		if (!isWorkingOnTopChain && !a.canKeyBeConclusion ()) a.flipApplyRepaint ();
		EquationPart keyNode = a.getKey ();

		if (!(keyNode is Sum)) { //Hvis der pilles ved denne if-sætning, så bekræft at keyNode (x+y)' kan holdes over (a+b+c)' og give forventet resultat (hvor x -> a og y -> b+c)
			lastMatchingNode = null; //lastMatchingNode == null means that match is not a range
			EquationPart currentNode = eh.GetComponentInParent<wNode> ().node;
			List<CNDictionary> dictList = keyNode.createDictionariesTo (currentNode, true);
			while (dictList.Count == 0) {
				if (currentNode.parent == null || currentNode.parent is Implication) {
					matchingNode = null;
					return null;
				}
				currentNode = (EquationPart)currentNode.parent;
				dictList = keyNode.createDictionariesTo (currentNode, true);
			}
			matchingNode = currentNode;
			return dictList[0];
		}


		Sum keySum = (Sum)keyNode;

		for(Expression currentTerm = getImmediateLeftTerm(eh); currentTerm != null; currentTerm = getOverlordTerm(currentTerm)){ //One iteration for each sum to examine
			Sum currentSum = currentTerm.parent as Sum;
			int mandatoryIndex = currentTerm.indexNo (); //mandatoryIndex : Index that MUST be included

			for (int currentIndex =
					((mandatoryIndex + keySum.Count() <= currentSum.Count())
						? mandatoryIndex
						: currentSum.Count() - keySum.Count());
				currentIndex >= 0;
				currentIndex--) {

				Sum mapIntoSum = currentSum.getFosterSum (currentIndex, currentSum.Count() - currentIndex);
				int mandatoryMapIntoTerms = mandatoryIndex - currentIndex + 1;

				List<CNDictionary> dicts = keySum.createDictionariesTo(mapIntoSum, true, mandatoryMapIntoTerms);
				if(dicts.Count > 0) { //hoorayy! Found a match!!
					CNDictionary dict = dicts[0];

					matchingNode = mapIntoSum [0]; //currentSum.childList[currentIndex];
					Sum keySumClone = (Sum) keySum.clone ().apply (dict);
					lastMatchingNode = (Expression)mapIntoSum [keySumClone.Count () - 1];//currentSum [currentIndex + keySumClone.Count () - 1];
					return dict;
				}
			}
		}

		matchingNode = lastMatchingNode = null;
		return null;
			
	}


	//This method goes up at least once and returns the first ancestor that is a term (has Sum as a parent)
	private Expression getOverlordTerm(Expression node){

		if(node.parent == null || node.parent is Equation) return null;
		do {
			node = (Expression) node.parent;
			if (node.parent == null || node.parent is Equation) return null;
		} while (!(node.parent is Sum));

		return node;
	}


	//if node is a Sum, the immediate left term (of what is hovered over) is returned. Otherwise the term, that the node is part of, is returned
	private Expression getImmediateLeftTerm(EventHandler eh){
		EquationPart node = eh.GetComponentInParent<wNode> ().node;

		if (node is Sum) {
			return (Expression)node.childList [eh.nodePart];
		} else if (node is Equation) {
			return null;
		} else {
			return getTermRoot ((Expression)node);
		}


	}

	private Expression getTermRoot(Expression node){
		//We can now deal with equations. Not just expressions. Take into account!S
		Debug.Assert (!(node is Sum));

		while (node.parent != null && node.parent is Expression) {
			if (node.parent is Sum) return node;
			node = (Expression) node.parent;
		}

		return null;
	}
}
