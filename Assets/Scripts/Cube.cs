using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Cube : MonoBehaviour
{
    public List<Piece> pieces = new List<Piece>();
    private List<Piece> centers = new List<Piece>();

    public List<Face> faces;

    private bool isMoving;
    private RaycastHit hit;

    public Material debugMaterial;

    private void Start()
    {
        pieces = GetComponentsInChildren<Piece>().ToList();
        faces = GetComponentsInChildren<Face>().ToList();

        foreach (var piece in pieces)
        {
            if (piece.pieceType == Piece.PieceType.Center)
                centers.Add(piece);
        }
    }
    private void FixedUpdate()
    {
        if (!isMoving)
        {
          //  GetFaces();
        }

        if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100f))
        {
            
        }   
    }
    // get face pieces
    private void GetFaces()
    {
        foreach (var face in faces)
        {
            if (face.resolvedFaces) { continue; }

            var pieces = GetFacePieces(face.transform.position, face.searchBoxSizes[face.faceType], Quaternion.identity);
            foreach (var piece in pieces)
            {
                var p = piece.GetComponent<Piece>();
                if(p)
                    face.facePieces.Add(p);
            }
            face.resolvedFaces = true;
        }
    }
    public Collider[] GetFacePieces(Vector3 center, Vector3 halfExtent, Quaternion quaternion)
    {
        return Physics.OverlapBox(center, halfExtent, quaternion);
    }
    public void MoveFace(Face chosenFace, float direction)
    {
        foreach (var piece in chosenFace.facePieces)
        {
            piece.transform.RotateAround(chosenFace.transform.position, Vector3.forward, 90f * direction);
            Debug.Log("Moving Piece");
        }
    }
    [ContextMenu("Move")]
    public void MoveFaceDebug()
    {
        var chosenFace = faces[1];
        foreach (var piece in chosenFace.facePieces)
        {
            piece.transform.RotateAround(chosenFace.transform.position, chosenFace.transform.right, 90f);
            Debug.Log("Moving Piece: " + chosenFace.faceType);
        }
    }
    /// <summary>
    /// Gets a piece in the cube by it's piece type.
    /// </summary>
    /// <param name="pieceType">Piece type of the piece being searched for.</param>
    /// <returns>Piece.</returns>
    public Piece GetPiece(Piece.PieceType pieceType)
    {
        foreach (var piece in pieces)
        {
            if(pieceType == piece.pieceType)
            {
                return piece.GetComponent<Piece>();
            }
        }
        return null;
    }
    public List<Piece> GetCenters()
    {
        return centers;
    }
    private void OnDrawGizmos()
    {
       
    }
}
