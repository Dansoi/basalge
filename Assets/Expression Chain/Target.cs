using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Target : MonoBehaviour {
	// Use this for initialization
	public void SetTarget (EquationPart target, int size, string linkStr) {
		//equal (n-1)
		RectTransform rc = Builder.InstantiateAtomic (size, linkStr).GetComponent<RectTransform>();
		rc.name = "Link n-1";
		rc.SetParent (transform);
		//Position
		rc.pivot = new Vector2 (1f, 1f);
		rc.anchoredPosition = new Vector2 (-10f, -Builder.spaceBetweenWorklines);
		float linkHeight = rc.sizeDelta.y;

		// dot, dot, dot
		rc = Builder.InstantiateAtomic (size, "...").GetComponent<RectTransform>();
		rc.name = "Step n-1";
		rc.SetParent (transform);
		//Position
		rc.pivot = new Vector2 (0f, 1f);
		rc.anchoredPosition = new Vector2 (0.0f, -Builder.spaceBetweenWorklines);

		//equal n
		rc = Builder.InstantiateAtomic (size, linkStr).GetComponent<RectTransform>();
		rc.name = "Link n";
		rc.SetParent (transform);
		//Position
		rc.pivot = new Vector2 (1f, 1f);
		rc.anchoredPosition = new Vector2 (-10f, -2f*Builder.spaceBetweenWorklines-linkHeight);
	
		// Target
		rc = target.Instantiate( size).GetComponent<RectTransform>();
		rc.name = "Step n";
		rc.SetParent (transform);
		//Position
		rc.pivot = new Vector2 (0f, 1f);
		rc.anchoredPosition = new Vector2 (0f, -2f * Builder.spaceBetweenWorklines - linkHeight);

		Builder.emphasize (transform, Color.grey);
	}

	public void clear(){
		foreach (Transform c in transform) {
			Destroy (c.gameObject);
		}
	}
}
