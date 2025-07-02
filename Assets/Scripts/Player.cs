using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Player settings")]
    [SerializeField]
    private float _speed = 7.0f;
    [SerializeField]
    private int _lives = 3;
    [SerializeField]
    private float _horizontalBound = 11.5f;
    [SerializeField]
    private float _fireRate = 0.3f;
    private float _canFire = -1f;
    [SerializeField]
    private int _maxAmmo = 15;
    [SerializeField]
    private int _currentAmmo;
    [SerializeField]
    private float _shiftSpeedModifier = 1.5f;

    [Header("Laser settings")]
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private float _laserSpawnOffset = 0.85f;
    [SerializeField]
    private AudioClip _laserSound;


    [Header("TripleShot settings")]
    [SerializeField]
    private GameObject _tripleShotPrefab;

    [SerializeField]
    private bool _isTripleShotActive = false;

    [Header("SpeedBoost settings")]
    [SerializeField]
    private float _speedBoost = 2f;

    [SerializeField]
    private bool _isSpeedBoostActive = false;

    [Header("Shield settings")]
    [SerializeField]
    private bool _isShieldActive = false;

    [SerializeField]
    private GameObject _shieldVisual;

    [SerializeField]
    private int _maxShieldLevel = 3;
    [SerializeField]
    private int _shieldLevel = 0;

    [Header("Score")]
    [SerializeField]
    private int _score;

    [Header("Other")]
    [SerializeField]
    private UIManager _uiManager;

    [SerializeField]
    private GameObject _leftFire;
    [SerializeField]
    private GameObject _rightFire;

    private AudioSource _audioSource;

    private SpawnManager _spawnManager;

    private float _horizontalInput;
    private float _verticalInput;



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

        _currentAmmo = _maxAmmo;
        _uiManager.UpdateAmmoCount(_currentAmmo);

    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();
        ShootProcedure();
    }

    void CalculateMovement()
    {
        _horizontalInput = Input.GetAxis("Horizontal");
        _verticalInput = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(_horizontalInput, _verticalInput, 0);

        if (_isSpeedBoostActive)
        {
            transform.Translate(direction * _speed * Time.deltaTime);
        } else
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                transform.Translate(direction * _speed * _shiftSpeedModifier * Time.deltaTime);
            }
            else
            {
                transform.Translate(direction * _speed * Time.deltaTime);
            }
        }


        if (transform.position.x > _horizontalBound)
        {
            transform.position = new Vector3(-_horizontalBound, transform.position.y, 0);
        }
        else if (transform.position.x < -_horizontalBound)
        {
            transform.position = new Vector3(_horizontalBound, transform.position.y, 0);
        }
        
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -4f, 2f), 0);
        
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
                _currentAmmo -= 3;
                _uiManager.UpdateAmmoCount(_currentAmmo);
            } else {
                Instantiate(_laserPrefab, transform.position + new Vector3(0, _laserSpawnOffset, 0), Quaternion.identity);
                _currentAmmo--;
                _uiManager.UpdateAmmoCount(_currentAmmo);
            }

            _audioSource.Play();
        }
    }

    public void Damage()
    {
        if (_isShieldActive)
        {
            ShieldDegradation();
            return;
        }
        else
        {
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
        _shieldLevel = _maxShieldLevel;
        _uiManager.UpdateShieldStatus(_shieldLevel -1);
    }

    // Shield Strength part
    private void ShieldDegradation()
    {
        _shieldLevel--;
        _uiManager.UpdateShieldStatus(_shieldLevel -1);
        if (_shieldLevel -1 < 0)
        {
            _isShieldActive = false;
            _shieldVisual.SetActive(false);
        }
    }

    public void AddScore(int pointsToAdd)
    {
        _score += pointsToAdd;
        _uiManager.UpdateScore(_score);
    }

    public void RefillAmmo()
    {
        _currentAmmo = _maxAmmo;
        _uiManager.UpdateAmmoCount(_currentAmmo);
    }

    public void HealPlayer()
    {
        _lives++;
        _uiManager.UpdateLives(_lives);

        if (_lives == 2)
        {
            _rightFire.SetActive(false);
        }
        else if (_lives > 2) 
        {
            _leftFire.SetActive(false);
        }
    }
}
