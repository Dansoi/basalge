using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class EquationPart : BasicModel {

	public BasicModel parent;

	public override void makeBackwards(){
		childList.Reverse ();
	}

	public abstract bool Equals (EquationPart other); // // // // // // // // // // // // // // // //

	public abstract void workify ();

	//Returns a dictionary from the variables in this to the nodes in node. returns null if not possible
	public virtual CNDictionary createDictionaryTo (EquationPart other) {
		if (GetType () != other.GetType ()) return null;
		if (childList.Count != other.childList.Count) return null;
		CNDictionary dict = new CNDictionary ();

		for(int i = 0; i < childList.Count; i++){
			CNDictionary temp = childList [i].createDictionaryTo (other.childList [i]);
			if (temp == null) return null;
			if (!dict.AddDictionary (temp)) return null;
		}
		return dict;
	}

	public virtual List<CNDictionary> createDictionariesTo (EquationPart other, bool returnFirstFound = false){
		List<CNDictionary> res = new List<CNDictionary>();

		if (GetType () != other.GetType ()) return res;
		if (childList.Count != other.childList.Count) return res;


		Stack<State> stateStack = new Stack<State>();
		stateStack.Push (new State (new CNDictionary (), this, 0, 0));

		/*
		 * Central idea in the while-loop:
		 * 
		 * Before any (new) state is pushed to the stack, the corresponding (addition to its previous) dictionary is applied to its keyNode
		 * As a consequence, we can take the first dictionary created from keyNode[lastNode] to other[lastNode] and it is
		 * guaranteed to merge with the previous dictionary and yield a master-dictionary from this to other
		 * 
		 * In this method, keyIndex (should be) equal mapIndex all the time. In overridden methods (in Sum), that's not the case
		 * */

		while (stateStack.Count > 0) {
			State s = stateStack.Pop();

			Debug.Assert (s.keyIndex == s.maptoIndex); //We're not a Sum nor a Variable.

			if (s.keyIndex < s.keyNode.Count()) {
				//Ways for dictionaries to map from s.keyNode[s.keyIndex] to other[s.keyIndex]
				bool isLastChild = (s.keyIndex == s.keyNode.Count() - 1);
				List<CNDictionary> nodeDictionaries = s.keyNode [s.keyIndex].createDictionariesTo (other [s.keyIndex], returnFirstFound && isLastChild);

				foreach (CNDictionary newDict in nodeDictionaries) {
					EquationPart newKeyNode = (EquationPart) s.keyNode.clone ().apply (newDict); //number of childs remains the same (we're not a Sum or a Variable)
					bool success = newDict.AddDictionary (s.dict); 
					Debug.Assert (success); //this shouldn't fail, because their keys should be disjoint
					stateStack.Push (new State (newDict, newKeyNode, s.keyIndex + 1, s.keyIndex + 1));

				}
			} else if (s.keyIndex == s.keyNode.Count()) {
				res.Add (s.dict);
				if (returnFirstFound) return res;
			}
		}

		return res;
	}


	public virtual EquationPart replaceWith(EquationPart newNode) {
		newNode.parent = parent;
		if (parent == null) return newNode;
		List<EquationPart> sibling = parent.childList; //sibling includes one self
		for( int i = 0; i < sibling.Count; i++){
			if (sibling [i] == this) {
				sibling [i] = newNode;
				return newNode;
			}
		}
		Debug.LogError ("Execution shouldn't reach this line");
		return null;
	}

	public virtual HashSet<char> getVarSet() {
		HashSet<char> list = new HashSet<char> ();
		foreach (Expression c in childList) {
			list.UnionWith (c.getVarSet ());
		}
		return list;
	}
		

	public EquationPart getRoot(){
		if (parent == null) return this;
//		if (parent is Implication) return this;
		return (parent as EquationPart).getRoot ();
	}

	public CRList createColoredRangeListLikeIn(CRList crl, EquationPart nodeForList){
//		Debug.Log ("called cCRLLI with this: " + this.ToString() + ", nodeForList: " + nodeForList.ToString ());
//		Debug.Log ("CRList.init: " + crl.init.ToString() + ", CRList: " + toString (crl));
		List<BasicModel> othersExpressionListReversed = new List<BasicModel> ();

		othersExpressionListReversed.Add (crl.init.rangeStart);

		foreach(ColoredRange c in crl){
			othersExpressionListReversed.Add (c.rangeStart);
			othersExpressionListReversed.Add (c.rangeEnd);
		}
		if( crl.init.rangeEnd != null) othersExpressionListReversed.Add (crl.init.rangeEnd);

		othersExpressionListReversed.Reverse ();
		List<BasicModel> thissExpressionList = this.createNodeListLikeIn (othersExpressionListReversed, nodeForList);

		CRList res = new CRList ();
		res.init = new ColoredRange(thissExpressionList[0],
			(crl.init.rangeEnd == null) ? null : thissExpressionList[thissExpressionList.Count-1],
			crl.init.clr);

		int count = 1;
		foreach (ColoredRange c in crl) {
			res.Add (new ColoredRange(thissExpressionList [count], thissExpressionList [count+1], c.clr));
			count += 2;
		}

		return res;
	}

	public string toString(List<ColoredRange> CRList){
		string res = "";
		foreach(ColoredRange c in CRList){
			res += c.ToString () + "|";
		}
		return res;
	}
	public string toString(List<BasicModel> list){
		string res = "";
		foreach(BasicModel c in list){
			res += c.ToString () + "|";
		}
		return res;
	}

	public List<BasicModel> createNodeListLikeIn(List<BasicModel> reversedNodeList, EquationPart nodeForList){
//		Debug.Log ("called CNLLI with this: " + this.ToString() + ", reversedNodeList: " + toString(reversedNodeList) + ", nodeForList: " + nodeForList.ToString ());

		List<BasicModel> res = new List<BasicModel>();

		int c = reversedNodeList.Count;
		while (c > 0 && reversedNodeList[c-1] == nodeForList) {//while because there could be several of the same after each other
			res.Add (this);
			reversedNodeList.RemoveAt (c - 1);
			--c;
		}

		List<EquationPart>.Enumerator thisEnum = childList.GetEnumerator ();
		List<EquationPart>.Enumerator otherEnum = nodeForList.childList.GetEnumerator ();


		//Enumerators start out pointing before the first element, so MoveNext must be called once to use it.
		while (thisEnum.MoveNext() && otherEnum.MoveNext()) {
			res.AddRange(thisEnum.Current.createNodeListLikeIn (reversedNodeList, otherEnum.Current));
		}


		return res;
	}
}



public class State
{
	public CNDictionary dict;
	public EquationPart keyNode;
	public int keyIndex;
	public int maptoIndex;

	public State(CNDictionary dict, EquationPart keyNode, int keyIndex, int maptoIndex){
		this.dict = dict;
		this.keyNode = keyNode;
		this.keyIndex = keyIndex;
		this.maptoIndex = maptoIndex;
	}
	public override string ToString(){
		return "dict: " + dict.ToString () + ", keyNode: " + keyNode.ToString () + ", keyIndex: " + keyIndex + ", maptoIndex: " + maptoIndex;
	}

}

