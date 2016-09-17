using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public abstract class ClickableRule : MonoBehaviour {

	public BasicModel model;

	public virtual void clicked(){
		if (!GameManager.inst.exerciseMode) return;

		GameObject adaptingRule = Instantiate ( Builder.inst.adaptingRulePrefab );
		adaptingRule.GetComponent<AdaptingRule> ().rule = this;
	}

	public void draw( string name){
		
		RectTransform expressionRT;
		if (name == "") {
			expressionRT = model.Instantiate (Builder.worksheetFontsize).GetComponent<RectTransform>();
		} else {
			expressionRT = Builder.InstantiateAtomic (Builder.worksheetFontsize, name).GetComponent<RectTransform>();
		}
		expressionRT.SetParent (transform);

		expressionRT.anchorMin = expressionRT.anchorMax = expressionRT.pivot = new Vector2 (0.5f, 0.5f);
		expressionRT.anchoredPosition = Vector2.zero;

		// Set size and position of this RectTransform
		float innerWidth = expressionRT.sizeDelta.x;
		float innerHeight = expressionRT.sizeDelta.y;

		GetComponent<RectTransform> ().sizeDelta = new Vector2 (innerWidth+40f, innerHeight+10f);
	}

}
