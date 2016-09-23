using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Equation : EquationPart {

	public new static Equation parse(string str){
		Expression left, right;
		int eqPos = str.IndexOf ('=');

		if (eqPos == -1) {
			left = null;
			right = Expression.parse(str.Substring(0, str.Length - 1));
		} else {
			left = Expression.parse (str.Substring (0, eqPos));
			right = Expression.parse(str.Substring(eqPos + 1, str.Length - eqPos - 1));

		}
		Equation res = new Equation (left, right);
		return res;
	}

	public override GameObject Instantiate(int size, bool setNodeTrn){
		nodeRectTrn = Builder.InstantiateDummy("Equation").GetComponent<RectTransform>();
		Transform trn;

		trn = this [0].Instantiate (size, setNodeTrn).transform;
		trn.name = "Left";
		trn.SetParent (nodeRectTrn);

		Builder.InstantiateTextObj (size, "=").transform.SetParent (nodeRectTrn);

		trn = this [1].Instantiate( size, setNodeTrn).transform;
		trn.name = "Right";
		trn.SetParent (nodeRectTrn);

		putTransformsSideBySide (size);
		return nodeRectTrn.gameObject;
	}

	public override void workify(){
		wNode.AddAsComponentTo (nodeRectTrn.FindChild ("Left").gameObject, this [0]);
		EventHandler.AddAsComponentTo(nodeRectTrn.FindChild ("=").gameObject, 0);
		wNode.AddAsComponentTo (nodeRectTrn.FindChild ("Right").gameObject, this [1]);
	}

	public Equation(Expression l, Expression r){
		childList = new List<EquationPart> ();
		Debug.Assert (l != null);
		l.parent = this;
		r.parent = this;
		childList.Add (l);
		childList.Add (r);
	}

	public override bool Equals(EquationPart other){
		if (!(other is Equation)) return false;
		return this [0].Equals (other [0]) && this [1].Equals (other [1]);
	}

	public override string ToString () {
		return this[0].ToString () + "=" + this[1].ToString ();
	}

	public override BasicModel clone(){
		return new Equation (this[0].clone () as Expression, this[1].clone () as Expression);
	}


	public override EquationPart replaceWith (EquationPart newNode) {
		Debug.Assert (newNode is Equation);
		return base.replaceWith (newNode);
	}
	/*
	public Equation cloneAndApply (CNDictionary dict){
		return new Equation (
			this[0].clone().apply (dict), this[1].clone().apply (dict)
		);
	}
*/
}
