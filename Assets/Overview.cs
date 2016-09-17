using UnityEngine;
using System.Collections;

public class Overview : MonoBehaviour {
	public static Overview inst; //instance

	public Toggler autoAssoc;
	//public Toggler autoCommutAdd;
	//public Toggler autoCommutMult;

	void Awake(){
		inst = this;
	}

}
