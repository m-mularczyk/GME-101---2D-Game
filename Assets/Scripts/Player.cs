using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 7.0f;
    [SerializeField]
    private float _horizontalBound = 11.5f;

    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private float _laserSpawnOffset = 0.85f;

    [SerializeField]
    private float _fireRate = 0.3f;
    private float _canFire = -1f;

    [SerializeField]
    private int _lives = 3;

    [SerializeField]
    private GameObject _tripleShotPrefab;

    private SpawnManager _spawnManager;
    [SerializeField]
    private bool _isTripleShotActive = false;
    [SerializeField]
    private float _speedBoost = 2f;

    [SerializeField]
    private bool _isSpeedBoostActive = false;
    [SerializeField]
    private bool _isShieldActive = false;

    [SerializeField]
    private GameObject _shieldVisual;

    [SerializeField]
    private int _score;

    [SerializeField]
    private UIManager _uiManager;

    [SerializeField]
    private GameObject _leftFire;
    [SerializeField]
    private GameObject _rightFire;

    private AudioSource _audioSource;
    [SerializeField]
    private AudioClip _laserSound;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(0, -2.75f, 0);

        _spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();
        if (_spawnManager == null)
        {
            Debug.LogError("Spawn Manager is NULL");
        }

        if(_uiManager == null)
        {
            Debug.LogError("UI Manager is NULL");
        }

        _audioSource = GetComponent<AudioSource>();
        if(_audioSource == null)
        {
            Debug.LogError("AudioSource is NULL");
        }
        else
        {
            _audioSource.clip = _laserSound;
        }

    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();
        ShootProcedure();
    }

    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);

         transform.Translate(direction * _speed * Time.deltaTime);



        if (transform.position.x > _horizontalBound)
        {
            transform.position = new Vector3(-_horizontalBound, transform.position.y, 0);
        }
        else if (transform.position.x < -_horizontalBound)
        {
            transform.position = new Vector3(_horizontalBound, transform.position.y, 0);
        }
        
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -4f, 2f), 0);
        
        /*
        if (transform.position.y >= _verticalBound)
        {
            transform.position = new Vector3(transform.position.x, _verticalBound, 0);
        }
        else if (transform.position.y <= -4f)
        {
            transform.position = new Vector3(transform.position.x, -4f, 0);
        }
        */
    }

    void ShootProcedure()
    {
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            /*
            // Simple laser shooting
            _canFire = Time.time + _fireRate;
            Instantiate(_laserPrefab, transform.position + new Vector3(0, _laserSpawnOffset, 0), Quaternion.identity);
            */

            //Laser and Triple Shot shooting
            _canFire = Time.time + _fireRate;
            if (_isTripleShotActive)
            {
                Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
            } else {
                Instantiate(_laserPrefab, transform.position + new Vector3(0, _laserSpawnOffset, 0), Quaternion.identity);
            }

            _audioSource.Play();
        }
    }

    public void Damage()
    {
        if (_isShieldActive)
        {
            _isShieldActive = false;
            _shieldVisual.SetActive(false);
            return;
        }

        _lives--;
        _uiManager.UpdateLives(_lives);

        if (_lives == 2)
        {
            _leftFire.SetActive(true);
        }
        else if (_lives == 1)
        {
            _rightFire.SetActive(true);
        }

        if (_lives < 1)
        {
            _spawnManager.OnPlayerDeath();
            Destroy(this.gameObject);
        }
    }

    public void TripleShotActive()
    {
        _isTripleShotActive = true;
        StartCoroutine(TripleShotCountdownRoutine());
    }

    IEnumerator TripleShotCountdownRoutine()
    {
        yield return new WaitForSeconds(5);
        _isTripleShotActive = false;
    }

    public void SpeedBoostActive()
    {
        _isSpeedBoostActive = true;
        _speed = _speed * _speedBoost;
        StartCoroutine(PowerBoostCountdownRoutine());
    }

    IEnumerator PowerBoostCountdownRoutine()
    {
        yield return new WaitForSeconds(5);
        _speed = _speed / _speedBoost;
        _isSpeedBoostActive = false;
    }

    public void ShieldBoostActive()
    {
        _isShieldActive = true;
        _shieldVisual.SetActive(true);
    }

    public void AddScore(int pointsToAdd)
    {
        _score += pointsToAdd;
        _uiManager.UpdateScore(_score);
    }

    /*
  private void OnTriggerEnter2D(Collider2D other)
  {
      if(other.CompareTag("Enemy Laser"))
      {
          Damage();
      }
  }
  */
}
