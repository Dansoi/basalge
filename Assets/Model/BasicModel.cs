using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class BasicModel {

	public List<EquationPart> childList;
	public RectTransform nodeRectTrn;

	public virtual BasicModel apply (CNDictionary dict, List<ColoredRange> crList = null, bool useLeafsForRangeIfSameType = false) {
		for(int i = 0; i < childList.Count; ++i){
			this [i].apply (dict, crList);
		}
		return this;
	}

	//Clones the node itself and descendants, but not ancestors (so the returned node will have parent == null)
	public abstract BasicModel clone ();


	public int Count(){
		return childList.Count;
	}

	public static BasicModel parse(string str){
		if (str.IndexOf ("<=>") >= 0 || str.IndexOf ("==>") >= 0 || str.IndexOf ("<==") >= 0) {
			return Implication.parse (str);
		} else {
			return Equation.parse (str);
		}
	}
	public abstract GameObject Instantiate (int size, bool setNodeTrn = false);

	public abstract override string ToString();

	public EquationPart this[int index]{
		get { return childList[index]; }
		set { childList[index] = value; }
	}

	public void putTransformsSideBySide (int size){ //Put them side by side in the order they appear in the hierarchy

		float maxHeight = 0f;
		float widthUntilNow = 0f;
		foreach (RectTransform rt in nodeRectTrn) {
			if (rt.sizeDelta.y > maxHeight) maxHeight = rt.sizeDelta.y;

			rt.anchoredPosition = new Vector2 (widthUntilNow, 0f);
			widthUntilNow += rt.sizeDelta.x;
		}

		nodeRectTrn.sizeDelta = new Vector2 (widthUntilNow, maxHeight);
	}


	/// I dont like this..:
	public abstract void makeBackwards();
}
