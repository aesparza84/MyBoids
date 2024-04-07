using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneBoids : MonoBehaviour
{
    [SerializeField] private GameObject BoidPrefab;
    [SerializeField] private int BoidAmount;

    private List<BoidBehavior> AllBoids;

    private const float SpawnOffset = 4.0f;

    private void Awake()
    {
        AllBoids = new List<BoidBehavior>();

        for (int i = 0; i < BoidAmount; i++)
        {
            float x = UnityEngine.Random.Range(-SpawnOffset, SpawnOffset);
            float y = UnityEngine.Random.Range(-SpawnOffset, SpawnOffset);
            float z = UnityEngine.Random.Range(-SpawnOffset, SpawnOffset);
            Vector3 spawn = new Vector3(x, y, z);

            GameObject m = Instantiate(BoidPrefab, transform.position + spawn, Quaternion.identity);
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
            boid.CalculateMovement();
        }
    }
}
