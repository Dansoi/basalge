using UnityEngine;
using System.Collections.Generic;

public class CRList : List<ColoredRange> {
	public ColoredRange init;

	public override string ToString(){
		string res = init.ToString() + "|";
		foreach (ColoredRange c in this) {
			res += "|" + c.ToString ();
		}
		return res;
	}


}
