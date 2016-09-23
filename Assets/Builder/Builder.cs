using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class Builder : MonoBehaviour {
	public static Builder inst; //instance
	public const float deltaZ = 0f; //consts can be adressed like Builder.deltaZ (they are actually static)
	public const float spaceBetweenWorklines = 10f;
	public const int worksheetFontsize = 24; //Hvis denne ændres, så tilpas også Preferred Width i LayoutKomponenten i: Alle tre links (i modsætning til step og usedRule), og begge spacer
	public const float fadedFactor = 0.5f;
	public Color xClr = new Color(1f, 0f, 0f);
	public Color yClr = new Color(199f/255f, 31f/255f, 153f/255f);
	public Color zClr = new Color (213f/255f, 105f/255f, 30f/255f);
	public Color fadedClr = new Color (0.5f, 0.5f, 0.5f);
	public Color emphClr = new Color (1f, 1f, 1f);


	public GameObject adaptingRulePrefab;
	public GameObject assumptionPrefab;
	public GameObject linkPrefab;
	public GameObject exerciseRulePrefab;

	public GameObject dummyObj;
	public GameObject textObj;

	void Awake(){
		inst = this;
	}



	public static GameObject InstantiateDummy(string name){
		GameObject obj = Instantiate (inst.dummyObj);
		obj.name = name;
		return obj;
	}

	//Initiate textObj, set text, fontsize and transform size, and set name
	public static GameObject InstantiateTextObj(int size, string text, string name = ""){
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

	public static void emphasizeRange(BasicModel firstInRange, BasicModel lastInRange, Color clr, bool emphasizeOuterparentheses = false){
		if (lastInRange == null) lastInRange = firstInRange;

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

			if (emphasizeOuterparentheses && _firstInRange.parent is Expression) {
				if(_firstInRange.parent is Sum){
					Sum parent = (Sum)_firstInRange.parent;
					int index = _firstInRange.indexNo ();
					emphasize (parent.nodeRectTrn.FindChild ("Term "+index+" ("), clr);
					emphasize (parent.nodeRectTrn.FindChild ("Term "+index+" )"), clr);
				} else {
					Debug.Assert(_firstInRange.parent is Negative);
					Negative parent = (Negative)_firstInRange.parent;
					emphasize (parent.nodeRectTrn.FindChild ("("), clr);
					emphasize (parent.nodeRectTrn.FindChild (")"), clr);

				}
			
			}
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
