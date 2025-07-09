using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{

    [Header("ENEMY SPAWNING")]
    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private GameObject[] _rareEnemies;

    [Header("POWERUP SPAWNING")]
    [SerializeField]
    private GameObject[] _powerups;
    [SerializeField]
    private float _rareSpawnProbability = 0.25f;
    [SerializeField]
    private GameObject[] _rarePowerups;
    [SerializeField]
    private float _frequentSpawnProbability = 0.65f;
    [SerializeField]
    private GameObject[] _frequentPowerups;
    [SerializeField]
    private GameObject _powerupContainer;

    [SerializeField]
    private bool _isSpawningEnemies = false;
    [SerializeField]
    private bool _isSpawningPowerups = false;

    [Header("WAVE MANAGER")]
    [SerializeField]
    private int _currentWave = 1;
    [SerializeField]
    private int _enemiesPerLevel = 4;
    [SerializeField]
    private float _enemiesMultiplier = 1.5f;
    [SerializeField]
    private int _totalEnemiesSpawned = 0;
    [SerializeField]
    private int _remainingEnemies = 0;
    private bool _isEnemyLimitReached = false;

    [SerializeField]
    private UIManager _uiManager;

    private Coroutine _enemySpawnRoutine;
    private Coroutine _powerupSpawnRoutine;

    private void Start()
    {
        if (_uiManager == null)
        {
            Debug.LogError("UI Manager is NULL");
        }
    }



    public void StartSpawning()
    {
        _isSpawningEnemies = true;
        _isSpawningPowerups = true;

        if(_enemySpawnRoutine == null)
        {
            _enemySpawnRoutine = StartCoroutine(EnemySpawningRoutine());
        }

        if (_powerupSpawnRoutine == null)
        {
            _powerupSpawnRoutine = StartCoroutine(PowerupSpawningRoutine());
        }
    }

    IEnumerator EnemySpawningRoutine()
    {
        yield return new WaitForSeconds(0.75f);
        while (_isSpawningEnemies)
        {
            float randomX = Random.Range(-9f, 9f);
            GameObject newEnemy = Instantiate(_enemyPrefab, new Vector3(randomX, 10, 0), Quaternion.identity);
            newEnemy.transform.SetParent(_enemyContainer.transform);

            newEnemy.GetComponent<Enemy>().SetEnemyConfiguration(ConfigureBool(), ConfigureBool(), ConfigureBool(), ConfigureBool());
            AddEnemy();
            yield return new WaitForSeconds(5f);
        }

        _enemySpawnRoutine = null;
    }

    public void OnPlayerDeath()
    {
        _isSpawningEnemies = false;
        _isSpawningPowerups = false;
    }

    IEnumerator PowerupSpawningRoutine()
    {
        yield return new WaitForSeconds(1.5f);
        while (_isSpawningPowerups)
        {
            float randomX = Random.Range(-9f, 9f);

            //test for rare powerup
            if(Random.Range(0f, 1f) < _rareSpawnProbability && _rarePowerups.Length > 0)
            {
                int randomPowerupID = Random.Range(0, _rarePowerups.Length);
                GameObject newPowerup = Instantiate(_rarePowerups[randomPowerupID], new Vector3(randomX, 10, 0), Quaternion.identity);
                newPowerup.transform.SetParent(_powerupContainer.transform);
                Debug.Log("Rare powerup spawned");
                
            }
            else
            {   
                //test for frequent powerup
                if (Random.Range(0f, 1f) < _frequentSpawnProbability && _frequentPowerups.Length > 0)
                {
                    int randomPowerupID = Random.Range(0, _frequentPowerups.Length);
                    GameObject newPowerup = Instantiate(_frequentPowerups[randomPowerupID], new Vector3(randomX, 10, 0), Quaternion.identity);
                    newPowerup.transform.SetParent(_powerupContainer.transform);
                }
                else
                {
                    int randomPowerupID = Random.Range(0, _powerups.Length);
                    GameObject newPowerup = Instantiate(_powerups[randomPowerupID], new Vector3(randomX, 10, 0), Quaternion.identity);
                    newPowerup.transform.SetParent(_powerupContainer.transform);
                }
            }

            yield return new WaitForSeconds(Random.Range(3f, 7f));
        }

        _powerupSpawnRoutine = null;
    }


    public void AddEnemy()
    {
        _totalEnemiesSpawned++;
        _remainingEnemies++;
        Debug.Log("Enemies total: " + _totalEnemiesSpawned + "/" + _enemiesPerLevel);
        if(_totalEnemiesSpawned == _enemiesPerLevel)
        {
            Debug.Log("Reached enemy limit for wave. Stopped spawning enemies");
            _isSpawningEnemies = false;
            _isEnemyLimitReached = true;
        }
    }

    public void RemoveEnemy()
    {
        _remainingEnemies--;
        if(_remainingEnemies == 0 && _isEnemyLimitReached) 
        {
            _isSpawningPowerups = false;
            _isSpawningEnemies = false;
            _uiManager.WaveFinished();
        }
    }

    public void StartNewWave()
    {
        _enemiesPerLevel = (int)Mathf.Ceil(_enemiesPerLevel * _enemiesMultiplier);
        _totalEnemiesSpawned = 0;
        _remainingEnemies = 0;
        _isEnemyLimitReached = false;
        _currentWave++;

        StartSpawning();
    }

    public int GetCurrentWaveNumber()
    {
        return _currentWave;
    }

    private bool ConfigureBool()
    {
        int boolSetup = Random.Range(0, 2);
        return (boolSetup == 0) ? false : true;
    }

}
