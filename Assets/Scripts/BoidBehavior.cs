using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidBehavior : MonoBehaviour
{
    //Direction Vector
    private Vector3 Direction;

    //Velocity vector
    private Vector3 Velocity;

    //The target positon for ALL boids
    private Transform TargetObject;

    //Flight speed of the boid
    [SerializeField] private float Speed;

    private const float RotationSmoothing = 1.0f;
    private const float DetectingRadius = 2.5f;

    //Buffer for checking neighboring Boids
    [SerializeField] private int MaxNeighbors;
    private Collider[] Neighboids;

    //Toggle for boids to follow/NOT Follow 'center' point
    public bool FreeFly;
    private void Start()
    {
        //Setting the boid buffer
        Neighboids = new Collider[MaxNeighbors];

        //Find Target position
        TargetObject = GameObject.Find("Target").transform;
    }

    public void CalculateMovement()
    {
        //See nearby Boids
        //Set direction to avoid neighboids
        Direction = GetValidDirection();

        transform.forward = Direction;

        //Update transform position
        transform.position += Direction.normalized * Speed * Time.deltaTime;
    }

    private Vector3 GetValidDirection()
    {
        Vector3 newDirection = Vector3.zero;

        //Check for nearby boids
        //Get average of all nearby boids Vector
        int neighboids = Physics.OverlapSphereNonAlloc(transform.position, DetectingRadius, Neighboids);
        
        for (int i = 0; i < neighboids; i++)
        {
            newDirection += (transform.position - Neighboids[i].transform.position);
        }

        //ADD the desired 'center direction' to the direction
        if (!FreeFly && TargetObject != null)
        {
            newDirection += (transform.position - TargetObject.position);
        }

        //Average Direction of surrounding neighbors 
        newDirection /= neighboids;
        

        return newDirection.normalized;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, DetectingRadius);
    }
}
