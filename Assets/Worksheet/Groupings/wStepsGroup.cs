using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class wStepsGroup : MonoBehaviour {
	public LayoutElement widthBoundLayout;

	public void clear () {
		foreach (Transform c in transform) {
			Destroy (c.gameObject);
		}
		GetComponent<LayoutElement> ().preferredWidth = 100f;
	}

	public void addStep(GameObject newStep, float centerYPos, bool leftAdjust){
		RectTransform stepRT = newStep.GetComponent<RectTransform> ();
		stepRT.SetParent (transform);

		//Positioning
		stepRT.anchorMax = stepRT.anchorMin = new Vector2 (leftAdjust ? 0f : 0.5f,
															transform.parent.GetComponent<EndChain> ().isTop ? 1f : 0f);
		stepRT.pivot = new Vector2 (leftAdjust ? 0f : 0.5f, 0.5f);
		stepRT.anchoredPosition = new Vector2 (0f, centerYPos);

		LayoutElement thisLayout = gameObject.GetComponent<LayoutElement> ();
		if(stepRT.sizeDelta.x > thisLayout.preferredWidth) {
			widthBoundLayout.preferredWidth = thisLayout.preferredWidth = stepRT.sizeDelta.x;
		}
	}

	public float getHeightOf(string stepID){
		return transform.FindChild (stepID).GetComponent<RectTransform> ().sizeDelta.y;
	}

	public void stepSetActive(string stepID, bool active){
		Transform stepToActivate = transform.FindChild (stepID);
		foreach (EventHandler c in stepToActivate.GetComponentsInChildren<EventHandler>()) {
			c.enabled = active;
		}
	}

}
