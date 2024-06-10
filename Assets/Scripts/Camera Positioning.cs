using Cinemachine;
using UnityEngine;

public class Camera : MonoBehaviour
{
    private Transform _playerTransform;

    private CinemachineVirtualCamera _vCam;

    private bool _cameraPositioned;
    
    // Start is called before the first frame update
    void Start()
    {
        _vCam = GetComponent<CinemachineVirtualCamera>();
        _cameraPositioned = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_cameraPositioned)
        {
            if (_playerTransform == null)
            {
                _playerTransform = GameObject.Find("Harap Alb(Clone)").transform;
            }
            else
            {
                _vCam.LookAt = _playerTransform;
                _vCam.Follow = _playerTransform;
                _cameraPositioned = true;
            }
        }
    }
}
