using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class AdaptingRule : MonoBehaviour {

	public GameObject inputFieldPrefab;
	private GameObject inputFieldInstance;

	public static AdaptingRule inst; //instance

	bool flipped;
	Vector3 mouseToThis;

	public CNDictionary dict;
	public CRList keyRanges;
	public CRList replacerRanges;

	public iRule rule; //reference to tree in the actual (one and only) rule. No altering this one.
	public BasicModel appliedRoot;

	void Start(){
		if (inst != null) Destroy(inst.gameObject);
		inst = this;
		name = "Adapting Rule";

		Camera camThatSeesRule = rule.GetComponentInParent<Canvas> ().worldCamera;
		Vector3 ruleScreenPosition = camThatSeesRule.WorldToScreenPoint (rule.transform.position) + Vector3.down*60f;
		transform.SetParent (Overlay.inst.transform);
		transform.position = ruleScreenPosition;

		mouseToThis = transform.position - Input.mousePosition;

		flipped = false;
		dict = new CNDictionary ();
		keyRanges = new CRList ();
		replacerRanges = new CRList ();

		applyDictionary (false, true);
		repaint ();
		freeVarSearch ();
	}

	void Update(){

		transform.position = Input.mousePosition + mouseToThis;

		if (Input.GetMouseButtonDown (0)) { //Input events are handled before Update-calls(). Everything that nedded this AdaptingRule has done what it had to do, since it was done in event-methods like OnMouseDown()
			inst = null;
			Destroy (gameObject);
			return;
		}

		if (Input.GetMouseButtonDown (1)) { //May not flip implication
			if (rule.model is Implication) {
				if (!((Implication)rule.model).bothWays) return;
			}
			flipped = !flipped;
			freeVarSearch ();

			Worksheet.inst.interactor.mouseReEnter ();
		}
	}

	private void freeVarSearch(){
		HashSet<char> keyVars = getKey().getVarSet();
		HashSet<char> freeVars = getReplacer ().getVarSet ();
		freeVars.ExceptWith(keyVars);

		if (freeVars.Count > 0) {
			if (freeVars.Count > 1) {
				Debug.LogError ("Cannot handle more than one free variables");
			}
			inputFieldInstance = Instantiate (inputFieldPrefab);
			inputFieldInstance.transform.SetParent (transform);
			inputFieldInstance.transform.localPosition = new Vector3 (0f, -60f, 0f);
			IEnumerator<char> e = freeVars.GetEnumerator ();
			e.MoveNext ();
			inputFieldInstance.GetComponent<FreeVarInput> ().setVar (e.Current);

		} else {
			if (inputFieldInstance != null) {
				Destroy (inputFieldInstance);
				inputFieldInstance = null;
			}
		}
	}

	public void flipApplyRepaint(){
		flipped = !flipped;
		freeVarSearch ();
	}

	public bool isFlipped(){
		return flipped;
	}

	public bool canKeyBePremise(){
		if (rule.model is Equation) return true;
		Implication m = (Implication)rule.model;
		if(m.bothWays) return true;
		return(m.rightWay != flipped);
	}
	public bool canKeyBeConclusion(){
		if (rule.model is Equation) return true;
		Implication m = (Implication)rule.model;
		if(m.bothWays) return true;
		return(m.rightWay == flipped);
	}


	public EquationPart getKey(){
		//Implications (that are allowed to be used) will always have excactly one assumption
		return rule.model[flipped ? 1 : 0];
	}

	private EquationPart getReplacer(){
		return rule.model[flipped ? 0 : 1];
	}

	public EquationPart getAppliedKey(){
		return appliedRoot [flipped ? 1 : 0];
	}
	public EquationPart getAppliedReplacer(){
		return appliedRoot [flipped ? 0 : 1];
	}


	public void applyDictionary(bool useDerivedDict, bool useInput){
		Expression inputNode = null;
		char inputVar = '0';
		if (inputFieldInstance != null) {
			inputNode = inputFieldInstance.GetComponent<FreeVarInput> ().node;
			inputVar = inputFieldInstance.GetComponent<FreeVarInput> ().varName;
		}

		CNDictionary d = (useDerivedDict) ? dict : new CNDictionary ();

		if (useInput && inputNode != null) d [inputVar] = inputNode;

		appliedRoot = rule.model.clone ();
		keyRanges.Clear ();
		getAppliedKey ().apply (d, keyRanges);
		keyRanges.init = new ColoredRange (getAppliedKey (), null, Builder.inst.emphClr);

		replacerRanges.Clear ();
		getAppliedReplacer ().apply (d, replacerRanges);
		replacerRanges.init = new ColoredRange (getAppliedReplacer (), null, Builder.inst.emphClr);


		if (useInput && inputNode != null) d.Remove (inputVar);
	}


	public void repaint(){ //This method will f*** up if called more than once between the same frames (because then body.gameObject is not being destroyed before the next call)
		Transform body = transform.FindChild ("Model"); //Husk, dataen om modellen refereres fra denne klasse. Denne klasse kan ikke undværes ved at referere fra "Equation" istedet for, fordi den bliver slettet her, og skabt igen. Det er nødvendigt med denne "wrapper"
		if (body != null) Destroy (body.gameObject);

		RectTransform ruleRT = appliedRoot.Instantiate ( Builder.worksheetFontsize).GetComponent<RectTransform> ();
		ruleRT.SetParent(transform);
		ruleRT.anchorMin = ruleRT.anchorMax = ruleRT.pivot = new Vector2 (0.5f, 0.5f);
		ruleRT.anchoredPosition = Vector2.zero;
		ruleRT.name = "Model";
		this.GetComponent<RectTransform> ().sizeDelta = new Vector2 (ruleRT.sizeDelta.x + 10f, ruleRT.sizeDelta.y + 10f);

		Builder.emphasizeRange (keyRanges.init.rangeStart, keyRanges.init.rangeEnd, keyRanges.init.clr);
		foreach(ColoredRange c in keyRanges){
			Builder.emphasizeRange (c.rangeStart, c.rangeEnd, c.clr, true);
		}


		Color clrFaded = Builder.inst.emphClr;
		clrFaded.a = Builder.fadedFactor;
		Builder.emphasizeRange (replacerRanges.init.rangeStart, replacerRanges.init.rangeEnd, clrFaded);
		foreach(ColoredRange c in replacerRanges){
			clrFaded = c.clr;
			clrFaded.a = Builder.fadedFactor;
			Builder.emphasizeRange (c.rangeStart, c.rangeEnd, clrFaded, true);
		}
	}



	public override string ToString(){
		return appliedRoot.ToString ();
	}
}

