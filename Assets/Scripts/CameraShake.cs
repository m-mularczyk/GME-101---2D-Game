using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    //[SerializeField]
    //private Vector3 _cameraStartPos;
    [SerializeField]
    private float _cameraXShake = 0.12f;
    [SerializeField]
    private float _cameraShakeDuration = 0.1f;
    //[SerializeField]
    //private float _cameraMovementSpeed = 10f;

    // Start is called before the first frame update
    void Start()
    {
        //_cameraStartPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.V))
        {
            transform.position = new Vector3(-_cameraXShake, transform.position.y, transform.position.z);
        }
        */
    }

    public void CameraShaking()
    {
        StartCoroutine(CameraShakeProcedure());
    }

    IEnumerator CameraShakeProcedure()
    {
        
        transform.position = new Vector3(-_cameraXShake, transform.position.y, transform.position.z);
        yield return new WaitForSeconds(_cameraShakeDuration / 2);
        transform.position = new Vector3(_cameraXShake * 2, transform.position.y, transform.position.z);
        yield return new WaitForSeconds(_cameraShakeDuration / 2);
        transform.position = new Vector3(0, transform.position.y, transform.position.z);
        

        /* // Smooth movement but
        float elapsed = 0f;
        Vector3 originalPosition = transform.position;

        while (elapsed < _cameraShakeDuration)
        {
            float x = Mathf.Sin(elapsed * _cameraMovementSpeed * Mathf.PI * 2f) * _cameraXShake;
            float deltaX = x - (transform.position.x - originalPosition.x);

            transform.Translate(new Vector3(deltaX, 0f, 0f));

            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = originalPosition;
        */
    }
}
