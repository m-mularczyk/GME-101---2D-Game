using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class EnemyBoss : MonoBehaviour
{
    [SerializeField]
    private float _bossSpeed = 1f;
    [SerializeField]
    private int _bossLives = 5;
    [SerializeField]
    private int _bossScore = 100;
    private Vector3 _bossDirection = Vector3.down;

    private Player _player;
    private Collider2D _boxCollider2D;
    private SpawnManager _spawnManager;
    private UIManager _uiManager;

    private AudioSource _audioSource;
    private Animator _anim;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("Player is NULL");
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

        _anim = GetComponent<Animator>();
        if (_anim == null)
        {
            Debug.LogError("Animator is NULL");
        }

        _spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();
        if( _spawnManager == null)
        {
            Debug.LogError("Spawn Manager is NULL");
        }
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        if(_uiManager == null)
        {
            Debug.LogError("UI Manager is null");
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(_bossDirection * _bossSpeed * Time.deltaTime);

        if (transform.position.y <= 2f)
        {
            _bossSpeed = 0f;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Laser"))
        {

            Laser laser = other.GetComponent<Laser>();
            if (laser != null)
            {
                if (laser.GetIsEnemyLaser() == false)
                {
                    Destroy(other.gameObject);
                    OnBossHit();
                }
            }

        }
        else if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.Damage();
            }

            OnBossHit();
        }

    }

    public void OnBossHit()
    {
        if (_bossLives > 1)
        {
            _bossLives--;
        }
        else // Boss is dead
        {
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
