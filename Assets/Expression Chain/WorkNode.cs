using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class WorkNode : MonoBehaviour {
	public EquationPart node;

	public static void AddAsComponentTo(GameObject expressionObj, EquationPart node) {
		WorkNode wn = expressionObj.AddComponent<WorkNode> ();
		wn.node = node;
		wn.node.workify ();
	}

}
