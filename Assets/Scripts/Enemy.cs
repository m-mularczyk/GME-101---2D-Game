using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _enemySpeed = 4f;
    [SerializeField]
    private GameObject _enemyLaserPrefab;
    [SerializeField]
    private GameObject _enemyShieldVisual;
    
    [SerializeField]
    private bool _isEnemyEvasive = false;
    [SerializeField]
    private bool _isEnemyAggressive = false;
    [SerializeField]
    private bool _isEnemySmart = false;
    
    [SerializeField]
    private bool _isEnemyProtected = false;

    private Player _player;
    private Animator _anim;
    private BoxCollider2D _boxCollider2D;
    private AudioSource _audioSource;
    private SpawnManager _spawnManager;

    private bool _isEnemyAlive = true;

    private float _fireRate = 3f;
    private float _canFire = -1f;

    [SerializeField]
    private Vector3 _laserOffset = Vector3.down;

    private bool _isEvading = false;

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

        _spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();
        if(_spawnManager == null)
        {
            Debug.LogError("Spawn Manager is NULL");
        }

        if (_isEnemyProtected)
        {
            _enemyShieldVisual.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();

        if(Time.time > _canFire && _isEnemyAlive)
        {
            EnemyFiring();
        }

        EvadeLaser();
    }

    private void EnemyFiring()
    {
        _fireRate = Random.Range(3f, 7f);
        _canFire = Time.time + _fireRate;
        GameObject enemyLaser = Instantiate(_enemyLaserPrefab, transform.position + _laserOffset, Quaternion.identity);
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

                    OnEnemyHit();
                }
            }

        } else if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null) {
                player.Damage();
            }

            OnEnemyHit();
        }

    }

    public void OnEnemyHit()
    {
        if (!_isEnemyAlive)
        {
            return;
        }

        if (_isEnemyProtected)
        {
            _isEnemyProtected = false;
            _enemyShieldVisual.SetActive(false);
            return;
        }

        _isEnemyAlive = false;
        _boxCollider2D.enabled = false;

        _anim.SetTrigger("OnEnemyDeath");
        _audioSource.Play();

        _spawnManager.RemoveEnemy();

        Destroy(this.gameObject, 2.8f);
        
    }

    public void SetEnemyConfiguration(bool evasive,bool aggressive, bool smart, bool shield)
    {
        
        _isEnemyEvasive = evasive;
        _isEnemyAggressive = aggressive;
        _isEnemySmart = smart;
        _isEnemyProtected = shield;
    }


    // Laser evasive movement
    public void LaserDetected()
    {
        _isEvading = true;
    }

    public void EvadeLaser()
    {
        if (_isEvading && _isEnemyAlive)
        {
            {
                transform.Translate(Vector3.left * 7 * Time.deltaTime);
            }
                StartCoroutine(EvadingCountdown());
        }
    }

    IEnumerator EvadingCountdown()
    {
        yield return new WaitForSeconds(0.2f);
        _isEvading = false;
    }

    public bool IsEnemyAlive()
    {
        return _isEnemyAlive;
    }
}
