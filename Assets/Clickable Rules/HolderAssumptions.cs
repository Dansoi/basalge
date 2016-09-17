using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HolderAssumptions : MonoBehaviour {
	public static HolderAssumptions inst; //instance
	public GameObject assumptionPrefab;

	void Awake(){
		inst = this;
	}
	public void setAssumption(Equation assumption){
		Clear ();
		GetComponentInChildren<Text> ().enabled = true;

		GameObject assumptionObj = Instantiate (assumptionPrefab);
		assumptionObj.transform.SetParent(transform);
		assumptionObj.GetComponent<ClickableAssumption>().attachModelAndDraw(assumption);

		float assumptionWidth = assumptionObj.GetComponent<RectTransform> ().sizeDelta.x;
		assumptionObj.transform.localPosition = new Vector2 (10f+0.5f*assumptionWidth, 0.0f);
	}

	public void Clear(){
		foreach (ClickableAssumption r in GetComponentsInChildren<ClickableAssumption>()) {
			Destroy (r.gameObject);
		}
		GetComponentInChildren<Text> ().enabled = false;

	}
		
}
