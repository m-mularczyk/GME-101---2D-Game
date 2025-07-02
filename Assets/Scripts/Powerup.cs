using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{

    [SerializeField]
    private float _powerUpSpeed = 3f;

    [Tooltip("0 - TripleShot\r\n1 - Speed\r\n2 - Shields\r\n3 - Ammo\r\n4 - Health")]
    [SerializeField]
    //0 - TripleShot
    //1 - Speed
    //2 - Shields
    //3 - Ammo
    //4 - Health
    private int _powerupID;

    [SerializeField]
    private AudioClip _clip;


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
                    case 3:
                        //Debug.Log("Collected Ammo");
                        player.RefillAmmo();
                        break;
                    case 4:
                        //Debug.Log("Collected Health");
                        player.HealPlayer();
                        break;
                    default:
                        break;
                }
            }
            
            Destroy(gameObject);
        }
    }
}
