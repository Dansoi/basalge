using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class EquationPart : BasicModel {

	public BasicModel parent;

	public virtual EquationPart apply (CNDictionary dict) {
		for(int i = 0; i < childList.Count; ++i){
			this [i].apply (dict);
		}
		return this;
	}
	//Clones the node itself and descendants, but not ancestors (so the returned node will have parent == null)
	public abstract EquationPart clone ();

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
					EquationPart newKeyNode = s.keyNode.clone ().apply (newDict); //number of childs remains the same (we're not a Sum or a Variable)
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

	public List<EquationPart> createNodeListLikeIn(List<EquationPart> reversedNodeList, EquationPart nodeForList){

		List<EquationPart> res = new List<EquationPart>();

		List<EquationPart>.Enumerator thisEnum = childList.GetEnumerator ();
		List<EquationPart>.Enumerator otherEnum = nodeForList.childList.GetEnumerator ();

		//Enumerators start out pointing before the first element, så MoveNext must be called once to use it.
		while (thisEnum.MoveNext() && otherEnum.MoveNext()) {
			res.AddRange(thisEnum.Current.createNodeListLikeIn (reversedNodeList, otherEnum.Current));
		}

		int c = reversedNodeList.Count;
		while (c > 0 && reversedNodeList[c-1] == nodeForList) {//while because there could be to of the same in row 
			res.Add (this);
			reversedNodeList.RemoveAt (c - 1);
			--c;
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

