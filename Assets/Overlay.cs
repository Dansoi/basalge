using UnityEngine;
using System.Collections;

public class Overlay : MonoBehaviour {
	public static Overlay inst; //instance

	void Awake(){
		inst = this;
	}
}
