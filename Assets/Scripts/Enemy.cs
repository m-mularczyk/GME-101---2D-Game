using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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

    [SerializeField]
    private bool _horizontalMovement = false;
    private Vector3 _horizontalDirection = Vector3.left;

    private Player _player;
    private Animator _anim;
    private BoxCollider2D _boxCollider2D;
    private AudioSource _audioSource;
    private SpawnManager _spawnManager;
    private GameManager _gameManager;
    private UIManager _uiManager;

    private bool _isEnemyAlive = true;
    private float _fireRate = 3f;
    private float _canFire = -1f;

    private bool _isRammingCountdownRunning = false;
    private bool _isEvadingCountdownRunning = false;

    [SerializeField]
    private Vector3 _laserOffset = Vector3.down;

    private bool _isEvading = false;
    private bool _isRamming = false;

    [SerializeField]
    private float _rammingSpeedMultiplier = 1.6f;

    [SerializeField]
    private bool _isBoss = false;
    [SerializeField]
    private float _bossSpeed = 1f;
    [SerializeField]
    private int _bossLives = 5;
    [SerializeField]
    private int _bossScore = 100;
    private Vector3 _bossDirection = Vector3.down;

    private bool _hasEvadeStarted = false;
    private Vector3 _evadeDirection = Vector3.left;

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

        _gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

        if (_isEnemyProtected)
        {
            _enemyShieldVisual.SetActive(true);
        }

        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        if( _uiManager == null)
        {
            Debug.Log("UI Manager is NULL");
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (!_isBoss)
        {
            CalculateMovement();

            if (Time.time > _canFire && _isEnemyAlive)
            {
                EnemyFiring();
            }

            EvadeLaser();
        }
        else
        {
            transform.Translate(_bossDirection * _bossSpeed * Time.deltaTime);

            if (transform.position.y <= 2f)
            {
                _bossSpeed = 0f;
            }
        }
        
    }

    private void EnemyFiring()
    {
        _fireRate = Random.Range(3f, 7f);
        _canFire = Time.time + _fireRate;
        if (_player != null)
        {
            if (_isEnemySmart && transform.position.y < _player.transform.position.y && !_gameManager.IsGameOver())
            {
                // Reverse shooting
                EnemyShotBackwards();
            }
            else
            {
                // Standard shooting
                EnemyShot();
            }
        }
        
    }

    private void EnemyShot()
    {
        GameObject enemyLaser = Instantiate(_enemyLaserPrefab, transform.position + _laserOffset, Quaternion.identity);
        Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();

        for (int i = 0; i < lasers.Length; i++)
        {
            lasers[i].AssignEnemyLaser();
        }
    }

    private void EnemyShotBackwards()
    {
        GameObject enemyLaser = Instantiate(_enemyLaserPrefab, transform.position + _laserOffset, Quaternion.Euler(new Vector3(0, 0, 180)));
        Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();

        for (int i = 0; i < lasers.Length; i++)
        {
            lasers[i].AssignEnemyLaser();
        }
    }

    private void CalculateMovement()
    {
        if (_isRamming && _isEnemyAggressive)
        {
            transform.Translate(Vector3.down * _enemySpeed * _rammingSpeedMultiplier * Time.deltaTime);
            
            if (!_isRammingCountdownRunning)
            {
                StartCoroutine(RammingCountdown());
                _isRammingCountdownRunning = true;
            }
        } else
        {
            transform.Translate(Vector3.down * _enemySpeed * Time.deltaTime);
        }

        if (_horizontalMovement )
        {
            transform.Translate(_horizontalDirection * (_enemySpeed / 2) * Time.deltaTime);

            if(transform.position.x <= -6 && _isEnemyAlive)
            {
                _horizontalDirection = Vector3.right;
            }
            
            if (transform.position.x >= 6 && _isEnemyAlive)
            {
                _horizontalDirection = Vector3.left;
            }
        }

        if (transform.position.y < -6 && _isEnemyAlive && !_gameManager.IsGameOver())
        {
            float randomX = Random.Range(-9f, 9f);
            float randomY = Random.Range(10f, 15f);
            transform.position = new Vector3(randomX, randomY, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_isEnemyAlive)
        {
            return;
        }

        if (other.CompareTag("Laser")){

            Laser laser = other.GetComponent<Laser>();
            if (laser != null) 
            {
                if(laser.GetIsEnemyLaser() == false)
                {
                    Destroy(other.gameObject);

                    if (!_isBoss && !_isEnemyProtected)
                    {
                        _player.AddScore(10);
                    }

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
        if (!_isBoss)
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
        else
        {
            if (_bossLives > 1)
            {
                _bossLives--;
            }
            else // Boss is dead
            {
                _isEnemyAlive = false;
                _player.AddScore(_bossScore);
                _boxCollider2D.enabled = false;

                _anim.SetTrigger("OnEnemyDeath");
                _audioSource.Play();

                _spawnManager.SetPowerupsSpawning(false);
                _uiManager.GameFinishedSequence(_player.PlayerScore());

                Destroy(this.gameObject, 2.8f);
            }
        }
        
    }

    public void SetEnemyConfiguration(bool evasive,bool aggressive, bool smart, bool shield, bool horizontalMovement)
    {
        _isEnemyEvasive = evasive;
        _isEnemyAggressive = aggressive;
        _isEnemySmart = smart;
        _isEnemyProtected = shield;
        _horizontalMovement = horizontalMovement;
    }


    // Laser evasive movement
    public void LaserDetected()
    {
        _isEvading = true;

        if (!_isEvadingCountdownRunning)
        {
            StartCoroutine(EvadingCountdown());
            _isEvadingCountdownRunning = true;
        }
    }

    public void EvadeLaser()
    {
        if(!_hasEvadeStarted)
        {
            _hasEvadeStarted = true;
            _evadeDirection = Random.Range(0, 2) == 0 ? Vector3.left : Vector3.right;
        }

        if (_isEvading && _isEnemyAlive)
        {
            {
                transform.Translate(_evadeDirection * 7 * Time.deltaTime);
            }
        }
    }

    IEnumerator EvadingCountdown()
    {
        //Debug.Log("Enemy is evading shot");
        yield return new WaitForSeconds(0.2f);
        _isEvading = false;
        _isEvadingCountdownRunning = false;
        _hasEvadeStarted = false;
    }

    public bool IsEnemyAlive()
    {
        return _isEnemyAlive;
    }

    public void RamPlayer(Player player)
    {
        _isRamming = true;
    }

    IEnumerator RammingCountdown()
    {
        //Debug.Log("Enemy is ramming player!");
        yield return new WaitForSeconds(0.75f);
        _isRamming = false;
        _isRammingCountdownRunning = false;
    }

    public void AttackPowerup()
    {
        Debug.Log("Enemy attacking a powerup!");
        EnemyShot();
    }

    public bool IsBoss()
    {
        return _isBoss;
    }

    public bool IsProtected()
    {
        return _isEnemyProtected;
    }
}
