using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TargetController : MonoBehaviour
{
    private MeshRenderer renderer;
    private Collider thisCollider;
    private Camera CamReference;

    private float Offset = 10;

    private Vector3 StartingPos;
    void Start()
    {
        renderer= GetComponent<MeshRenderer>();
        thisCollider= GetComponent<Collider>();

        CamReference = Camera.main;

        StartingPos = transform.position;
    }

    void Update()
    {
        HandleInput();   
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            renderer.enabled = !renderer.enabled;
            thisCollider.enabled= !thisCollider.enabled;
        }

        if (renderer.enabled)
        {
            PositionUpdate();
        }
    }

    private void PositionUpdate()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.x = Mathf.Clamp(mousePos.x, StartingPos.x -Offset, StartingPos.x +Offset);
        mousePos.y = Mathf.Clamp(mousePos.y, StartingPos.y -Offset, StartingPos.y +Offset);
        mousePos.z = Mathf.Clamp(mousePos.z, 0, float.PositiveInfinity);
        Vector3 newPoint = CamReference.ScreenToWorldPoint(mousePos);
        transform.position = newPoint;
    }
}
