using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField]
    private float _rotationSpeed = 20f;
    [SerializeField]
    private GameObject _explosionPrefab;
    [SerializeField]
    private SpawnManager _spawnManager;

    // Start is called before the first frame update
    void Start()
    {
        if(_spawnManager == null)
        {
            Debug.LogError("SpawnManager is NULL");
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0, 0, _rotationSpeed) * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Laser"))
        {
            _spawnManager.StartSpawning();
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            Destroy(other.gameObject);
            Destroy(gameObject, 0.5f);
        }
    }
}
