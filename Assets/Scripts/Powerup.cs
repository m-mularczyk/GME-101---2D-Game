using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{

    [SerializeField]
    private float _powerUpSpeed = 3f;

    [SerializeField]
    //0 - TripleShot
    //1 - Speed
    //2 - Shields
    private int _powerupID;

    [SerializeField]
    private AudioClip _clip;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * _powerUpSpeed * Time.deltaTime);

        if(transform.position.y < -9)
        {
            Destroy(gameObject);
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
                        //Debug.Log("Collected PowerBoost");
                        player.SpeedBoostActive();
                        break;
                    case 2:
                        //Debug.Log("Collected Shields");
                        player.ShieldBoostActive();
                        break;
                    default:
                        break;
                }

            }
            
            Destroy(gameObject);
        }
    }
}
