using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class Variable : Expression {

	public char varName;
	//int index; // TODO (læs GDD)


	public override GameObject Instantiate(int size, bool setNodeTrn ){
		nodeRectTrn = Builder.InstantiateTextObj (size, this.ToString (), "Variable").GetComponent<RectTransform>();
		return nodeRectTrn.gameObject;
	}

	//return null if str is invalid
	public new static Variable parse(string str){
		if (str.Length != 1) return null;
		if (str [0] < 'x' || str [0] > 'z')	return null;
		return new Variable (str.ToLower() [0]);
	}

	public override void workify(){
		EventHandler.AddAsComponentTo(nodeRectTrn.gameObject, 0);
	}

	public Variable (char c){
		childList = new List<EquationPart> ();
		varName = c;
	}

	public override HashSet<char> getVarSet() {
		HashSet<char> res = new HashSet<char>();
		res.Add(varName);
		return res;
	}

	public override BasicModel apply(CNDictionary dict, List<ColoredRange> crList, bool useLeafsForRangeIfSameType){
		if (dict.ContainsKey (varName)) {
			Expression replacer = dict [varName].clone () as Expression; //Important detail: dict[varname] can be a foster-Sum, where its child has 'incorrect' parent-references. However, clone() will create a new Sum, where its cloned children have correct parent-references.
			this.replaceWith (replacer);
			if (crList != null) {
				
				if(useLeafsForRangeIfSameType && replacer.GetType() == parent.GetType()) {
					crList.Add(new ColoredRange(replacer[0], replacer[replacer.Count()-1], getColor(varName)));
				} else {
					crList.Add(new ColoredRange(replacer, replacer, getColor(varName)));
				}
			}
			return replacer;
		} else {
			if (crList != null) {
				crList.Add(new ColoredRange(this, this, getColor(varName)));
			}
			return this;
		}
	}
	
	public Color getColor(char var){
		if( var == 'x'){ return Builder.inst.xClr; }
		if( var == 'y'){ return Builder.inst.yClr; }
		if( var == 'z'){ return Builder.inst.zClr; }
		return Color.grey;
	}

	public override bool Equals(EquationPart other){ //TODO: Examine if this will do. Maybe accept a small error due to using floats?
		if (!(other is Variable)) {
			return false;
		}
		Variable otherCasted = other as Variable;
		return this.varName == otherCasted.varName;
	}

	public override List<CNDictionary> createDictionariesTo (EquationPart other, bool returnFirst = false){
		List<CNDictionary> res = new List<CNDictionary>();
		res.Add(createDictionaryTo(other));
		return res;
	}

	public override CNDictionary createDictionaryTo (EquationPart other) {
		if(!(other is Expression)) return null;
		Expression _other = other as Expression;
		CNDictionary dict = new CNDictionary ();
		dict.Add (varName, _other);
		return dict;
	}

	public override string ToString () {
		return varName.ToString ();
	}

	public override BasicModel clone(){
		Expression res = new Variable (varName);
		return res;
	}

}
