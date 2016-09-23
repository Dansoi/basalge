using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ContainerIAssumption : MonoBehaviour {
	public static ContainerIAssumption inst; //instance

	void Awake(){
		inst = this;
	}
	public void setAssumption(Equation assumption){
		Clear ();
		GetComponentInChildren<Text> ().enabled = true;

		GameObject assumptionObj = Instantiate (Builder.inst.assumptionPrefab);
		assumptionObj.transform.SetParent(transform);
		assumptionObj.GetComponent<iAssumption>().attachModelAndDraw(assumption);

		float assumptionWidth = assumptionObj.GetComponent<RectTransform> ().sizeDelta.x;
		assumptionObj.transform.localPosition = new Vector2 (10f+0.5f*assumptionWidth, 0.0f);
	}

	public void Clear(){
		foreach (iAssumption r in GetComponentsInChildren<iAssumption>()) {
			Destroy (r.gameObject);
		}
		GetComponentInChildren<Text> ().enabled = false;

	}
		
}
