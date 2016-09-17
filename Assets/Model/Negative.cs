using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Negative : Expression {
	
	public new static Negative parse(string str){
		if (str == "") return null;
		Expression inner = Expression.parse(str.Substring (0, str.Length - 1));
		if (inner == null) return null;

		if (str [str.Length - 1] != '\'') {
			return null;
		}

		return new Negative (inner);
	}

	public Negative(Expression i){
		childList = new List<EquationPart> ();
		childList.Add (i);
		i.parent = this;
	}

	public override GameObject Instantiate(int size, bool setNodeTrn){
		nodeRectTrn = Builder.InstantiateDummy("Inverse").GetComponent<RectTransform>();

		Transform trn;

		bool innerP = (this[0] is Negative || this[0] is Sum);

		if (innerP) Builder.InstantiateAtomic (size, "(").transform.SetParent (nodeRectTrn);

		trn = this [0].Instantiate(size, setNodeTrn).transform;
		trn.name = "Inner";
		trn.SetParent (nodeRectTrn);

		if (innerP) Builder.InstantiateAtomic (size, ")").transform.SetParent (nodeRectTrn);

		Builder.InstantiateAtomic (size, "'").transform.SetParent (nodeRectTrn);

		putTransformsSideBySide (size);
		return nodeRectTrn.gameObject;
	}


	public override void workify(){
		WorkNode.AddAsComponentTo (nodeRectTrn.FindChild ("Inner").gameObject, this [0]);
		EventHandler.AddAsComponentTo(nodeRectTrn.FindChild ("'").gameObject, 0);

		Transform leftP = nodeRectTrn.FindChild ("(");
		Transform rightP = nodeRectTrn.FindChild (")");
		if (leftP != null) EventHandler.AddAsComponentTo(leftP.gameObject, 0);
		if (rightP != null) EventHandler.AddAsComponentTo(rightP.gameObject, 0);
	}


	public override string ToString(){
		return this[0].ToString () + "'";
	}

	public override bool Equals(EquationPart other){
		if (!(other is Negative)) return false;
		return this[0].Equals (other[0]);
	}

	public override EquationPart clone(){
		return new Negative (this[0].clone() as Expression);
	}

}
