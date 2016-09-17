using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Toggler : MonoBehaviour {
	public bool isUnlocked;

	public void unlock(){
		isUnlocked = true;

		setEnable (true);
		GetComponent<Toggle> ().isOn = true;
	}

	public void setEnable(bool b){
		GetComponent<Toggle> ().enabled = b;

		foreach (Image i in GetComponentsInChildren<Image>()) { //both the Image in this gameobject, and the Image in the proper child is here...
			i.enabled = b;
		}
	}

	public bool isChecked(){
		return GetComponent<Toggle> ().isOn;
	}



}
