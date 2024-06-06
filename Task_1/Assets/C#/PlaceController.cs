using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class PlaceController : MonoBehaviour
{
    [SerializeField] private GameObject objectToPlace;
    [SerializeField] private GameObject placedObject;
    public Transform folder;
    public GameObject buttons;

    [SerializeField] private ARRaycastManager _arRaycastManager;
    private Camera arCamera;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private void Awake() 
    {
        _arRaycastManager = GetComponent<ARRaycastManager>();
        arCamera = GameObject.Find("AR Camera").GetComponent<Camera>();
    }

    private void Update()
    {
        if (Input.touchCount == 0)
            return;

        RaycastHit hit;
        Ray ray = arCamera.ScreenPointToRay(Input.GetTouch(0).position);

        if(_arRaycastManager.Raycast(Input.GetTouch(0).position, hits))
        {
            if(Input.GetTouch(0).phase == TouchPhase.Began && placedObject == null && Input.GetTouch(0).position.y > 704)
            {
                if(Physics.Raycast(ray, out hit))
                {
                    if(hit.collider.tag == "Player")
                    {
                        placedObject = hit.collider.gameObject;
                    }
                    else
                    {
                        var hitPose = hits[0].pose;
                        var spawnedobject = Instantiate(objectToPlace, hitPose.position, Quaternion.identity);
                        spawnedobject.transform.SetParent(folder);
                    }
                }
            }
            if(Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                placedObject = null;
            }
        }

        buttons.SetActive(folder.childCount > 0);
    }
}