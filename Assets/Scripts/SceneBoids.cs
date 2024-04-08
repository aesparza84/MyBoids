using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneBoids : MonoBehaviour
{
    [SerializeField] private GameObject BoidPrefab;
    [SerializeField] private int BoidAmount;

    private List<BoidBehavior> AllBoids;

    private const float SpawnOffset = 4.0f;

    [SerializeField] private bool FreeFly;
    [SerializeField] private bool Cohesion;
    [SerializeField] private bool Alignment;
    [SerializeField] private bool Seperation;

    private void Awake()
    {
        AllBoids = new List<BoidBehavior>();

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

    private void Update()
    {
        UpdateBoids();
    }

    private void UpdateBoids()
    {
        foreach (BoidBehavior boid in AllBoids)
        {
            boid.FreeFly = FreeFly;
            boid.Alignment = Alignment;
            boid.Cohesion = Cohesion;
            boid.Seperation = Seperation;
            boid.CalculateMovement();
        }

        HandleInput();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            ResetAllBoids();
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
            AllBoids.Add(mm);
        }

    }
}
