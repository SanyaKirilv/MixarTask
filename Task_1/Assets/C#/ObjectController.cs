using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectController : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public static Action<Vector3> OnSpawned;
    [SerializeField] private Color enable;
    [SerializeField] private Color disable;
    [SerializeField] private int secondsToDestroy;
    private bool state;
    public DateTime pointerDown;
    public DateTime pointerUp;

    private void Start() => OnSpawned?.Invoke(gameObject.GetComponent<Transform>().position);

    public void OnPointerDown(PointerEventData eventData)
    {
        state = !state;
        GetComponent<MeshRenderer>().material.color = state ? enable : disable;
        pointerDown = DateTime.Now;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        pointerUp = DateTime.Now;
        print((pointerDown - pointerUp).TotalSeconds);
        if (Math.Abs((pointerDown - pointerUp).TotalSeconds) > secondsToDestroy)
            Destroy(gameObject);
    }
}
