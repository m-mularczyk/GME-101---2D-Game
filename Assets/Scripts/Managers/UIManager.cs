using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _scoreText;

    [SerializeField]
    private Image _livesImage;

    [SerializeField]
    private Sprite[] _liveSprites;

    [SerializeField]
    private TMP_Text _gameOverText;

    [SerializeField]
    private TMP_Text _restartText;

    [SerializeField]
    private Image _shieldStatus;
    [SerializeField]
    private Sprite[] _shieldRegression;

    [SerializeField]
    private TMP_Text _ammoText;


    [SerializeField]
    private Image _speedBoostDurationImage;
    private bool _isSpeedBoostActive = false;
    private float _speedBoostDuration;
    private float _countdownStart;
    private float _countdownLeft;
    [SerializeField]
    private TMP_Text _waveFinishedText;
    [SerializeField]
    private TMP_Text _newWaveCountdownText;

    [SerializeField]
    private GameObject _thrustersImage;
    [SerializeField]
    private Image _thrustersIndicatorImage;
    [SerializeField]
    private TMP_Text _thrustersCooldownCounterText;


    private GameManager _gameManager;
    private SpawnManager _spawnManager;


    // Start is called before the first frame update
    void Start()
    {
        _scoreText.text = "Score: " + 0;
        _gameOverText.gameObject.SetActive(false);

        _gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        if( _gameManager == null)
        {
            Debug.LogError("Game Manager is NULL");
        }

        _spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();
        if(_spawnManager == null)
        {
            Debug.LogError("Spawn Manager is NULL");
        }
    }

    private void Update()
    {
        if (_isSpeedBoostActive)
        {
            SpeedBoostIndicatorCountdown();
        }
    }

    public void UpdateScore(int updatedScore)
    {
        _scoreText.text = "Score: " + updatedScore;
    }

    public void UpdateLives(int lives)
    {
        if(lives < 0 || lives > _liveSprites.Length -1)
        { 
            return;
        }
        _livesImage.sprite = _liveSprites[lives];
        if(lives == 0)
        {
            GameOverSequence();
        }
    }

    IEnumerator GameOverUIProcedure()
    {

        while (true)
        {
            _gameOverText.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            _gameOverText.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void GameOverSequence()
    {
        StartCoroutine(GameOverUIProcedure());
        _restartText.gameObject.SetActive(true);
        _gameManager.GameOver();
    }

    // Shield Strength UI updating
    public void UpdateShieldStatus(int shieldLevel)
    {
        if(shieldLevel >= 0)
        {
            if(shieldLevel > _shieldRegression.Length - 1)
            {
                _shieldStatus.sprite = _shieldRegression[_shieldRegression.Length -1];
            } else
            {
                _shieldStatus.sprite = _shieldRegression[shieldLevel];
            }
            _shieldStatus.gameObject.SetActive(true);
            
        }
        else
        {
            _shieldStatus.gameObject.SetActive(false);
        }
    }

    // Ammo count UI updating
    public void UpdateAmmoCount(int ammoCount)
    {
        if (ammoCount >= 0)
        {
            _ammoText.text = "Ammo: " + ammoCount;
        }
        else
        {
            _ammoText.text = "Ammo: " + 0;
        }
    }

    // SpeedBoost duration indicator
    public void SpeedBoostIndicatorStart(float boostDuration)
    {
        _speedBoostDurationImage.gameObject.SetActive(true);
        _isSpeedBoostActive = true;
        _speedBoostDuration = boostDuration;
        _countdownStart = Time.time;
    }

    // SpeedBoost duration indicator
    private void SpeedBoostIndicatorCountdown()
    {
        _countdownLeft = _countdownStart + _speedBoostDuration - Time.time;
        _speedBoostDurationImage.fillAmount = _countdownLeft / _speedBoostDuration;
        if (_countdownLeft <= 0)
        {
            _countdownLeft = 0;
            _isSpeedBoostActive = false;
            _speedBoostDurationImage.gameObject.SetActive(false);
        }
    }


    public void WaveFinished()
    {
        StartCoroutine(WaveFinishedSequence());
    }

    IEnumerator WaveFinishedSequence()
    {
        _waveFinishedText.gameObject.SetActive(true);
        _waveFinishedText.text = "Wave " + _spawnManager.GetCurrentWaveNumber() + " finished";
        yield return new WaitForSeconds(1.5f);

        _newWaveCountdownText.gameObject.SetActive(true);
        _newWaveCountdownText.text = "New wave in: 3";
        yield return new WaitForSeconds(1f);
        _newWaveCountdownText.text = "New wave in: 2";
        yield return new WaitForSeconds(1f);
        _newWaveCountdownText.text = "New wave in: 1";
        yield return new WaitForSeconds(1f);

        _waveFinishedText.gameObject.SetActive(false);
        _newWaveCountdownText.gameObject.SetActive(false);

        _spawnManager.StartNewWave();
    }


    public void DisplayThrustersCharge(float chargeValue)
    {
        _thrustersIndicatorImage.fillAmount = chargeValue;
    }

    public void DisplayThrustersCooldown(int cooldownCount)
    {
        _thrustersCooldownCounterText.gameObject.SetActive(true);
        _thrustersCooldownCounterText.text = cooldownCount.ToString();
    }

    public void HideThrustersCooldown()
    {
        _thrustersCooldownCounterText.gameObject.SetActive(false);
    }

    public void ThrustersActive(bool value)
    {
        if(value == true)
        {
            _thrustersImage.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
            _thrustersImage.GetComponent<SpriteRenderer>().color = new Color(1f, 0.73f, 0.73f, 1f);
            //_thrustersIndicatorImage.gameObject.SetActive(true);
        }
        else
        {
            _thrustersImage.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            _thrustersImage.GetComponent<SpriteRenderer>().color = Color.white;
        }
        
    }
}
