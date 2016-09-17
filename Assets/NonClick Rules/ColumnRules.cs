using UnityEngine;
using System.Collections;

public class ColumnRules : MonoBehaviour {

	public void clear () {
		foreach (Transform c in transform) {
			Destroy (c.gameObject);
		}
		GetComponent<RectTransform> ().sizeDelta = new Vector2 (75f, 100f);
	}

	public void addRule (GameObject ruleObj, float centerYPos){
		Debug.Assert(ruleObj != null);
		ruleObj.transform.SetParent (transform);

		//Positioning
		RectTransform stepRT = ruleObj.GetComponent<RectTransform> ();
		stepRT.anchorMin = new Vector2 (0.5f, 1f);
		stepRT.anchorMax = new Vector2 (0.5f, 1f);
		stepRT.pivot = new Vector2 (0.5f, 0.5f);
		stepRT.anchoredPosition = new Vector2 (0f, centerYPos);

		RectTransform thisRT = gameObject.GetComponent<RectTransform> ();
		float oldWidth = thisRT.sizeDelta.x;
		if(stepRT.sizeDelta.x > oldWidth) thisRT.sizeDelta = new Vector2 (stepRT.sizeDelta.x, 100f);
	}

	public float removeRule(string ruleID){
		RectTransform thisRC = GetComponent<RectTransform> ();
		thisRC.sizeDelta = new Vector2 (getWidestChild(ruleID), 100f);
		Transform ruleTrn = transform.FindChild (ruleID);
		float height = ruleTrn.GetComponent<RectTransform> ().sizeDelta.y;
		Destroy ( ruleTrn.gameObject );
		return height;
	}

	float getWidestChild(string objectToIgnore){
		float widestChildSoFar = 75f; //minimum width of this recttransform
		foreach (Transform c in transform) {
			if (c.name == objectToIgnore) continue;
			float childWidth = c.GetComponent<RectTransform> ().sizeDelta.x;
			if (childWidth > widestChildSoFar) widestChildSoFar = childWidth;
		}
		return widestChildSoFar;
	}
}
