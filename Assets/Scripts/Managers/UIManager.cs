using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

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

    private GameManager _gameManager;


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
}
