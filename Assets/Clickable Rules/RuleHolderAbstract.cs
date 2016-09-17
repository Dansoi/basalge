using UnityEngine;
using System.Collections;

public abstract class RuleHolderAbstract : MonoBehaviour {

	bool updateLayoutOnUpdate;
	public abstract void setLayout ();

	public void setLayoutOnUpdate () {
		updateLayoutOnUpdate = true;
	}

	// Update is called once per frame
	void Update () {
		if (!updateLayoutOnUpdate) return;
		updateLayoutOnUpdate = false;
		setLayout ();
	}
}
