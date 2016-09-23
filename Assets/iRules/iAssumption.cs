using UnityEngine;
using System.Collections;

public class iAssumption : iRule {

	public void attachModelAndDraw(EquationPart model){
		this.model = model;
		draw ("");
	}
}
