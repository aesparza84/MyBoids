using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UIElements;

public class BoidBehavior : MonoBehaviour
{
    //Direction Vector
    [SerializeField]private Vector3 Direction;

    //The target positon for ALL boids
    public GameObject TargetObject;

    //Flight speed of the boid
    public float Speed;

    private const float LocalRadius = 10.0f;
    private const float MinClumpDistance = 4.0f;

    public float HeadingInfluence = 0.3f;

    //Buffer for checking neighboring Boids
    [SerializeField] private int MaxNeighbors;
    private Collider[] Neighboids;
    private Collider myCollider;

    private const int BitMask = ( (1 << 0) | (1<<6) );

    //Toggle for boids to follow/NOT Follow 'center' point
    public bool FreeFly;

    //Rules 
    public bool Alignment;
    public bool Seperation;
    public bool Cohesion;

    [Range(0, 1)]
    [SerializeField] private float AvoidanceStrength = 1;
    [Range(0, 1)]
    [SerializeField] private float CohesionStrength = 1;
    [Range(0, 1)]
    [SerializeField] private float AlignmentStrength = 1;

    private int DebugID;

    [SerializeField] private bool SelfUpdate;

    [SerializeField] private GameObject DebugOj;
    private void Start()
    {
        //Setting the boid buffer
        Neighboids = new Collider[MaxNeighbors];

        //Find Target position
        TargetObject = GameObject.Find("Target");

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
            //Direction = OldGetValidDirection();
        Direction = GetValidDirection();
        if (TargetObject != null && !FreeFly)
        {
            Vector3 DirToObj = TargetObject.transform.position - transform.position;
            Direction += DirToObj.normalized;
        }

        Quaternion newLook = Quaternion.LookRotation(Direction, transform.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, newLook, Time.deltaTime * 350.0f);

        //Update transform position
        transform.position += transform.forward * Speed * Time.deltaTime;
    }

    private Vector3 OldGetValidDirection()
    {
        Vector3 AvgHeading = transform.forward;
        Vector3 AvgCenterPosition = transform.position;
        Vector3 SpacingVector = Vector3.zero;
        Vector3 finalDirection = transform.forward;

        //ADD the desired 'center direction' to the direction
        if (!FreeFly && TargetObject != null)
        {
            AvgHeading += (transform.position - TargetObject.transform.position);
        }

        //Check for nearby boids
        //
        //Get average of all nearby boids Vector-HEADINGS
        int neighboids = Physics.OverlapSphereNonAlloc(transform.position, LocalRadius, Neighboids);

        for (int i = 0; i < neighboids; i++)
        {
            if (Neighboids[i] != myCollider && !Neighboids[i].gameObject.CompareTag("Obstacle"))
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
                    Vector3 hit = Neighboids[i].ClosestPointOnBounds(transform.position);
                    float dist = Vector3.Distance(transform.position, hit);

                    if (dist < MinClumpDistance)
                    {
                        //Direction TO index boid
                        SpacingVector += (transform.position - Neighboids[i].transform.position);
                        Debug.DrawRay(transform.position, SpacingVector, Color.cyan);
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

                Vector3 DirToAvg = AvgCenterPosition - transform.position;
                finalDirection += (DirToAvg.normalized * CohesionStrength);
                Debug.DrawRay(transform.position, DirToAvg, Color.magenta);
            }

            if (DebugOj != null)
            {
                DebugOj.transform.position = transform.position + AvgCenterPosition;
            }
        }

        //Average Direction of surrounding neighbors. HEADING
        if (Alignment && neighboids > 1)
        {

            /*
            if (AvgHeading != transform.forward)
            {
                AvgHeading /= neighboids;
                
            }
            else
            {
                float ex = UnityEngine.Random.Range(-360, 360);
                float ey = UnityEngine.Random.Range(-360, 360);
                float ez = UnityEngine.Random.Range(-360, 360);
                AvgHeading = new Vector3(ex, ey, ez);
            }*/
            AvgHeading /= neighboids;

            Vector3 dirTo = AvgHeading - transform.position;
            finalDirection += (dirTo * HeadingInfluence).normalized;

            Debug.DrawRay(transform.position, dirTo, Color.magenta);
        }

        //Averages the seperation vector
        if (Seperation)
        {
            //Average of all directions from Boid to neighbor
            if (SpacingVector != Vector3.zero)
            {
                SpacingVector /= neighboids;

                finalDirection += (SpacingVector.normalized * AvoidanceStrength);
            }

            //Debug.DrawRay(transform.position, SpacingVector * 1.5f, Color.cyan);
        }

        //Vector3 finalDirection = (transform.position - AvgCenterPosition) + AvgHeading + SpacingVector; 
        //finalDirection += SpacingVector + AvgCenterPosition;

        Debug.DrawRay(transform.position, finalDirection.normalized * 3, Color.red);

        return finalDirection.normalized;
    }

    private Vector3 GetValidDirection()
    {
        //Avg Heading of neighbors
            //Vector3 AvgHeading = transform.forward;
        Vector3 AvgHeading = Vector3.zero;

        //Center of all boids relative to THIS
            //Vector3 AvgCenterPosition = Vector3.zero;
        Vector3 AvgCenterPosition = Vector3.zero;

        //Away vector (Sums all away vectors)
        Vector3 SpacingVector = Vector3.zero;
        
        //Final direction that we will add to
        Vector3 finalDir = Vector3.zero;

        int neighboids = Physics.OverlapSphereNonAlloc(transform.position, LocalRadius, Neighboids);
        int trueBoids = 0;
        int CollisionCount = 0;

        //ALL objects withint the local radius
        for (int i = 0; i < neighboids; i++)
        {
            //If the index is Exclusively another BOID
            if (Neighboids[i] != myCollider && !Neighboids[i].gameObject.CompareTag("Obstacle"))
            {
                //Debug.DrawRay(transform.position, (Neighboids[i].transform.position - transform.position), Color.green);

                //Add neighbors Forward
                AvgHeading += Neighboids[i].transform.forward;

                //Add the neighbors Position [Center calcs.]
                AvgCenterPosition += Neighboids[i].transform.position;

                trueBoids++;
            }
            
            //If the index is anything other than THIS
            if (Neighboids[i] != myCollider || Neighboids[i].gameObject.CompareTag("Obstacle"))
            {
                //Add the neighbors Position [Seperation calcs.]
                Vector3 hit = Neighboids[i].ClosestPointOnBounds(transform.position);
                Vector3 DirToHit = hit - transform.position;
                float dist = Vector3.Distance(transform.position, hit);

                    //Debug.DrawRay(transform.position, DirToHit, Color.red);

                if (dist < MinClumpDistance && Vector3.Dot(transform.forward, DirToHit) > -0.4f)
                {

                    //Direction TO index Object
                    SpacingVector += (transform.position - hit).normalized;

                    CollisionCount++;
                }
            }
        }

        Debug.Log("True Boids "+trueBoids);

        //Aligns towrad average Heading of neighbors
        if (Alignment)
        {
            Vector3 avg = AlignmentVector(trueBoids, AvgHeading);
            finalDir += avg * AlignmentStrength;
        }

        //Moves toward center of neighbors
        if (Cohesion)
        {
            Vector3 coh = CohesionVector(trueBoids, AvgCenterPosition);
            finalDir += (transform.forward+coh).normalized * CohesionStrength;
        }
        else
        {
            //Will add THIS boids forward if no Cohesion
            finalDir += transform.forward;
        }

        //Calculates based on ALL collsions detected
        if (Seperation)
        {
            Vector3 sepDir = SeperationVector(CollisionCount,SpacingVector);
            finalDir += sepDir * AvoidanceStrength;
            Debug.DrawRay(transform.position, sepDir.normalized * AvoidanceStrength, Color.cyan);
        }

        return finalDir.normalized;
    }

    
    private Vector3 AlignmentVector(int totalBoids, Vector3 AvgHeading)
    {
        Vector3 alignment = Vector3.zero;

        if (totalBoids > 0)
        {
            alignment = AvgHeading / (totalBoids + 1); //+1 = this instance
        }

        return alignment.normalized;
    }
    private Vector3 CohesionVector(int totalBoids, Vector3 AvgCenterPosition)
    {
        //Averages position to get direction to 'center' of cluster
        Vector3 DirToAvg = Vector3.zero;

        if (totalBoids > 0)
        {
            AvgCenterPosition /= totalBoids;

            DirToAvg = AvgCenterPosition - transform.position;
        }
        else
        {
            //Generate a random direction
            float x = UnityEngine.Random.Range(-360, 360);
            float y = UnityEngine.Random.Range(-360, 360);
            float z = UnityEngine.Random.Range(-360, 360);
            DirToAvg = new Vector3(x, y, z);
        }

        Debug.DrawRay(transform.position, DirToAvg.normalized, Color.magenta);

        return DirToAvg.normalized;
    }
    
    private Vector3 SeperationVector(int collisionCount, Vector3 SpacingVector) 
    {
        Vector3 SepDir = Vector3.zero;
        SepDir = (SpacingVector / collisionCount);

        return SepDir.normalized;
    }
   
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, LocalRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, MinClumpDistance);
    }
}
