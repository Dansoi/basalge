using UnityEngine;
using System.Collections;

public class ColumnSteps : MonoBehaviour {

	public void clear () {
		foreach (Transform c in transform) {
			Destroy (c.gameObject);
		}
		GetComponent<RectTransform> ().sizeDelta = new Vector2 (1f, 100f);
	}


	public float removeStep(string stepID){
		RectTransform thisRC = GetComponent<RectTransform> ();
		thisRC.sizeDelta = new Vector2 (getWidestChild(stepID), 100f);
		Transform stepTrn = transform.FindChild (stepID);
		float height = stepTrn.GetComponent<RectTransform> ().sizeDelta.y;
		Destroy ( stepTrn.gameObject );
		return height;
	}

	float getWidestChild(string objectToIgnore){
		float widestChildSoFar = 5f; //minimum width of this recttransform
		foreach (Transform c in transform) {
			if (c.name == objectToIgnore) continue;
			float childWidth = c.GetComponent<RectTransform> ().sizeDelta.x;
			if (childWidth > widestChildSoFar) widestChildSoFar = childWidth;
		}
		return widestChildSoFar;
	}

	public void addStep(GameObject newStep, float topYPos, bool leftAdjust){
		RectTransform stepRT = newStep.GetComponent<RectTransform> ();

		stepRT.SetParent (transform);

		//Positioning
		if (leftAdjust) {
			stepRT.anchorMax = stepRT.anchorMin = stepRT.pivot = new Vector2 (0f, 1f); //Left-adjust
		} else {	
			stepRT.anchorMax = stepRT.anchorMin = stepRT.pivot = new Vector2 (0.5f, 1f); //Center-adjust
		}

		RectTransform thisRT = gameObject.GetComponent<RectTransform> ();
		stepRT.anchoredPosition = new Vector2 (0f, topYPos);

		float oldWidth = thisRT.sizeDelta.x;
		if(stepRT.sizeDelta.x > oldWidth) thisRT.sizeDelta = new Vector2 (stepRT.sizeDelta.x, 100f);

	}


	public void stepSetActive(string stepID, bool active){
		Transform stepToActivate = transform.FindChild (stepID);
		foreach (EventHandler c in stepToActivate.GetComponentsInChildren<EventHandler>()) {
			c.enabled = active;
		}
	}

}
