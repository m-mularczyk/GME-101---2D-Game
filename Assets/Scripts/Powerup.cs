using UnityEngine;

public class Powerup : MonoBehaviour
{

    [SerializeField]
    private float _powerUpSpeed = 3f;
    [SerializeField]
    private float _magnetPower = 12f;

    private bool _movingTowardsPlayer = false;
    private GameObject _playerObject;

    [Tooltip("0 - TripleShot\r\n1 - Speed\r\n2 - Shields\r\n3 - Ammo\r\n4 - Health\r\n5 - Shotgun\r\n6 - Homing Missle\r\n7 - Bomb (negative)")]
    [SerializeField]
    //0 - TripleShot
    //1 - Speed
    //2 - Shields
    //3 - Ammo
    //4 - Health
    //5 - Shotgun
    //6 - Homing Missile
    //7 - Bomb (negative)
    private int _powerupID;

    [SerializeField]
    private AudioClip _clip;


    private void Start()
    {
        _playerObject = GameObject.Find("Player");
    }
    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * _powerUpSpeed * Time.deltaTime);

        if(transform.position.y < -9)
        {
            Destroy(gameObject);
        }

        if(_movingTowardsPlayer)
        {
            MoveToPlayer();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {

                AudioSource.PlayClipAtPoint(_clip, transform.position);

                switch (_powerupID)
                {
                    case 0:
                        //Debug.Log("TripleShot activated");
                        player.TripleShotActive();
                        break;
                    case 1:
                        //Debug.Log("Collected SpeedBoost");
                        player.SpeedBoostActive();
                        break;
                    case 2:
                        //Debug.Log("Collected Shields");
                        player.ShieldBoostActive();
                        break;
                    case 3:
                        //Debug.Log("Collected Ammo");
                        player.RefillAmmo();
                        break;
                    case 4:
                        //Debug.Log("Collected Health");
                        player.HealPlayer();
                        break;
                    case 5:
                        //Debug.Log("Collected Shotgun");
                        player.ShotgunActive();
                        break;
                    case 6:
                        //Debug.Log("Collected Homing Missile");
                        player.HomingMissileActive();
                        break;
                    case 7:
                        //Debug.Log("Collected Negative pickup");
                        player.Damage();
                        break;
                    default:
                        break;
                }
            }
            
            Destroy(gameObject);
        }
    }

    public void StartMovingTowardsPlayer()
    {
        _movingTowardsPlayer = true;
    }

    public void MoveToPlayer()
    {
        var step = _magnetPower * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, _playerObject.transform.position, step);
    }
}
