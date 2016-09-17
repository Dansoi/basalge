using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class Builder : MonoBehaviour {
	public static Builder inst; //instance
	public const float deltaZ = 0f; //consts can be adressed like Builder.deltaZ (they are actually static)
	public const float spaceBetweenWorklines = 10f;
	public const int worksheetFontsize = 24;

	public GameObject dummyObj;
	public GameObject textObj;
	public GameObject adaptingRulePrefab;

	void Awake(){
		inst = this;
	}



	public static GameObject InstantiateDummy(string name){
		GameObject obj = Instantiate (inst.dummyObj);
		obj.name = name;
		return obj;
	}

	//Initiate Atomic, set text, fontsize and transform size, and set name
	public static GameObject InstantiateAtomic(int size, string text, string name = ""){
		GameObject obj = Instantiate (inst.textObj);
		obj.GetComponent<Text> ().text = text;
		obj.GetComponent<Text> ().fontSize = size;

		/*
		// For this method to work, call "font.RequestCharactersInTexture (">abcdefghijklmnopqrstuvwxyz 0123456789.,()+'=\u21d2\u21d4", worksheetFontsize);" in the Awake() method

		int width = 0;
		foreach (char c in text) {
			CharacterInfo chInfo;
			bool success = font.GetCharacterInfo (c, out chInfo, size);
			Debug.Assert (success); //failure = character with given size not found in fonttexture
			width += chInfo.advance;
		}
		objTrn.GetComponent<RectTransform> ().sizeDelta = new Vector2 (width, objTrn.GetComponent<Text> ().preferredHeight);
		*/

		obj.GetComponent<RectTransform> ().sizeDelta = new Vector2 (obj.GetComponent<Text> ().preferredWidth, obj.GetComponent<Text> ().preferredHeight); //Important, that the text-element has horizontal-overflow set to overflow (not wrap!). Otherwise preferredHeight is way to high for multi-char strings.


		if (name == "") name = text;
		obj.name = name;
		return obj;
	}

	public static void emphasize(Transform transform, Color color){ //including the transform itself
		if (transform == null) return;
		foreach (Text txt in transform.GetComponentsInChildren<Text> ()) {
			txt.color = color;
		}
	}

	public static void emphasizeRange(EquationPart firstInRange, EquationPart lastInRange, Color clr){
		if (!(firstInRange is Expression)) {
			Debug.Assert (firstInRange == lastInRange);
			emphasize (firstInRange.nodeRectTrn, clr);
			return;
		}
		Expression _firstInRange = firstInRange as Expression;
		Expression _lastInRange = lastInRange as Expression;

		Debug.Assert (_firstInRange.parent == _lastInRange.parent);
		if(_firstInRange == _lastInRange){
			emphasize (_firstInRange.nodeRectTrn, clr);

		} else {
			Sum parent = _firstInRange.parent as Sum;
			int i = _firstInRange.indexNo ();
			Transform parentTrn = parent.nodeRectTrn;

			emphasize (parentTrn.FindChild ("Term "+i+" ("), clr);
			emphasize (parentTrn.FindChild ("Term "+i), clr);
			emphasize (parentTrn.FindChild ("Term "+i+" )"), clr);

			while(parent [i] != _lastInRange) {
				emphasize (parentTrn.FindChild ("Plus "+i), clr);
				i++;
				emphasize (parentTrn.FindChild ("Term "+i+" ("), clr);
				emphasize (parentTrn.FindChild ("Term "+i), clr);
				emphasize (parentTrn.FindChild ("Term "+i+" )"), clr);
			} 

		}
	}

}
