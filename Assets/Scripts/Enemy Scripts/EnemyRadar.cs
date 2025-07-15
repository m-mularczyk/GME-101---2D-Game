using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRadar : MonoBehaviour
{
    private Enemy _enemy;

    // Start is called before the first frame update
    void Start()
    {
        _enemy = transform.parent.GetComponent<Enemy>();
        if( _enemy == null)
        {
            Debug.LogError("Enemy is NULL");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Player laser in range
        if (other.CompareTag("Laser"))
        {
            Laser laser = other.GetComponent<Laser>();
            if (laser != null)
            {
                if (laser.GetIsEnemyLaser() == false)
                {
                    _enemy.LaserDetected();
                }
            }
        }
        // Player in range
        else if (other.CompareTag("Player"))
        {
            _enemy.RamPlayer(other.GetComponent<Player>());
        }
        // Powerup in range
        else if (other.CompareTag("Powerup"))
        {
            _enemy.AttackPowerup();
        }
    }
}
