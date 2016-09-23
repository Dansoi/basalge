using UnityEngine;
using System.Collections;

public class ColoredRange {
	public BasicModel rangeStart;
	public BasicModel rangeEnd;
	public Color clr;

	public ColoredRange(BasicModel first, BasicModel last, Color color){
		rangeStart = first;
		rangeEnd = last;
		clr = color;
	}

	public override string ToString () {
		string res = "[" + rangeStart.ToString () + "-";
		if( rangeEnd == null ) res += "null";
		else res += rangeEnd.ToString ();
		res += ", " + clr.ToString ();
		return res;
	}
}
