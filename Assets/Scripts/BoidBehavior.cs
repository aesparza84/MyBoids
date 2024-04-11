using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BoidBehavior : MonoBehaviour
{
    //Direction Vector
    private Vector3 Direction;

    //Velocity vector
    public Vector3 Velocity;

    //The target positon for ALL boids
    private Transform TargetObject;

    //Flight speed of the boid
    [SerializeField] private float Speed;

    private const float LocalRadius = 10.0f;
    private const float MinClumpDistance = 4.0f;

    //Buffer for checking neighboring Boids
    [SerializeField] private int MaxNeighbors;
    private Collider[] Neighboids;
    private Collider myCollider;

    private const int BitMask = (1 << 0);

    //Toggle for boids to follow/NOT Follow 'center' point
    public bool FreeFly;

    //Rules 
    public bool Alignment;
    public bool Seperation;
    public bool Cohesion;

    private int DebugID;

    [SerializeField] private bool SelfUpdate;

    [SerializeField] private GameObject DebugOj;
    private void Start()
    {
        //Setting the boid buffer
        Neighboids = new Collider[MaxNeighbors];

        //Find Target position
        TargetObject = GameObject.Find("Target").transform;

        myCollider = GetComponent<Collider>();

        DebugID = UnityEngine.Random.Range(0, 100);
    }

    private void Update()
    {
        if (SelfUpdate)
        {
            CalculateMovement();
        }
    }
    public void CalculateMovement()
    {
        //See nearby Boids
        //Set direction to avoid neighboids
        Direction = GetValidDirection();

        Quaternion newLook = Quaternion.LookRotation(Direction, transform.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, newLook, Time.deltaTime * 100.0f);

        //Velocity = Direction.normalized * Speed * Time.deltaTime;

        //Update transform position

        Debug.Log(Direction);
        transform.position += transform.forward * Speed * Time.deltaTime;
    }

    private Vector3 GetValidDirection()
    {
        Vector3 AvgHeading = Vector3.zero;
        Vector3 AvgCenterPosition = Vector3.zero;
        Vector3 SpacingVector = Vector3.zero;
        Vector3 finalDirection = transform.forward;

        //ADD the desired 'center direction' to the direction
        if (!FreeFly && TargetObject != null)
        {
            AvgHeading += (transform.position - TargetObject.position);
        }

        //Check for nearby boids
        //
        //Get average of all nearby boids Vector-HEADINGS
        int neighboids = Physics.OverlapSphereNonAlloc(transform.position, LocalRadius, Neighboids);
        
        for (int i = 0; i < neighboids; i++)
        {
            if (Neighboids[i] != myCollider)
            {
                Debug.DrawRay(transform.position, (Neighboids[i].transform.position - transform.position), Color.green);


                //Boids will try and aim towards center of neighbor cluster
                if (Cohesion)
                {
                    //Totals the positions of neighbors
                    AvgCenterPosition += Neighboids[i].transform.position;
                }

                //Boids align with neighboring headings
                if (Alignment)
                {
                    AvgHeading += Neighboids[i].transform.forward;
                }

                //Boids will not collide with eachother--DONE
                if (Seperation)
                {
                    float dist = Vector3.Distance(transform.position, Neighboids[i].transform.position);

                    if (dist < MinClumpDistance)
                    {
                        //Direction TO index boid
                        SpacingVector += (transform.position - Neighboids[i].transform.position);
                    }
                }
            }
            /*
            if (Neighboids[i] != this)
            {
                //Avoids boids that are in front of boid
                if (Vector3.Dot(transform.forward, Neighboids[i].transform.position) > 0.0f)
                {
                    newDirection += (transform.position - Neighboids[i].transform.position);
                }
            } 
            */
        }

        //Averages position to get direction to 'center' of cluster
        if (Cohesion)
        {
            if (AvgCenterPosition != Vector3.zero)
            {
                AvgCenterPosition /= neighboids;

                Vector3 DirToAvg = AvgCenterPosition-transform.position;
                finalDirection += DirToAvg;
                Debug.DrawRay(transform.position, DirToAvg, Color.magenta);
            }

            if (DebugOj != null)
            {
                DebugOj.transform.position = transform.position + AvgCenterPosition;
            }
        }

        //Average Direction of surrounding neighbors. HEADING
        if (Alignment)
        {
            if (AvgHeading != Vector3.zero)
            {
                AvgHeading /= neighboids;
            }
            else
            {
                float ex = UnityEngine.Random.Range(-360, 360);
                float ey = UnityEngine.Random.Range(-360, 360);
                float ez = UnityEngine.Random.Range(-360, 360);
                AvgHeading = new Vector3(ex, ey, ez);
            }
        }

        //Averages the seperation vector
        if (Seperation)
        {
            //Average of all directions from Boid to neighbor
            if (SpacingVector != Vector3.zero)
            {
                SpacingVector /= neighboids;

                finalDirection += SpacingVector;
            }

            Debug.DrawRay(transform.position, SpacingVector * 1.5f, Color.cyan);
        }

        //Vector3 finalDirection = (transform.position - AvgCenterPosition) + AvgHeading + SpacingVector; 
        //finalDirection += SpacingVector + AvgCenterPosition;

        Debug.DrawRay(transform.position, finalDirection.normalized, Color.red);
        
        return finalDirection.normalized;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, LocalRadius);

        Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(transform.position, MinClumpDistance);
    }
}
