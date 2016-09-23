using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class WorkRule : EventTrigger {

	//Even nodes are firstInRange nodes. Odd nodes are lastInRange nodes
	//They must be separated in two list, because worksheet will need to use only one of them, when stitching references in the last hint to the other endChain

	public CRList towardsEdgeRangeList;
	public CRList thisRangeList;
	public CRList towardsDotsRangeList;
	//List<List<Node>> variableRanges; //use this to color ranges that correspond to x one color, ranges that correspond to y another color, ...

	public static void AddAsComponentTo(GameObject obj){
		WorkRule wr = obj.AddComponent<WorkRule> ();
		wr.towardsEdgeRangeList = new CRList ();
		wr.thisRangeList = new CRList ();
		wr.towardsDotsRangeList = new CRList ();
	}


	public override void OnPointerEnter( PointerEventData data){
		if (AdaptingRule.inst != null) return;
		emphasize ();
	}

	public override void OnPointerExit( PointerEventData data){
		deemphasize ();
	}

	void emphasize(){
		
		emphasizeRange (thisRangeList);
		emphasizeRange (towardsEdgeRangeList);
		emphasizeRange (towardsDotsRangeList);
	}

	void deemphasize(){
		emphasizeRange (thisRangeList, true);
		emphasizeRange (towardsEdgeRangeList, true);
		emphasizeRange (towardsDotsRangeList, true);

	}


	void emphasizeRange(CRList list, bool fade = false){
		Builder.emphasizeRange (list.init.rangeStart, list.init.rangeEnd, fade ? Builder.inst.fadedClr : list.init.clr);
		foreach (ColoredRange c in list){
			Builder.emphasizeRange (c.rangeStart, c.rangeEnd, fade ? Builder.inst.fadedClr : c.clr, true);
		}
	}
}
