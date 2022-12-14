using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class ObstacleSpawner : MonoBehaviour
{

    #region Inspector

    [SerializeField] private List<GameObject> prefabsList;

    [SerializeField] private float minWaitForSpawn = 1f;
    [SerializeField] private float maxWaitForSpawn = 5f;

    #endregion

    [SerializeField] private bool canSpawn = true;

    #region Unity Event Functions

    private void OnEnable()
    {
        canSpawn = true;
    }

    private void OnDisable()
    {
        canSpawn = true;
    }

    private void Awake()
    {
        //SpawnObstacle(spawnFrequency);
    }

    private void Update()
    {
        if (!canSpawn) return;
        float waitForSpawn = Random.Range(minWaitForSpawn, maxWaitForSpawn);
        StartCoroutine(SpawnObstacleDelayed(waitForSpawn));
        canSpawn = false;
    }

    private void SpawnObstacle()
    {
        int listLength = prefabsList.Count;
        //Debug.Log(listLength);
        int randomPrefab = Random.Range(0, listLength);
        Instantiate(prefabsList[randomPrefab], transform.position, transform.rotation);
        //GameObject obstacleInstance = Instantiate(prefabsList[randomPrefab], transform.position, transform.rotation);
    }

    #endregion

    public void IncreaseSpawnRate(float time)
    {
        if (maxWaitForSpawn > 1 + time)
        {
            maxWaitForSpawn -= time;
        }
        
    }
    
    public void DecreaseSpawnRate(float time)
    {
        maxWaitForSpawn += time;

    }

    public void SetSpawnRates(float min, float max)
    {
        minWaitForSpawn = min;
        maxWaitForSpawn = max;
    }

    private IEnumerator SpawnObstacleDelayed(float time)
    {
        yield return new WaitForSeconds(time);
        SpawnObstacle();
        canSpawn = true;
    }

}
