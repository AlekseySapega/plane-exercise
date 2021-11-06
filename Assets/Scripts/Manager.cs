using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class Manager : MonoBehaviour
{
    [SerializeField] private GameObject planeView;
    [SerializeField] private GameObject pointView;

    public UnityEvent sendFirstPointOfQueue;

    private readonly Queue<GameObject> pointsViewQueue = new Queue<GameObject>();
    private readonly Quaternion quaternion = new Quaternion();

    private void Start()
    {
        Plane.Instance.SetPlaneGameObject(planeView);
    }

    public void EventHandlerOfNewPointAppearance()
    {
        SendPointToPlane(Sky.Instance.GetPointCoordinates());
        ShowObjectInGame(Sky.Instance.GetPointCoordinates());
    }

    public void DestroyPoint()
    {
       Destroy(pointsViewQueue.Dequeue(), 0.0f);
    }

    private void SendPointToPlane(Vector3 point)
    {
        if (Plane.Instance.IsQueueOfPointEmpty())
        {
            Plane.Instance.Destination.Enqueue(point);
            sendFirstPointOfQueue.Invoke();
        }
        else
        {
            Plane.Instance.Destination.Enqueue(point);
        }
    }

    private void ShowObjectInGame(Vector3 coordinates)
    {
        var container = FindObjectOfType<Canvas>().transform;
        pointView.transform.position = coordinates;
        pointsViewQueue.Enqueue(Instantiate(pointView,pointView.transform.position, quaternion, container));
    }
    
    



}