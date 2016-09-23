using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// SUMS *MUST* CONTAIN AT LEAST TWO TERMS
/// </summary>
/// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/// 
/// 
/// 
/// 
///                                                                              SUMS *MUST* CONTAIN AT LEAST TWO TERMS
/// 
/// 
/// 
/// 
/// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
public class Sum : Expression {
	public static Sum parse(string str, List<int> plusPosList){
		List<EquationPart> list = new List<EquationPart> ();

		Expression node;
		node = Expression.parse (str.Substring (0, plusPosList [0]));
		if (node == null) return null;
		list.Add (node);


		for (int i = 1; i < plusPosList.Count; i++) {
			node = Expression.parse (str.Substring (plusPosList[i-1]+1, plusPosList[i] - plusPosList[i-1] - 1));
			if (node == null) return null;
			list.Add (node);
		}

		node = Expression.parse (str.Substring (plusPosList[plusPosList.Count-1] + 1));
		if (node == null) return null;
		list.Add (node);

		return new Sum (list);

	}

	public override GameObject Instantiate(int size, bool setNodeTrn){
		nodeRectTrn = Builder.InstantiateDummy("Sum").GetComponent<RectTransform>();

		Transform trn;

		for (int i = 0; i < childList.Count; i++) {

			// (
			if (this [i] is Sum) Builder.InstantiateTextObj (size, "(", "Term " + i + " (").transform.SetParent(nodeRectTrn);

			//Add term i
			trn = this [i].Instantiate( size, setNodeTrn).transform;
			trn.name = "Term " + i;
			trn.SetParent (nodeRectTrn);

			//)
			if (this [i] is Sum) Builder.InstantiateTextObj (size, ")", "Term " + i + " )").transform.SetParent(nodeRectTrn);

			//+
			if (i < childList.Count - 1) {
				Builder.InstantiateTextObj (size, "+", "Plus " + i).transform.SetParent (nodeRectTrn);
			}

		}

		putTransformsSideBySide (size);
		return nodeRectTrn.gameObject;
	}

	public override void workify (){
		Transform trn;
		for (int i = 0; i < childList.Count; i++) {

			trn = nodeRectTrn.Find ("Term " + i + " (");
			if (trn != null) EventHandler.AddAsComponentTo(trn.gameObject, i);

			wNode.AddAsComponentTo( nodeRectTrn.FindChild ("Term " + i).gameObject, this [i]);

			trn = nodeRectTrn.Find ("Term " + i + " )");
			if (trn != null) EventHandler.AddAsComponentTo(trn.gameObject, i);

			trn = nodeRectTrn.Find ("Plus " + i);
			if (trn != null) EventHandler.AddAsComponentTo(trn.gameObject, i);
		}
	}


	public Sum(List<EquationPart> list, bool setChildParent = true){
		Debug.Assert (list.Count >= 2); // Sum must be given at least two terms
		childList = list;
		if (!setChildParent) return;
		foreach (Expression c in list) {
			c.parent = this;
		}
	}

	//VERY CAREFUL USE OF THIS METHOD. For the returned Sum, the statement this.child[i].parent == this will not hold.
	//Use it for dictionary-entries, that only maps to certain terms in a sum.
	public Sum getFosterSum(int firstIndex, int count){
		Debug.Assert (count >= 2);
		List<EquationPart> newChild = new List<EquationPart> ();
		for (int i = firstIndex; i < firstIndex + count; i++) {
			newChild.Add (this [i]);
		}
		return new Sum (newChild, false);
	}


	public override List<CNDictionary> createDictionariesTo (EquationPart other, bool returnFirstFound = false){
		if (!(other is Sum)) return new List<CNDictionary> (); //0 dictionaries exists because its impossible to create dictionary from at least two terms to one term.
		return createDictionariesTo ((Sum) other, returnFirstFound, other.childList.Count);
	}


	public List<CNDictionary> createDictionariesTo (Sum other, bool returnFirstFound, int mandatoryMapIntoTerms) {
//		Debug.Log ("Creating dictionaries from " + this.ToString() + " to at least " + mandatoryMapIntoTerms +" terms from " + other.ToString());

		List<CNDictionary> res = new List<CNDictionary>();

		Stack<State> stateStack = new Stack<State>();
		stateStack.Push (new State (new CNDictionary (), this, 0, 0));

		while (stateStack.Count > 0) {
			State s = stateStack.Pop();

//			Debug.Log ("Popped state("+stateStack.Count+"): " + s.dict.ToString () + "|" + s.keyNode.ToString () + "|" + s.keyIndex + "|" + s.maptoIndex);
				
			Debug.Assert (s.keyIndex <= s.keyNode.Count()); //If this is allowed to fail, then fix this first if-condition:
			if (s.keyIndex == s.keyNode.Count()) {
//				Debug.Log ("KeyIndex has advanced past end. Returning dict if mandatoryTerms has been mapped into");

				//If refactoring this if-statement, make sure that hovering the rule "x'+x=0" over various terms in "(a+b)'+a+b+c" behaves as expected
				if (s.maptoIndex >= mandatoryMapIntoTerms) {
//					Debug.Log ("mandatoryTerms has been mapped into. Returning " + s.dict.ToString());
					res.Add (s.dict);
					if (returnFirstFound) return res;
				}

			} else if (s.keyNode [s.keyIndex] is Variable) {
//				Debug.Log ("keyNode[keyIndex] is a variable");
				char varName = (s.keyNode [s.keyIndex] as Variable).varName;

				for (int j = other.Count() - s.maptoIndex; j >= 1; j--) { //j: number of terms to map into
					CNDictionary newDict = new CNDictionary ();
					newDict.Add (varName, (j == 1) ? (Expression)other [s.maptoIndex] : other.getFosterSum (s.maptoIndex, j));
					Sum newKeyNode = (Sum) s.keyNode.clone ().apply (newDict); //the variable in s.keyNode turns into j terms in newKeyNode
					bool success = newDict.AddDictionary (s.dict);
					Debug.Assert (success); //this shouldn't fail, because their keys should be disjoint
//					Debug.Log ("Pushing state that maps to " + j + " terms");
					stateStack.Push (new State (newDict, newKeyNode, s.keyIndex + j, s.maptoIndex + j));
				}


			} else if (s.maptoIndex < other.Count()) {
//				Debug.Log ("keyNode[keyIndex] is not a variable variable");
				//Ways for dictionaries to map from s.keyNode[s.keyIndex] to other[s.mapToIndex]
				List<CNDictionary> nodeDictionaries = s.keyNode [s.keyIndex].createDictionariesTo (other [s.maptoIndex]);

				foreach (CNDictionary newDict in nodeDictionaries) {
					Sum newKeyNode = (Sum) s.keyNode.clone ().apply (newDict);
					bool success = newDict.AddDictionary (s.dict);
					Debug.Assert (success); //this shouldn't fail, because their keys should be disjoint
					stateStack.Push (new State (newDict, newKeyNode, s.keyIndex + 1, s.maptoIndex + 1));

				}
			}
		}

//		Debug.Log ("returning dictlist(" + res.Count + ")");
		return res;
	}

	public override BasicModel apply(CNDictionary dict, List<ColoredRange> crList, bool useLeafsForRangeIfSameType){
		bool relaxSums = Overview.inst.autoAssoc.isChecked ();
		for(int i = 0; i < Count(); ++i){
			Expression replacer = this [i].apply (dict, crList, relaxSums) as Expression;
			if (replacer is Sum && relaxSums) {
				replaceTerms (i, 1, replacer);
				i = i - 1 + replacer.childList.Count;
			}
		}
		return this;
	}

	public Expression replaceTerms (int firstIndexToRemove, int termsToRemove, Expression replacer){
		childList.RemoveRange (firstIndexToRemove, termsToRemove);
		if (childList.Count == 0) {
			this.replaceWith (replacer);
			return replacer;
		}
		if (replacer is Sum) {
			foreach (Expression c in replacer.childList) c.parent = this;
			childList.InsertRange (firstIndexToRemove, replacer.childList);
		} else {
			replacer.parent = this;
			childList.Insert (firstIndexToRemove, replacer);
		}
		return this;
	}

	public override string ToString(){
		string str = this [0].ToString ();
		for (int i = 1; i < childList.Count; i++) {
			str += "+" + this [i];
		}
		if (parent != null) str = "(" + str + ")";
		return str;
	}



	public override bool Equals(EquationPart other){
		if (!(other is Sum)) return false;
		if (childList.Count != other.childList.Count) return false;
		for (int i = 0; i < childList.Count; i++) {
			if ( !(this [i].Equals (other [i])) ) return false;
		}
		return true;
	}

	public override BasicModel clone(){
		Debug.Assert (childList.Count >= 2);
		List<EquationPart> newList = new List<EquationPart> ();
		foreach (EquationPart c in childList) {
			newList.Add ((EquationPart) c.clone ());
		}
		return new Sum (newList);
	}
}
