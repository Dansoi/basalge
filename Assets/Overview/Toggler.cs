using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Toggler : MonoBehaviour {
	public bool isUnlocked;

	public void unlock(){
		isUnlocked = true;

		gameObject.SetActive (true);
		GetComponent<Toggle> ().isOn = true;
	}


	public bool isChecked(){
		return GetComponent<Toggle> ().isOn;
	}
}
