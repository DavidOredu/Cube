using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SwipeType
{
    LeftSwipe,
    RightSwipe,
    UpSwipe,
    DownSwipe,
    UpLeftSwipe,
    UpRightSwipe,
    DownLeftSwipe,
    DownRightSwipe
}
public class CubeController : MonoBehaviour
{
    private Cube cube;
    private CubeOrientation cubeOrientation;

    private Vector3 faceSwipeStart;
    private Vector3 faceSwipeEnd;

    private Vector3 currentFaceSwipe;
    private SwipeType currentSwipeType;

    private Vector3 deltaMousePosition;
    private Vector3 previousMousePosition;

    private RaycastHit hit;
    private Face currentFace;

    private bool isSwiping;
    private bool isRotating;

    [SerializeField]
    private Transform targetRotation;
    [SerializeField]
    private float rotationSpeed = .3f;
    // Start is called before the first frame update
    void Start()
    {
        cube = GetComponent<Cube>();
        cubeOrientation = GetComponent<CubeOrientation>();
    }

    // Update is called once per frame
    void Update()
    {
        GetFaceSwipe();
        UpdateCubeRotation();

    }
    private void FixedUpdate()
    {
    }
    private void GetFaceSwipe()
    {
        if (Input.GetMouseButtonDown(0))
        {
            faceSwipeStart = new Vector3(Input.mousePosition.x, Input.mousePosition.y);
            cubeOrientation.SetTouchOrientation(faceSwipeStart);
            isSwiping = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            faceSwipeEnd = new Vector3(Input.mousePosition.x, Input.mousePosition.y);
            currentFaceSwipe = new Vector3(faceSwipeEnd.x - faceSwipeStart.x, faceSwipeEnd.y - faceSwipeStart.y);
            currentFaceSwipe.Normalize();

            var faces = GetTouchedFaces();
            DetermineFaceSwipeType();
            HandleCubeRotation();
            HandleFaceMovement(faces);
        }
    }
    void DetermineFaceSwipeType()
    {
        if(currentFaceSwipe.x > 0 && currentFaceSwipe.y > -0.5f && currentFaceSwipe.y < 0.5f)
        {
            currentSwipeType = SwipeType.RightSwipe;
        }
        else if(currentFaceSwipe.x < 0 && currentFaceSwipe.y > -0.5f && currentFaceSwipe.y < 0.5f)
        {
            currentSwipeType = SwipeType.LeftSwipe;
        }

        else if(currentFaceSwipe.y > 0 && currentFaceSwipe.x > -0.5f && currentFaceSwipe.x < 0.5f)
        {
            currentSwipeType = SwipeType.UpSwipe;
        }
        else if(currentFaceSwipe.y < 0 && currentFaceSwipe.x > -0.5f && currentFaceSwipe.x < 0.5f)
        {
            currentSwipeType = SwipeType.DownSwipe;
        }
        else if (currentFaceSwipe.y > 0 && currentFaceSwipe.x < 0)
        {
            currentSwipeType = SwipeType.UpLeftSwipe;
        }
        else if (currentFaceSwipe.y > 0 && currentFaceSwipe.x > 0)
        {
            currentSwipeType = SwipeType.UpRightSwipe;
        }
        else if (currentFaceSwipe.y < 0 && currentFaceSwipe.x < 0)
        {
            currentSwipeType = SwipeType.DownLeftSwipe;
        }
        else if (currentFaceSwipe.y < 0 && currentFaceSwipe.x > 0)
        {
            currentSwipeType = SwipeType.DownRightSwipe;
        }
    }
    private void HandleFaceMovement(List<Face> faces)
    {
        for (int i = 0; i < faces.Count; i++)
        {
            GetFaceWithSwipe(faces[i], faces[i].faceType, currentSwipeType);
        }

        
    }
    private List<Face> GetTouchedFaces()
    {
        var hits = Physics.RaycastAll(Camera.main.ScreenPointToRay(faceSwipeStart), 1000f, LayerMask.GetMask("Center"));
        List<Face> detectedFaces = new List<Face>();
        if(hits.Length > 0)
        {
            for (int i = 0; i < hits.Length; i++)
            {
                detectedFaces.Add(hits[i].collider.GetComponent<Face>());
            }
        }
        Debug.Log("Number of faces: " + detectedFaces.Count);
        return detectedFaces;
    }

    private void HandleCubeRotation()
    {
        // target an empty section on screen
        Physics.Raycast(Camera.main.ScreenPointToRay(faceSwipeStart), out hit, 100f);

        // if we hit something, get out of the function
        if (hit.collider) { return; }
        
        var core = cube.GetPiece(Piece.PieceType.Core);
        var corePosition = Camera.main.WorldToScreenPoint(core.transform.position);

        switch (currentSwipeType)
        {
            // for x-axis rotation
            case SwipeType.LeftSwipe:
                targetRotation.transform.Rotate(0f, 90f, 0f, Space.World);
                break;
            case SwipeType.RightSwipe:
                targetRotation.transform.Rotate(0f, -90f,0f, Space.World);
                break;
            case SwipeType.UpSwipe:
                if(faceSwipeStart.x < corePosition.x)
                {
                    targetRotation.transform.Rotate(90f, 0f, 0f, Space.World);
                }
                else if(faceSwipeStart.x > corePosition.x)
                {
                   targetRotation.transform.Rotate(0f, 0f, 90f, Space.World);
                }
                break;
            case SwipeType.DownSwipe:
                if (faceSwipeStart.x < corePosition.x)
                {
                    targetRotation.transform.Rotate(-90f, 0f, 0f, Space.World);
                }
                else if (faceSwipeStart.x > corePosition.x)
                {
                   targetRotation.transform.Rotate(0f, 0f, -90f, Space.World);
                }
                break;
            default:
                break;
        }
    }
    void UpdateCubeRotation()
    {
        if(transform.rotation != targetRotation.rotation)
        {
            var step = rotationSpeed * Time.deltaTime;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation.rotation, step);
            isRotating = true;
        }
        else if(isRotating)
        {
            cubeOrientation.AdjustCores();
            isRotating = false;
        }
    }
    private void GetFaceWithSwipe(Face face, Face.FaceType faceType, SwipeType swipeType)
    {
        deltaMousePosition = Input.mousePosition - previousMousePosition;
        Debug.Log("Delta Mouse Position: " + deltaMousePosition);

        switch (faceType)
        {
            case Face.FaceType.F:
                if (cubeOrientation.GetCurrentOrientation() == Orientation.Z)
                {
                    if(Mathf.Abs(deltaMousePosition.y) > Mathf.Abs(deltaMousePosition.x))
                    {
                        // we have y axis movement, we can move face up or down
                        int direction;
                        if (deltaMousePosition.y < 0)
                            direction = -1;
                        else
                            direction = 1;

                        Debug.Log("Direction of Move: " + direction);
                        currentFace = face;
                        Debug.Log("Current Face: " + currentFace);
                        cube.MoveFace(currentFace, direction);
                    }
                }
                else if(cubeOrientation.GetCurrentOrientation() == Orientation.Y)
                {
                   if(Mathf.Abs(deltaMousePosition.x) > Mathf.Abs(deltaMousePosition.y))
                   {

                   }
                }
                break;
            case Face.FaceType.B:
                break;
            case Face.FaceType.U:
                break;
            case Face.FaceType.D:
                break;
            case Face.FaceType.L:
                break;
            case Face.FaceType.R:
                break;
            case Face.FaceType.M:
                break;
            case Face.FaceType.E:
                break;
            case Face.FaceType.S:
                break;
            default:
                break;
        }
        previousMousePosition = Input.mousePosition;
    }
}
