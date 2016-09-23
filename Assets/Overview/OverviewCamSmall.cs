using UnityEngine;
using System.Collections;

public class OverviewCamSmall : MonoBehaviour {

	// Update is called once per frame
	void Update () {
		if (GetComponent<Camera> ().ScreenToViewportPoint (Input.mousePosition).y < 0f) return;;
		float scrollWheel = Input.GetAxis ("Mouse ScrollWheel");
		if (scrollWheel != 0f) {
			move (100f * scrollWheel);
		}
	}

	private void move(float amount){
		Vector3 newPos = transform.localPosition + Vector3.up * amount;
		if (newPos.y < -50f) newPos.y = -50f;
		if (newPos.y > 250f) newPos.y = 250f;
		transform.localPosition = newPos;

	}
}
