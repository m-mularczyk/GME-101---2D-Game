using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField]
    private float _cameraXShake = 0.12f;
    [SerializeField]
    private float _cameraShakeDuration = 0.1f;

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
    }
}
