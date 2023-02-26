using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Orientation
{
    X,
    Y,
    Z
}

[System.Serializable]
class FaceSearch
{
    public List<Transform> raycastPositions;
    public Color debugRayColor;
}
public class CubeOrientation : MonoBehaviour
{
    Cube cube;
    [SerializeField]
    private Transform coreTarget;

    [SerializeField]
    List<FaceSearch> faceSearchers;

    Orientation currentOrientation;
    [SerializeField]
    private Collider XAxisOrientationBox;
    [SerializeField]
    private Collider YAxisOrientationBox;
    [SerializeField]
    private Collider ZAxisOrientationBox;

    private void Start()
    {
        cube = GetComponent<Cube>();
        AdjustCores();
    }
    private void Update()
    {
        SetFacePieces();
    }
    public void AdjustCores()
    {
        SetCoreFaceType(coreTarget.up, Face.FaceType.U, Color.green);
        SetCoreFaceType(coreTarget.up * -1, Face.FaceType.D, Color.yellow);
        SetCoreFaceType(coreTarget.right, Face.FaceType.L, Color.red);
        SetCoreFaceType(coreTarget.right * -1, Face.FaceType.R, Color.magenta);
        SetCoreFaceType(coreTarget.forward, Face.FaceType.F, Color.blue);
        SetCoreFaceType(coreTarget.forward * -1, Face.FaceType.B, Color.cyan);
    }
    void SetCoreFaceType(Vector3 direction, Face.FaceType faceType, Color debugColor)
    {
        RaycastHit hit;
        Ray ray = new Ray(coreTarget.position, direction);
        Debug.DrawRay(coreTarget.position, direction, debugColor);
        Physics.Raycast(ray, out hit, 100f, LayerMask.GetMask("Center"));
        if (hit.collider)
        {
            var face = hit.collider.GetComponent<Face>();
            face.faceType = faceType;
        }
    }
    public void SetTouchOrientation(Vector3 touchPosition)
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(touchPosition), out hit, 100f, LayerMask.GetMask("Orientation")))
        {
            if(hit.collider == XAxisOrientationBox)
            {
                currentOrientation = Orientation.X;
            }
            else if (hit.collider == YAxisOrientationBox)
            {
                currentOrientation = Orientation.Y;
            }
            else if (hit.collider == ZAxisOrientationBox)
            {
                currentOrientation = Orientation.Z;
            }
        }
    }
    void SetFacePieces()
    {
        for (int i = 0; i < faceSearchers.Count; i++)
        {
            if (cube.faces[i].resolvedFaces) { continue; }

            List<Piece> face = new List<Piece>();
            foreach (var searchPosition in faceSearchers[i].raycastPositions)
            {
                RaycastHit hit;
                Physics.Raycast(searchPosition.position, searchPosition.forward, out hit, 100f, LayerMask.GetMask("Piece"));

                var piece = hit.collider.GetComponent<Piece>();
                face.Add(piece);
                
            }
            cube.faces[i].facePieces = new List<Piece>(face);
            cube.faces[i].resolvedFaces = true;
            face.Clear();
        }
    }
    public Orientation GetCurrentOrientation()
    {
        return currentOrientation;
    }
    private void OnDrawGizmos()
    {
        for (int i = 0; i < faceSearchers.Count; i++)
        {
            foreach (var searchPosition in faceSearchers[i].raycastPositions)
            {
                Gizmos.color = faceSearchers[i].debugRayColor;
                Gizmos.DrawRay(searchPosition.position, searchPosition.forward * 4f);
            }
        }
    }
}
