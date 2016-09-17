using UnityEngine;
using System.Collections;

public class ClickableAssumption : ClickableRule {

	public void attachModelAndDraw(EquationPart model){
		this.model = model;
		draw ("");
	}
}
