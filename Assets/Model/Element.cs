using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class Element : Expression {

	private float number; //must not be public, or someone might test if two numbers are equal and not allowing a tiny error (it is floats afterall). Use equls().
	private char symbol;

	public override GameObject Instantiate(int size, bool setNodeTrn){
		nodeRectTrn = Builder.InstantiateAtomic (size, this.ToString (), "Element").GetComponent<RectTransform>();
		return nodeRectTrn.gameObject;
	}


	public override void workify(){
		EventHandler.AddAsComponentTo(nodeRectTrn.gameObject, 0);
	}

	public new static Element parse(string str){
		float number;
		if (Single.TryParse (str, out number)) {
			return new Element (number);
		}
		if (str.Length != 1) return null;
		if (str[0] < 'a' || str[0] > 'w') return null;

		return new Element (-1f, str[0]);
	}

	public Element(float num, char sym = '/'){
		childList = new List<EquationPart> ();
		number = num;
		symbol = sym;
	}


	public override List<CNDictionary> createDictionariesTo (EquationPart other, bool returnFirst = false){
		List<CNDictionary> res = new List<CNDictionary>();
		CNDictionary dict = createDictionaryTo(other);
		if (dict != null) res.Add (dict);
		return res;
	}

	public override CNDictionary createDictionaryTo (EquationPart other)
	{
		if (!(other is Element)) return null;
		Element otherNumber = other as Element;
		if (this.Equals (otherNumber)) {
			return new CNDictionary ();
		}
		return null;
	}

	public override string ToString(){
		if (number < 0f) return symbol.ToString ();
		return number.ToString();
	}

	public override bool Equals(EquationPart other){ //TODO: Examine if this will do. Maybe accept a small error due to using floats?
		if (!(other is Element)) {
			return false;
		}
		Element otherCasted = other as Element;
		return (this.number == otherCasted.number && this.symbol == otherCasted.symbol);
	}

	public override EquationPart clone(){
		Expression res = new Element (this.number, this.symbol);
		return res;
	}

}
