using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Face : MonoBehaviour
{
    public List<Piece> facePieces;
    public FaceType faceType;

    public readonly Dictionary<FaceType, Vector3> searchBoxSizes = new Dictionary<FaceType, Vector3> {
        {FaceType.F, new Vector3(.3f, 3f, 3f) },
        {FaceType.B, new Vector3(.3f, 3f, 3f) },
        {FaceType.U, new Vector3(3f, .3f, 3f) },
        {FaceType.D, new Vector3(3f, .3f, 3f) },
        {FaceType.L, new Vector3(3f, 3f, .3f) },
        {FaceType.R, new Vector3(3f, 3f, .3f) },
        {FaceType.M, new Vector3(3f, 3f, .3f) },
        {FaceType.E, new Vector3(3f, .3f, 3f) },
        {FaceType.S, new Vector3(.3f, 3f, 3f) },
    };
    public bool resolvedFaces;
    public enum FaceType
    {
        F,
        B,
        U,
        D,
        L,
        R,
        M,
        E,
        S
    }
}
