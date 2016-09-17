using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CNDictionary : System.Collections.Generic.Dictionary<char, Expression> {

	public override string ToString(){
		//GameObject.FindObjectOfType<EmergencyLogger> ().print ("in CNDictionary.ToString()");
		//GameObject.FindObjectOfType<EmergencyLogger> ().print ("numKeys: " + Keys.Count);

		string res = "";
		foreach (char c in Keys) {
			res = "| " + res + c.ToString () + " --> " + this[c].ToString () + " |";
		}
		return res;
	}

	//Return false if its not possible to merge due to conflicting key-value pairs. If this method terminates like that, this dictionary's data is hardly usable, and you're better not use it
	public bool AddDictionary(CNDictionary other){
		foreach (char c in other.Keys) {
			if (ContainsKey (c)) {
				if (!(this[c].Equals( other[c] ))) {
					return false;
				}
			} else {
				Add (c, other[c]);
			}
		}
		return true;
	}

	public CNDictionary shallowClone() { //What the dictionaries maps into is not being cloned
		CNDictionary res = new CNDictionary ();
		foreach (char c in Keys) {
			res.Add (c, this [c]);
		}
		return res;
	}

}
