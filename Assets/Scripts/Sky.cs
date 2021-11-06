using UnityEngine;
using UnityEngine.Events;



public class Sky : MonoBehaviour
{
    public static Sky Instance;

    private Vector3 pointCoordinates;

    public UnityEvent newPointAppearanceEvent;

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
    }

    private void OnEnable()
    {
        Lean.Touch.LeanTouch.OnFingerTap += HandleFingerTap;
    }

    private void OnDisable()
    {
        Lean.Touch.LeanTouch.OnFingerTap -= HandleFingerTap;
    }

    private void HandleFingerTap(Lean.Touch.LeanFinger finger)
    {
        pointCoordinates = finger.ScreenPosition;
        newPointAppearanceEvent.Invoke();
    }

    public Vector3 GetPointCoordinates()
    {
        return pointCoordinates;
    }

}



