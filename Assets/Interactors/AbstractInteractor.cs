﻿using UnityEngine;
using System.Collections;

public abstract class AbstractInteractor {

	public abstract void Update();
	public abstract void mouseEnter (EventHandler eh);
	public abstract void mouseReEnter ();
	public abstract void mouseExit (EventHandler eh);
	public abstract void mouseClick ();

}
