using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class wLinksGroup : MonoBehaviour {

	public void clear () {
		foreach (Transform c in transform) {
			Destroy (c.gameObject);
		}
	}

	public void addLink(GameObject newLink, float centerYPos){
		RectTransform linkRT = newLink.GetComponent<RectTransform> ();
		linkRT.SetParent (transform);

		//Positioning
		linkRT.anchorMax = linkRT.anchorMin = new Vector2 (1f, transform.parent.GetComponent<EndChain> ().isTop ? 1f : 0f);
		linkRT.pivot = new Vector2 (1f, 0.5f);
		linkRT.anchoredPosition = new Vector2 (0f, centerYPos);
	}

	public void freeze(){
		foreach (Button b in GetComponentsInChildren<Button>()) {
			b.interactable = false;
		}
	}

}
