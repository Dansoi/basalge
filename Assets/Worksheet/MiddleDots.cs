using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MiddleDots : MonoBehaviour {
	public GameObject dotsLink;
	public GameObject dotdotdot;

	public void Clear(){
		gameObject.SetActive (false);
	}

	public void setup(string linkStr, bool placeLinkAbove){
		gameObject.SetActive (true);
		float dotsHeight = dotdotdot.GetComponent<Text> ().preferredHeight;
		GetComponent<LayoutElement> ().preferredHeight = dotsHeight + 2*Builder.spaceBetweenWorklines;
		dotsLink.GetComponent<Text> ().text = linkStr;

		dotsLink.GetComponent<RectTransform> ().anchoredPosition =
			(placeLinkAbove ? new Vector2 (0f, dotsHeight * 0.5f + Builder.spaceBetweenWorklines * 0.5f) : Vector2.zero);
	}
}
