using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Implication : BasicModel {
	public bool bothWays;

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
		if(!bothWays) nextComma = str.IndexOf ("==>", firstEqCharIndex);

		eqStr = str.Substring (firstEqCharIndex, nextComma - firstEqCharIndex);
		list.Add (Equation.parse (eqStr));


		firstEqCharIndex = nextComma + 3;
		eqStr = str.Substring (firstEqCharIndex);
		list.Add (Equation.parse (eqStr));


		return new Implication (list, bothWays);
	}

	public Implication(List<EquationPart> list, bool bothWays){
		Debug.Assert (list.Count == 2); //Currently, we're not dealing with several assumptions...
		this.bothWays = bothWays;
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
			Builder.InstantiateAtomic (size, ",").transform.SetParent(nodeRectTrn);
			this [i].Instantiate(size, setNodeTrn).transform.SetParent (nodeRectTrn);
		}
		if (bothWays) {
			Builder.InstantiateAtomic (size, "\u21d4", "BiImp").transform.SetParent (nodeRectTrn);
		} else {
			Builder.InstantiateAtomic (size, "\u21d2", "Imp").transform.SetParent (nodeRectTrn);
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
		res += "=>" + childList [childList.Count - 1].ToString();
		return res;
	}
}
