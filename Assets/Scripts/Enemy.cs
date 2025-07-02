using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _enemySpeed = 4f;

    private Player _player;

    private Animator _anim;
    private BoxCollider2D _boxCollider2D;

    private AudioSource _audioSource;

    [SerializeField]
    private GameObject _enemyLaserPrefab;

    private bool _isEnemyAlive = true;

    private float _fireRate = 3f;
    private float _canFire = -1f;

    private Vector3 _laserOffser = Vector3.down;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("Player is NULL");
        }
        _anim = GetComponent<Animator>();
        if (_anim == null)
        {
            Debug.LogError("Animator is NULL");
        }
        _boxCollider2D = GetComponent<BoxCollider2D>();
        if (_boxCollider2D == null)
        {
            Debug.LogError("BoxCollider2D is NULL");
        }

        _audioSource = GetComponent<AudioSource>();
        {
            if (_audioSource == null)
            {
                Debug.LogError("AudioSource is NULL");
            }
        }

        //StartCoroutine(EnemyShotCooldown());
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();

        if(Time.time > _canFire && _isEnemyAlive)
        {
            EnemyFiring();
        }
    }

    private void EnemyFiring()
    {
        _fireRate = Random.Range(3f, 7f);
        _canFire = Time.time + _fireRate;
        GameObject enemyLaser = Instantiate(_enemyLaserPrefab, transform.position + _laserOffser, Quaternion.identity);
        Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();

        for (int i = 0; i < lasers.Length; i++)
        {
            lasers[i].AssignEnemyLaser();
        }
    }

    private void CalculateMovement()
    {
        transform.Translate(Vector3.down * _enemySpeed * Time.deltaTime);

        if (transform.position.y < -6 && _isEnemyAlive)
        {
            float randomX = Random.Range(-9f, 9f);
            float randomY = Random.Range(10f, 15f);
            transform.position = new Vector3(randomX, randomY, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Laser")){

            Laser laser = other.GetComponent<Laser>();
            if (laser != null) 
            {
                if(laser.GetIsEnemyLaser() == false)
                {
                    Destroy(other.gameObject);

                    _player.AddScore(10);

                    OnEnemyDeath();
                }
            }

        } else if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null) {
                player.Damage();
            }

            OnEnemyDeath();
        }

    }

    private void OnEnemyDeath()
    {
        _anim.SetTrigger("OnEnemyDeath");
        _boxCollider2D.enabled = false;
        _isEnemyAlive = false;
        _audioSource.Play();
        Destroy(this.gameObject, 2.8f);
    }

    /*
    private void EnemyShot()
    {
        Instantiate(_enemyLaserPrefab, transform.position + new Vector3(0, -1, 0), Quaternion.identity);
    }

    IEnumerator EnemyShotCooldown()
    {
        yield return new WaitForSeconds(1f);
        while (_isEnemyAlive) 
        {
            EnemyShot();
            yield return new WaitForSeconds(Random.Range(3f, 7f));
        }
        
    }
    */
}
