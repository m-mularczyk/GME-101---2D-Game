using System.Collections;
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

    [Header("Homing Missile settings")]
    [SerializeField]
    private GameObject _homingMissilePrefab;
    [SerializeField]
    private float _homingMissileOffset = 1.1f;
    [SerializeField]
    private bool _isHomingMissileActive = false;

    [Header("SpeedBoost settings")]
    [SerializeField]
    private float _speedBoost = 2f;
    [SerializeField]
    private float _speedBoostDuration = 5f;

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

    [Header("Special Weapon settings")]
    [SerializeField]
    private GameObject _specialWeaponPrefab;
    [SerializeField]
    private bool _isShotgunActive = false;

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
    private GameObject _mainCamera;
    
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

        _mainCamera = GameObject.Find("Main Camera");
        if (_mainCamera == null)
        {
            Debug.LogError("Main Camera is NULL");
        }

        _currentAmmo = _maxAmmo;
        _uiManager.UpdateAmmoCount(_currentAmmo);

    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();
        ShootProcedure();

        if (Input.GetKeyDown(KeyCode.C))
        {
            Powerup[] powerups = GameObject.FindObjectsOfType<Powerup>();
            if (powerups.Length > 0)
            {
                foreach (Powerup powerup in powerups)
                {
                    powerup.StartMovingTowardsPlayer();
                }
            }

            //Debug.Log("Powerup magnets enabled");
        }
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

            _canFire = Time.time + _fireRate;
            /*
            // Simple laser shooting
            _canFire = Time.time + _fireRate;
            Instantiate(_laserPrefab, transform.position + new Vector3(0, _laserSpawnOffset, 0), Quaternion.identity);
            */

            if (_isHomingMissileActive)
            {
                //Debug.Log("Homing missile shooting");
                Instantiate(_homingMissilePrefab, transform.position + Vector3.up * _homingMissileOffset, Quaternion.identity);
                _isHomingMissileActive = false;
            }
            // Special shot
            else if (_isShotgunActive)
            {
                //Debug.Log("Special shot shooting");
                Instantiate(_specialWeaponPrefab, transform.position + Vector3.up * _laserSpawnOffset, Quaternion.identity);
            }
            // Triple shot
            else if (_isTripleShotActive)
            {
                Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
                _currentAmmo -= 3;
                _uiManager.UpdateAmmoCount(_currentAmmo);
            }
            // Standard shot
            else
            {
                Instantiate(_laserPrefab, transform.position + Vector3.up * _laserSpawnOffset, Quaternion.identity);
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
            _mainCamera.GetComponent<CameraShake>().CameraShaking();

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

        _uiManager.SpeedBoostIndicatorStart(_speedBoostDuration);

        StartCoroutine(SpeedBoostCountdownRoutine());
    }

    IEnumerator SpeedBoostCountdownRoutine()
    {
        yield return new WaitForSeconds(5f);
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
        if(_lives == 3)
        {
            return;
        }
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

    public void SpecialWeaponActive()
    {
        _isShotgunActive = true;
        StartCoroutine(SpecialWeaponCountdown());
    }

    IEnumerator SpecialWeaponCountdown()
    {
        yield return new WaitForSeconds(5);
        _isShotgunActive = false;
    }

    public void HomingMissileActive()
    {
        _isHomingMissileActive = true;
    }

}
