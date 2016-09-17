using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public abstract class Expression : EquationPart {

	public int indexNo(){
		Debug.Assert (parent != null);
		List<EquationPart> sibling = parent.childList;
		for (int i = 0; i < sibling.Count; i++) {
			if (sibling [i] == this) return i;
		}
		Debug.Assert (false);
		return 0;
	}

	public new static Expression parse(string str){
		Expression res;

		int innerEnd = str.Length - 1;
		while (innerEnd >= 0 && str [innerEnd] == ')') { --innerEnd; }
		//Now innerEnd is the index of the last non-')' character

		int currentPos = 0;
		while (currentPos < str.Length && str [currentPos] == '(') { ++currentPos; }
		//Now currentPos is the index of the first non-'(' character

		int nestedParentheses = currentPos;
		int lowestNestedParenthesesSoFar = currentPos;
		List<int> operPosList = new List<int>(); //Position of the operators in fewest possible nested parentheses
		while (currentPos <= innerEnd) {
			
			if (str [currentPos] == '(') {
				++nestedParentheses;

			} else if (str [currentPos] == ')') {
				--nestedParentheses;
				if (nestedParentheses < lowestNestedParenthesesSoFar) {
					lowestNestedParenthesesSoFar = nestedParentheses;
					operPosList.Clear ();
				}

			} else if (nestedParentheses == lowestNestedParenthesesSoFar
				&& str [currentPos] == '+') { //  || str [currentPos] == '*')
				operPosList.Add(currentPos);

			}
			++currentPos;
		}

		//Now all characters from (including) position currentPos are ')'. If there's not nestedParentheses of them, str is not valid
		if (str.Length - currentPos != nestedParentheses) return null;

		//Trim leading '('s and trailing ')'s from str. Trim lowestNestedParenthesesSoFar from each side.
		str = str.Substring (lowestNestedParenthesesSoFar, str.Length - 2 * lowestNestedParenthesesSoFar);
		for(int i = 0; i < operPosList.Count; i++) operPosList[i] -= lowestNestedParenthesesSoFar;


		//Found sum?
		if (operPosList.Count > 0) {
			return Sum.parse (str, operPosList);
		}

		res = Variable.parse (str);
		if (res == null) res = Element.parse (str);
		if (res == null) res = Negative.parse (str);
		return res;
	}

	/*
	public List<AnyNode> createNodeListLike(List<AnyNode> nodeList){
		if (parent == null || !(parent is Node)) {
			Debug.LogError ("Only call on a rootNode");
		}
		Node rootNode = nodeList[0].getRoot ();
		nodeList.Reverse ();
		return createNodeListLikeIn(nodeList, rootNode);
	}
*/



	/*
	public void kill(){
		if (parent == null) {
			Debug.LogError ("Cannot kill the last node");
		}
		if (parent is Sum && parent.childList.Count == 2){
			Expression sibling;
			if (parent.childList [1] == this) {
				sibling = parent [0];
			} else {
				sibling = parent [1];
			}

			parent.replaceWith (sibling);
		} else if (parent is Negative) {
			parent.kill ();
		} else if (parent is Sum) {
			List<Expression> sibling = parent.childList;
			for( int i = 0; i < sibling.Count; i++){
				if (sibling [i] == this) {
					sibling.RemoveAt (i);
					return;
				}
			}
		}

	}
*/


}