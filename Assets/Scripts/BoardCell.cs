using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct BoardCell
{
	public enum CellType
	{
		EMPTY = 0,
		NORMAL,
		HIVE,
		BEE,
	}

	public CellType type;
	public Bubble.ColorType color;
	public Bubble.TrapType trap;
}
