
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MicroGestureTest : MonoBehaviour
{
    [SerializeField]
    private OVRMicrogestureEventSource leftGestureSource;

    [SerializeField]
    private OVRMicrogestureEventSource rightGestureSource;

    private bool isToggleLineRenderer = false;

    [SerializeField]
    private GameObject teleportationInteractor;

    [SerializeField]
    private GettingAndUpdatingMRObject gettingAndUpdatingMRObject;

    [Header("Gesture Labels")]
    [SerializeField]
    private Text leftGestureLabel;

    [SerializeField]
    private Text rightGestureLabel;

    [SerializeField]
    private float gestureShowDuration = 1.5f;

    [Header("Navigation Icons Left")]
    [SerializeField]
    private Image leftArrowL;

    [SerializeField]
    private Image rightArrowL;

    [SerializeField]
    private Image upArrowL;

    [SerializeField]
    private Image downArrowL;

    [SerializeField]
    private Image selectIconL;

    [Header("Navigation Icons Right")]
    [SerializeField]
    private Image leftArrowR;

    [SerializeField]
    private Image rightArrowR;

    [SerializeField]
    private Image upArrowR;

    [SerializeField]
    private Image downArrowR;

    [SerializeField]
    private Image selectIconR;

    [Header("Colors")]
    [SerializeField]
    private Color initialColor = Color.white;

    [SerializeField]
    private Color highlightColor = Color.blue;

    [SerializeField]
    private float highlightDuration = 1f;

    [SerializeField]
    private GameObject cubeInstantiatePosition;

    [SerializeField]
    private GameObject cube;

    private GameObject spawnedCube;

    [SerializeField]
    private float rotationStep = 45f;

    [SerializeField]
    private float rotationDuration = 0.25f;

    private Coroutine rotateCoroutine;


    void Start()
    {
        leftGestureSource.GestureRecognizedEvent.AddListener(gesture => OnGestureRecognized(OVRPlugin.Hand.HandLeft, gesture));
        rightGestureSource.GestureRecognizedEvent.AddListener(gesture => OnGestureRecognized(OVRPlugin.Hand.HandRight, gesture));
    }

    private void RotateCube(float angle)
    {
        if (rotateCoroutine != null)
            StopCoroutine(rotateCoroutine);

        rotateCoroutine = StartCoroutine(RotateCubeCoroutine(angle));
    }

    private IEnumerator RotateCubeCoroutine(float angle)
    {
        Quaternion startRotation = cube.transform.rotation;
        Quaternion targetRotation = startRotation * Quaternion.Euler(0f, angle, 0f);

        float elapsed = 0f;

        while (elapsed < rotationDuration)
        {
            float t = elapsed / rotationDuration;
            cube.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        cube.transform.rotation = targetRotation;
        rotateCoroutine = null;
    }


    private void HighlightGesture(OVRPlugin.Hand hand, OVRHand.MicrogestureType gesture)
    {
        switch (gesture)
        {
            case OVRHand.MicrogestureType.SwipeLeft:
                Debug.Log("MicroGestureTest : SWIPE LEFT");
                gettingAndUpdatingMRObject.ChangeAsset(-1);
                break;
            case OVRHand.MicrogestureType.SwipeRight:
                Debug.Log("MicroGestureTest : SWIPE RIGHT");
                gettingAndUpdatingMRObject.ChangeAsset(1);
                break;
            case OVRHand.MicrogestureType.SwipeForward:
                break;
            case OVRHand.MicrogestureType.SwipeBackward:
                break;
            case OVRHand.MicrogestureType.ThumbTap:
                Debug.Log("MicroGestureTest : THUMB TAP");
                isToggleLineRenderer = !isToggleLineRenderer;
                gettingAndUpdatingMRObject.ToggleLineRenderer(isToggleLineRenderer);
                break;
        }
    }

    



    private void OnGestureRecognized(OVRPlugin.Hand hand, OVRHand.MicrogestureType gesture)
    {
        HighlightGesture(hand, gesture);
    }

}
