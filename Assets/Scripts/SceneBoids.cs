using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneBoids : MonoBehaviour
{
    [SerializeField] private bool SpawnOnKey = false;
    [SerializeField] private GameObject BoidPrefab;
    [SerializeField] private int BoidAmount;

    private List<BoidBehavior> AllBoids;

    private const float SpawnOffset = 4.0f;

    [SerializeField] private bool FreeFly;
    [SerializeField] private bool Cohesion;
    [SerializeField] private bool Alignment;
    [SerializeField] private bool Seperation;
    [SerializeField] private float HeadingInfluence;
    [SerializeField] private float Speed;

    [SerializeField] private GameObject Target;
    private void Awake()
    {
        AllBoids = new List<BoidBehavior>();

        if (!SpawnOnKey)
        {
            for (int i = 0; i < BoidAmount; i++)
            {
                float x = UnityEngine.Random.Range(-SpawnOffset, SpawnOffset);
                float y = UnityEngine.Random.Range(-SpawnOffset, SpawnOffset);
                float z = UnityEngine.Random.Range(-SpawnOffset, SpawnOffset);
                Vector3 spawn = new Vector3(x, y, z);

                float ex = UnityEngine.Random.Range(-360, 360);
                float ey = UnityEngine.Random.Range(-360, 360);
                float ez = UnityEngine.Random.Range(-360, 360);

                Vector3 rand = new Vector3(ex, ey, ez);
                Quaternion spawnRot = Quaternion.Euler(rand);

                GameObject m = Instantiate(BoidPrefab, transform.position + spawn, spawnRot);
                BoidBehavior mm = m.GetComponent<BoidBehavior>();
                AllBoids.Add(mm);
            }
        }        
    }

    private void Update()
    {
        UpdateBoids();
    }

    private void UpdateBoids()
    {
        if (AllBoids.Count > 0)
        {
            foreach (BoidBehavior boid in AllBoids)
            {
                boid.FreeFly = FreeFly;
                boid.Alignment = Alignment;
                boid.Cohesion = Cohesion;
                boid.Seperation = Seperation;
                boid.Speed = Speed;
                boid.HeadingInfluence = HeadingInfluence;
                boid.CalculateMovement();
            }
        }
        

        HandleInput();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (!SpawnOnKey)
            {
                ResetAllBoids();
            }
            else
            {
                float x = UnityEngine.Random.Range(-SpawnOffset, SpawnOffset);
                float y = UnityEngine.Random.Range(-SpawnOffset, SpawnOffset);
                float z = UnityEngine.Random.Range(-SpawnOffset, SpawnOffset);
                Vector3 spawn = new Vector3(x, y, z);

                float ex = UnityEngine.Random.Range(-360, 360);
                float ey = UnityEngine.Random.Range(-360, 360);
                float ez = UnityEngine.Random.Range(-360, 360);

                Vector3 rand = new Vector3(ex, ey, ez);
                Quaternion spawnRot = Quaternion.Euler(rand);

                GameObject m = Instantiate(BoidPrefab, transform.position + spawn, spawnRot);
                BoidBehavior mm = m.GetComponent<BoidBehavior>();
                if (Target != null)
                {
                    mm.TargetObject = Target;
                }
                AllBoids.Add(mm);
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (SpawnOnKey)
            {
                foreach (BoidBehavior item in AllBoids)
                {
                    Destroy(item.gameObject);
                }
                AllBoids.Clear();
            }
        }
    }

    private void ResetAllBoids()
    {
        foreach (BoidBehavior item in AllBoids)
        {
            Destroy(item.gameObject);
        }

        AllBoids.Clear();

        for (int i = 0; i < BoidAmount; i++)
        {
            float x = UnityEngine.Random.Range(-SpawnOffset, SpawnOffset);
            float y = UnityEngine.Random.Range(-SpawnOffset, SpawnOffset);
            float z = UnityEngine.Random.Range(-SpawnOffset, SpawnOffset);
            Vector3 spawn = new Vector3(x, y, z);

            float ex = UnityEngine.Random.Range(-360, 360);
            float ey = UnityEngine.Random.Range(-360, 360);
            float ez = UnityEngine.Random.Range(-360, 360);

            Vector3 rand = new Vector3(ex, ey, ez);
            Quaternion spawnRot = Quaternion.Euler(rand);

            GameObject m = Instantiate(BoidPrefab, transform.position + spawn, spawnRot);
            BoidBehavior mm = m.GetComponent<BoidBehavior>();
            if (Target != null)
            {
                mm.TargetObject = Target;
            }

            AllBoids.Add(mm);
        }

    }
}
