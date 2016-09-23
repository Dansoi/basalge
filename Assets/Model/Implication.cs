using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Implication : BasicModel {
	public bool bothWays;
	public bool rightWay;

	public override void makeBackwards(){
		childList.Reverse ();
		if (!bothWays) rightWay = !rightWay;
	}



	public new static Implication parse(string str){

		List<EquationPart> list = new List<EquationPart> ();

		int firstEqCharIndex = 0;
		int nextComma = str.IndexOf(',');

		string eqStr;

		while (nextComma != -1) {
			eqStr = str.Substring (firstEqCharIndex, nextComma - firstEqCharIndex);
			list.Add (Equation.parse (eqStr));
			firstEqCharIndex = nextComma + 1;
			nextComma = str.IndexOf (',', firstEqCharIndex);
		}

		nextComma = str.IndexOf ("<=>", firstEqCharIndex);
		bool bothWays = (nextComma != -1);
		bool rightWay = false;
		if(!bothWays) {
			nextComma = str.IndexOf ("==>", firstEqCharIndex);
			rightWay = (nextComma != -1);
			if(!rightWay) nextComma = str.IndexOf ("<==", firstEqCharIndex);
		}


		eqStr = str.Substring (firstEqCharIndex, nextComma - firstEqCharIndex);
		list.Add (Equation.parse (eqStr));


		firstEqCharIndex = nextComma + 3;
		eqStr = str.Substring (firstEqCharIndex);
		list.Add (Equation.parse (eqStr));


		return new Implication (list, bothWays, rightWay);
	}

	public override BasicModel clone(){
		List<EquationPart> cList = new List<EquationPart> ();
		cList.Add ((EquationPart) this [0].clone());
		cList.Add ((EquationPart) this [1].clone());

		return new Implication (cList, bothWays, rightWay);
	}


	public Implication(List<EquationPart> list, bool bothWays, bool rightWay = false){
		Debug.Assert (list.Count == 2); //Currently, we're not dealing with several assumptions...
		this.bothWays = bothWays;
		this.rightWay = rightWay;
		childList = list;
		foreach (EquationPart c in list) {
			c.parent = this;
		}
	}

	public override GameObject Instantiate (int size, bool setNodeTrn = false) {
		nodeRectTrn = Builder.InstantiateDummy("Implication").GetComponent<RectTransform>();

		Transform trn;
		trn = this [0].Instantiate (size, setNodeTrn).transform;
		trn.SetParent (nodeRectTrn);
		trn.name = "Left";

		for (int i = 1; i < childList.Count-1; i++) {
			Builder.InstantiateTextObj (size, ",").transform.SetParent(nodeRectTrn);
			this [i].Instantiate(size, setNodeTrn).transform.SetParent (nodeRectTrn);
		}
		if (bothWays) {
			Builder.InstantiateTextObj (size, "\u21d4", "LRImp").transform.SetParent (nodeRectTrn);
		} else if (rightWay) {
			Builder.InstantiateTextObj (size, "\u21d2", "RImp").transform.SetParent (nodeRectTrn);
		} else {
			Builder.InstantiateTextObj (size, "\u21d0", "LImp").transform.SetParent (nodeRectTrn);
		}

		trn = this [childList.Count-1].Instantiate (size, setNodeTrn).transform;
		trn.SetParent (nodeRectTrn);
		trn.name = "Right";


		putTransformsSideBySide (size);
		return nodeRectTrn.gameObject;
	}

	public override string ToString () {
		string res = childList[0].ToString();
		for (int i = 1; i < childList.Count - 1; i++) res += "," + childList [i].ToString();
		if (bothWays) {
			res += "<=>";
		} else if (rightWay) {
			res += "==>";
		} else {
			res += "<==";
		}
		res += childList [childList.Count - 1].ToString();
		return res;
	}
}
