using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class HomingMissile : MonoBehaviour
{

    [SerializeField]
    private float _missileSpeed = 12f;

    private Enemy[] _enemies;
    private List<Enemy> _potentialTargets;
    private Enemy _closestEnemy;

    private bool _targetAcquired = false;

    // Start is called before the first frame update
    void Start()
    {
        _enemies = GameObject.FindObjectsOfType<Enemy>();

        if (_enemies.Length > 0)
        {
            _closestEnemy = _enemies[0];
            for (int i = 0; i < _enemies.Length; i++)
            {
                if (Vector3.Distance(transform.position, _enemies[i].transform.position) < Vector3.Distance(transform.position, _closestEnemy.transform.position))
                {
                    _closestEnemy = _enemies[i];
                }
            }
             _targetAcquired = true;
  
        }
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(HomingMissileRemoval());

        if (_targetAcquired && _closestEnemy.IsEnemyAlive())
        {
            float step = _missileSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, _closestEnemy.transform.position, step);

            Vector2 direction = _closestEnemy.transform.position - transform.position;
            transform.up = direction;
        }
        else
        {
            transform.Translate(Vector3.up * _missileSpeed * Time.deltaTime);

            if (transform.position.y > 8)
            {
                if (transform.parent != null)
                {
                    Destroy(transform.parent.gameObject);
                }
                Destroy(gameObject);
            }
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
                    enemy.OnEnemyHit();
                    Destroy(gameObject);
                }
            }
        }
    }

    IEnumerator HomingMissileRemoval()
    {
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }
}
