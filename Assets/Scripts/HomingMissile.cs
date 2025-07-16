using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingMissile : MonoBehaviour
{

    [SerializeField]
    private float _missileSpeed = 12f;
    [SerializeField]
    private float _missileLifespan = 3f;

    private Enemy[] _enemies;
    private Enemy _closestEnemy;
    
    private bool _targetAcquired = false;
    private Player _player;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if(_player == null)
        {
            Debug.LogError("Player is NULL");
        }

        // Get all enemies in the scene
        _enemies = GameObject.FindObjectsOfType<Enemy>();

        // Get all ALIVE enemies from these present in the scene
        List<Enemy> aliveEnemies = new List<Enemy>();
        foreach (Enemy enemy in _enemies)
        {
            if (enemy.IsEnemyAlive())
            {
                aliveEnemies.Add(enemy);
            }
        }

        // Search for the one that is closest to Player
        if (aliveEnemies.Count > 0)
        {
            _closestEnemy = aliveEnemies[0];
            for (int i = 0; i < aliveEnemies.Count; i++)
            {
                if (Vector3.Distance(transform.position, aliveEnemies[i].transform.position) < Vector3.Distance(transform.position, _closestEnemy.transform.position))
                {
                    _closestEnemy = aliveEnemies[i];
                }
            }
            _targetAcquired = true;
        }

        StartCoroutine(HomingMissileDisposal(_missileLifespan));
    }

    // Update is called once per frame
    void Update()
    {

        if (_closestEnemy == null || !_closestEnemy.IsEnemyAlive())
        {
            _targetAcquired = false;
            _closestEnemy = null;
        }

        if (_targetAcquired && _closestEnemy != null)
        {
  
                float step = _missileSpeed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, _closestEnemy.transform.position, step);

                Vector2 direction = _closestEnemy.transform.position - transform.position;
                transform.up = direction;
        }
        else
        {
            transform.Translate(Vector3.up * _missileSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (other.name == _closestEnemy.name)
            {
                Enemy enemy = other.GetComponent<Enemy>();
                if (enemy != null)
                {
                    if (!enemy.IsBoss() && !enemy.IsProtected())
                    {
                        _player.AddScore(10);
                    }
                    enemy.OnEnemyHit();
                    Destroy(gameObject);
                }
            }
        }
    }

    IEnumerator HomingMissileDisposal(float secondsToDestruction)
    {
        yield return new WaitForSeconds(secondsToDestruction);
        Destroy(gameObject);
    }
}
