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
	public ClickableRule rule; //reference to tree in the actual (one and only) rule. No altering this one.
	public BasicModel appliedRoot;

	GameObject ruleObj;

	void Start(){
		if (inst != null) Destroy(inst.gameObject);
		inst = this;
		name = "Adapting Rule";

		Camera camThatSeesRule = rule.GetComponentInParent<Canvas> ().worldCamera;
		Vector3 ruleScreenPosition = camThatSeesRule.WorldToScreenPoint (rule.transform.position) + Vector3.down*60f;
		transform.SetParent (Overlay.inst.transform);
		transform.position = ruleScreenPosition;

		mouseToThis = transform.position - Input.mousePosition;
		if (rule is ClickableExercise && ((ClickableExercise)rule).completionStage == 2) { //if so, then rule must be an biimplication thats only proved one way
			flipped = ((ClickableExercise)rule).isLeftwayCompleted;
		} else {
			flipped = false;
		}
		dict = new CNDictionary ();
		applyDictionary ();
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
				if (((ClickableExercise) rule).completionStage != 3) return;
			}
			flipped = !flipped;
			freeVarSearch ();

			Worksheet.inst.interactor.mouseReEnter ();
		}
	}

	private void freeVarSearch(){
		HashSet<char> keyVars = getKeyNode().getVarSet();
		HashSet<char> freeVars = getReplaceNode ().getVarSet ();
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
			}
		}
	}

	public EquationPart getKeyNode(){
		if(flipped){ //Implications (that are allowed to be used) will always have excactly one assumption
			return rule.model[1];
		} else {
			return rule.model[0];
		}
	}
	private EquationPart getReplaceNode(){
		if(flipped){
			return rule.model[0];
		} else {
			return rule.model[1];
		}
	}


	private void clearInput(){
		if (inputFieldInstance != null) {
			inputFieldInstance.GetComponent<FreeVarInput> ().node = null;
			inputFieldInstance.GetComponent<InputField> ().text = "";
		}
	}

	public void applyDictionary(){
		Expression inputNode = null;
		char inputVar = '0';
		if (inputFieldInstance != null) {
			inputNode = inputFieldInstance.GetComponent<FreeVarInput> ().node;
			inputVar = inputFieldInstance.GetComponent<FreeVarInput> ().varName;
		}

		if (inputNode != null) dict [inputVar] = inputNode;
		if (rule.model is Equation) {
			appliedRoot = new Equation (
				getKeyNode ().clone ().apply (dict) as Expression,
				getReplaceNode ().clone ().apply (dict) as Expression
			);
		} else {
			Debug.Assert (rule.model is Implication);
			List<EquationPart> list = new List<EquationPart> ();
			list.Add (getKeyNode ().clone ().apply (dict) as EquationPart);
			list.Add (getReplaceNode ().clone ().apply (dict) as EquationPart);
			appliedRoot = new Implication (list, false);
		}

		if (inputNode != null) dict.Remove (inputVar);
	}


	public void repaint(){ //This method will f*** up if called more than once between the same frames (because then body.gameObject is not being destroyed before the next call)
		
		Transform body = transform.FindChild ("Equation"); //Husk, dataen om modellen refereres fra denne klasse. Denne klasse kan ikke undværes ved at referere fra "Equation" istedet for, fordi den bliver slettet her, og skabt igen. Det er nødvendigt med denne "wrapper"
		if (body != null) Destroy (body.gameObject);


		ruleObj = appliedRoot.Instantiate ( Builder.worksheetFontsize);
		ruleObj.GetComponent<RectTransform> ().pivot = new Vector2 (0.5f, 0.5f);
		ruleObj.transform.SetParent(transform);
		ruleObj.transform.localPosition = Vector3.zero;
		ruleObj.name = "Equation";

		Builder.emphasize(ruleObj.transform.FindChild ("Left"), Color.red);
	}

	public void clearAndApply(bool doClearInput, bool doRepaint){
		dict = new CNDictionary ();
		if (doClearInput) clearInput ();
		applyDictionary ();
		if(doRepaint) repaint ();
	}

	public override string ToString(){
		return appliedRoot.ToString ();
	}
}

