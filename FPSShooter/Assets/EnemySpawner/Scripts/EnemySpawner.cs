using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public List<GameObject> possibleEnemies;
    public float spawnInterval;
    public EnemyCountCalculation enemyCountCalculation;

    private Player player;

    private void Awake()
    {
        if (enemyCountCalculation == null)
        {
            enemyCountCalculation = GetComponent<EnemyCountCalculation>();
        }
        player = FindObjectOfType<Player>();
    }

    public void SpawnEnemies(int wave)
    {
        int enemiesToSpawn = enemyCountCalculation.Calculate(wave);
        int enemiesRemain = enemiesToSpawn;
        List<SpawnPointWeight> spawnPoints;
        GetSpawnPointWeights(out spawnPoints);
        for (int i = 0; i< spawnPoints.Count; i++)
        {
            int spawnPointEnemiesCount = Mathf.RoundToInt(spawnPoints[i].weight * enemiesToSpawn);
            spawnPointEnemiesCount = System.Math.Min(spawnPointEnemiesCount, enemiesRemain);
            StartCoroutine(SpawnEnemiesOnPoint(spawnPoints[i].spawnPoint, spawnPointEnemiesCount));
            enemiesRemain -= spawnPointEnemiesCount;
        }
        StartCoroutine(SpawnEnemiesOnPoint(spawnPoints[spawnPoints.Count - 1].spawnPoint, enemiesRemain));
    }

    private bool GetSpawnPointWeights(out List<SpawnPointWeight> spawnPoints)
    {
        if(transform.childCount == 0)
        {
            spawnPoints = null;
            return false;
        }

        spawnPoints = new List<SpawnPointWeight>(transform.childCount);
        foreach (Transform child in transform)
        {
            spawnPoints.Add(new SpawnPointWeight(child, DistanceToPlayer(child.position)));
        }
        spawnPoints.Sort((spawnPoint1, spawnPoint2) => spawnPoint1.weight.CompareTo(spawnPoint2.weight));
        NormalizeWeights(spawnPoints);
        return true;

    }

    private struct SpawnPointWeight
    {
        public SpawnPointWeight(Transform newSpawnPoint, float newWeight)
        {
            spawnPoint = newSpawnPoint;
            weight = newWeight;
        }

        public Transform spawnPoint;
        public float weight;

        public void SetWeight(float newWeight)
        {
            weight = newWeight;
        }
    }

    private float DistanceToPlayer(Vector3 spawnPointPosition)
    {
        return Vector3.Distance(player.transform.position, spawnPointPosition);
    }

    private float NormalizeWeights(List<SpawnPointWeight> spawnPoints)
    {
        float sum = 0;
        foreach(var spawnPoint in spawnPoints)
        {
            sum += spawnPoint.weight;
        }

        for(int i=0; i< spawnPoints.Count; i++)
        {
            spawnPoints[i] = new SpawnPointWeight(spawnPoints[i].spawnPoint, spawnPoints[i].weight / sum);
        }

        return sum;
    }

    private IEnumerator SpawnEnemiesOnPoint(Transform spawnPoint, int enemyCount)
    {
        for(int i = 0; i< enemyCount; i++)
        {
            Instantiate(GetRandomEnemy(), spawnPoint.position, Quaternion.identity);
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private GameObject GetRandomEnemy()
    {
        return possibleEnemies[Random.Range(0, possibleEnemies.Count)];
    }
}
