using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    [SerializeField] private Transform _cameraPosition = null;
    void Update()
    {
        transform.position = _cameraPosition.position;
    }
}
