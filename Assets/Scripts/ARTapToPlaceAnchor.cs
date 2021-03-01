using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Experimental.XR;
using UnityEngine.XR.ARSubsystems;
using System;


public class ARTapToPlaceAnchor : MonoBehaviour
{
    public GameObject anchor;
    public GameObject placementIndicator;

    private ARSessionOrigin arOrigin;
    private ARRaycastManager rayCastMgr;
    private Pose placementPose;
    private bool placementPoseIsValid = false;
    private bool anchorPlaced = false;

    void Start()
    {
        if (Application.platform == RuntimePlatform.Android) {
            rayCastMgr = FindObjectOfType<ARRaycastManager>();
        } else {
            placementPose = new Pose(new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
            PlaceObject();
        }
    }

    void Update()
    {
        if (!anchorPlaced)
        {
            UpdatePlacementPose();
            UpdatePlacementIndicator();

            if (placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                PlaceObject();
            }
        }
    }

    private void PlaceObject()
    {
        Instantiate(anchor, placementPose.position, placementPose.rotation);
        anchorPlaced = true;
    }

    private void UpdatePlacementIndicator()
    {
        if (placementPoseIsValid)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }

    private void UpdatePlacementPose()
    {
        var screenCenter = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        rayCastMgr.Raycast(screenCenter, hits, TrackableType.Planes);

        placementPoseIsValid = hits.Count > 0;
        if (placementPoseIsValid)
        {
            placementPose = hits[0].pose;
        }
    }
}
