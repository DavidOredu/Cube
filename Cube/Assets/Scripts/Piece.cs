using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Piece : MonoBehaviour
{
    public GameObject pieceObject;
    public PieceType pieceType;

    private void Start()
    {
        pieceObject = gameObject;
    }
    public enum PieceType
    {
        Core,
        Center,
        Edge,
        Corner,
    }
}
