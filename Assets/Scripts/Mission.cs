using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Mission {
	public enum Type
	{
		KNOCK_DOWN_BEES = 0,
		COLLECT_BEEHIVES,
		BUBBLES_INTO_POT,
		MAX
	}

	public Type type;		// Type of missions
	public int goal;		// Goal to clear mission
	public int boardID;		// game board data id
}