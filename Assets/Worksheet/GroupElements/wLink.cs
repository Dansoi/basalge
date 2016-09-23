using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class wLink : MonoBehaviour {

	public static GameObject InstantiateWith(int size, string linkStr, string name){
		GameObject linkObj = Instantiate (Builder.inst.linkPrefab);
		linkObj.GetComponent<Text> ().fontSize = size;
		linkObj.GetComponent<Text> ().text = linkStr;
		linkObj.name = name;

		return linkObj;
	}

	public void clicked(){
		int stepNo = int.Parse (name.Substring (5));
		GetComponentInParent<EndChain> ().undo (stepNo);
	}
}
