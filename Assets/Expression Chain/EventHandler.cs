using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class EventHandler : EventTrigger {
	public int nodePart; //nodePart == 0 means its not going to be used.. 1, 2, 3, 4 is where its relevant

	public static void AddAsComponentTo(GameObject obj, int nodePart){
		EventHandler eh = obj.AddComponent<EventHandler> ();
		eh.nodePart = nodePart;
	}

	public override void OnPointerEnter( PointerEventData data){
		Worksheet.inst.interactor.mouseEnter (this);
	}
	public override void OnPointerDown( PointerEventData data){
		if(data.button != PointerEventData.InputButton.Left) return;
		Worksheet.inst.interactor.mouseClick ();
	}
	public override void OnPointerExit( PointerEventData data){
		Worksheet.inst.interactor.mouseExit (this);
	}
}
