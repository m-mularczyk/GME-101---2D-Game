using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLaser : MonoBehaviour
{
    [SerializeField]
    private float _laserSpeed = 7f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * _laserSpeed * Time.deltaTime);

        if(transform.position.y < -8)
        {
            Destroy(gameObject);
        }
    }
}
