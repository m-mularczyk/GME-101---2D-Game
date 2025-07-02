using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{

    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private GameObject[] _powerups;
    [SerializeField]
    private GameObject _powerupContainer;

    private bool _isSpawning = true;



    public void StartSpawning()
    {
        StartCoroutine(EnemySpawningRoutine());
        StartCoroutine(PowerupSpawningRoutine());
    }

    IEnumerator EnemySpawningRoutine()
    {
        yield return new WaitForSeconds(0.75f);
        while (_isSpawning)
        {
            float randomX = Random.Range(-9f, 9f);
            GameObject newEnemy = Instantiate(_enemyPrefab, new Vector3(randomX, 10, 0), Quaternion.identity);
            newEnemy.transform.SetParent(_enemyContainer.transform);
            yield return new WaitForSeconds(5f);
        }
    }

    public void OnPlayerDeath()
    {
        _isSpawning = false;
    }

    IEnumerator PowerupSpawningRoutine()
    {
        yield return new WaitForSeconds(1.5f);
        while (_isSpawning)
        {
            float randomX = Random.Range(-9f, 9f);
            int randomPowerupID = Random.Range(0, 5);
            GameObject newPowerup = Instantiate(_powerups[randomPowerupID], new Vector3(randomX, 10, 0), Quaternion.identity);
            newPowerup.transform.SetParent(_powerupContainer.transform);
            yield return new WaitForSeconds(Random.Range(3f, 7f));
        }
    }
}
