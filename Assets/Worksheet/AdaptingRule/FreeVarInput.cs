using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class FreeVarInput : MonoBehaviour {

	public char varName;
	public Expression node;

	public void setVar(char c){
		varName = c;
		node = null;

		transform.FindChild ("VarEqual").GetComponent<Text> ().text = varName + "=";
		GetComponent<InputField> ().ActivateInputField();
	}

	private void changed () {
		if (varName == '0') Debug.LogError ("May not input text when no variable chosen");

		string str = GetComponent<InputField> ().text;

		if (str == "") {
			node = null;
		} else {
			Expression parsedNode = Expression.parse (str);
			if (parsedNode != null) {
				node = parsedNode;
			}
		}
		Worksheet.inst.interactor.mouseReEnter ();
	}
}
