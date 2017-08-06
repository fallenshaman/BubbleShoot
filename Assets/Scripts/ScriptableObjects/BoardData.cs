using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BoardData : ScriptableObject
{
	public List<BoardRow> listRows;
}

[Serializable]
public class BoardRow
{
	public BoardCell[] cells = new BoardCell[12];
}