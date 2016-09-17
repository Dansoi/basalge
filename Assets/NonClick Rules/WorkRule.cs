using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class WorkRule : EventTrigger {

	List<EquationPart> nodeList; //even nodes are firstInRange nodes. Odd nodes are lastInRange nodes
	//List<List<Node>> variableRanges; //use this to color ranges that correspond to x one color, ranges that correspond to y another color, ...

	public static void AddAsComponentTo(GameObject obj){
		WorkRule wr = obj.AddComponent<WorkRule> ();
		wr.nodeList = new List<EquationPart> ();
	}
	public void addListener(EquationPart firstInRange, EquationPart lastInRange){ //can be equal if just one node is to be emphasized on hover
		nodeList.Add(firstInRange);
		nodeList.Add(lastInRange);
	}
	public override void OnPointerEnter( PointerEventData data){
		if (AdaptingRule.inst != null) return;
		for (int i = 0; i < nodeList.Count; i += 2) {
			Builder.emphasizeRange (nodeList [i], nodeList [i + 1], Color.red);
		}
		Builder.emphasize (transform, Color.red);
	}

	public override void OnPointerExit( PointerEventData data){
		for (int i = 0; i < nodeList.Count; i += 2) {
			Builder.emphasizeRange (nodeList [i], nodeList [i + 1], Color.white);
		}
		Builder.emphasize (transform, Color.white);
	}
}
