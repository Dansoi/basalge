using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class wRulesGroup : MonoBehaviour {
	public LayoutElement widthBoundLayout;

	public void clear () {
		foreach (Transform c in transform) {
			Destroy (c.gameObject);
		}
		GetComponent<LayoutElement> ().preferredWidth = 100f;
	}

	public void addRule (GameObject ruleObj, float centerYPos){
		Debug.Assert(ruleObj != null);
		RectTransform ruleRT = ruleObj.GetComponent<RectTransform> ();
		ruleRT.SetParent (transform);

		//Positioning
		ruleRT.anchorMax = ruleRT.anchorMin = new Vector2 (0.5f, transform.parent.GetComponent<EndChain> ().isTop ? 1f : 0f);
		ruleRT.pivot = new Vector2 (0.5f, 0.5f);
		ruleRT.anchoredPosition = new Vector2 (0f, centerYPos);

		RectTransform thisRT = gameObject.GetComponent<RectTransform> ();
		LayoutElement thisLayout = gameObject.GetComponent<LayoutElement> ();
		if (ruleRT.sizeDelta.x > thisRT.sizeDelta.x) {
			widthBoundLayout.preferredWidth = thisLayout.preferredWidth = ruleRT.sizeDelta.x;
		}
	}

}
