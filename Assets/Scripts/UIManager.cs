using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;
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

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateScore(int updatedScore)
    {
        _scoreText.text = "Score: " + updatedScore;
    }

    public void UpdateLives(int lives)
    {
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
}
