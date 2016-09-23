using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Overview : MonoBehaviour {
	public static Overview inst; //instance

	public Toggler impChain;
	public Toggler autoAssoc;
	public List<iExercise> exercisesToUnlockAutoAssoc;
	public List<iExercise> exercisesToUnlockImpChain;

	void Awake(){
		inst = this;
	}

}
