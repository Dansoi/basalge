using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class wNode : MonoBehaviour {
	public EquationPart node;

	public static void AddAsComponentTo(GameObject expressionObj, EquationPart node) {
		wNode wn = expressionObj.AddComponent<wNode> ();
		wn.node = node;
		wn.node.workify ();
	}

}
