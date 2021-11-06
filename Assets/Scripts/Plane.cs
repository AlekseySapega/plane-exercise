using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class Plane : MonoBehaviour
{
    [SerializeField] private GameObject markView;
    
    public UnityEvent planeCloseToPointEvent;
    
    public static Plane Instance;
    private float currentAngle;
    private Transform container;
    private GameObject planeView;
    private Vector3 lastMarkPosition;
    private const int MARK_DISTANCE = 10000;
    
    public readonly Queue<Vector3> Destination = new Queue<Vector3>();
    private readonly Queue<GameObject> pathMark = new Queue<GameObject>();
    
    
    private bool isMoving;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        container = FindObjectOfType<Canvas>().transform;

    }

    public void MovePlaneToPoint()
    {
        if (isMoving == false)
        {
            isMoving = true;
            StartCoroutine(MoveToTarget(planeView.transform, Destination.Dequeue(), 0.0f));
        }
    }

    public void SetPlaneGameObject(GameObject plane)
    {
        this.planeView = plane;
    }
    
    private IEnumerator MoveToTarget(Transform obj, Vector3 target, float t)
    {
        var startPosition = obj.position;
        
        RotatePlane(startPosition, target, obj);

        while (t < 1)
        {
            t += FlightAnimation(startPosition, target, obj,t);
            yield return null;
        }

        isMoving = false;
        
        planeCloseToPointEvent.Invoke();
        
        if(IsQueueOfPointEmpty() == false)
            MovePlaneToPoint();
    }

    public bool IsQueueOfPointEmpty()
    {
        return Destination.Count == 0;
    }

    private void MakePathLine(Vector3 startPosition)
    {
        if (pathMark.Count == 0)
        {
            DrawPathLine(startPosition);
            lastMarkPosition = startPosition;
        }
        else if (IsNewMarkNeeded())
        {
            lastMarkPosition = planeView.transform.position;
            DrawPathLine(lastMarkPosition);
            
        }
    }

    private bool IsNewMarkNeeded()
    {
        return (lastMarkPosition - planeView.transform.position).sqrMagnitude > MARK_DISTANCE;
    }

    private void DrawPathLine(Vector3 coordinates)
    {
        var q = new Quaternion();
        if(pathMark.Count <= 20)
            pathMark.Enqueue(Instantiate(markView,coordinates,q,container));
        else
        {
            Destroy(pathMark.Dequeue());
            pathMark.Enqueue(Instantiate(markView,coordinates,q,container));
        }
    }

    private float CalculateTheAngle(Vector3 positionOfPlane, Vector3 positionOfPoint)
    {
        var angle = Math.Atan2(positionOfPlane.y - positionOfPoint.y,positionOfPlane.x - positionOfPoint.x);
        return (float)(angle * (180 / Math.PI) + 90);
    }

    private void RotatePlane(Vector3 startPosition, Vector3 targetPosition, Transform plane)
    {
        var rotation = CalculateTheAngle(startPosition,targetPosition);
        plane.Rotate(0.0f,0.0f,rotation - currentAngle);
        currentAngle = rotation;
    }

    private float FlightAnimation(Vector3 startPosition, Vector3 targetPosition, Transform plane, float t)
    {
        MakePathLine(startPosition);
        plane.position = Vector3.Lerp(startPosition, targetPosition, t * t * t);
        return Time.deltaTime / 1.5f;
    }
}

