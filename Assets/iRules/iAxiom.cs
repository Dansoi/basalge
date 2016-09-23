using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class iAxiom : iRule {
	public string ruleName;
	public string modelStr;

	void Start() {
		model = BasicModel.parse (modelStr);
		draw (ruleName);
	}
}
