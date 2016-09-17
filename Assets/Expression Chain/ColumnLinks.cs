using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ColumnLinks : MonoBehaviour {

	public void clear () {
		foreach (Transform c in transform) {
			Destroy (c.gameObject);
		}
	}

	public void addLink(GameObject newLink, float centerYPos){
		RectTransform linkRT = newLink.GetComponent<RectTransform> ();
		linkRT.SetParent (transform);

		//Positioning
		linkRT.pivot = new Vector2 (1f, 0.5f);
		linkRT.anchoredPosition = new Vector2 (-10f, centerYPos);
	}

	public void removeLink(string linkID){
		Destroy ( transform.FindChild (linkID).gameObject );

	}
}
