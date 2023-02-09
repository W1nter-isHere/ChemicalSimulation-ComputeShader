using UnityEngine;

public class CameraZoomBehaviour : MonoBehaviour
{
    public static CameraZoomBehaviour Instance { get; private set; }
    public float OrthographicSize => _mainCamera.orthographicSize;

    private Camera _mainCamera;
        
    private void Awake()
    {
        _mainCamera = GetComponent<Camera>();
        Instance = this;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Minus))
        {
            _mainCamera.orthographicSize += 0.5f;
        }
        
        if (Input.GetKey(KeyCode.Equals))
        {
            _mainCamera.orthographicSize -= 0.5f;
        }
            
        _mainCamera.orthographicSize = Mathf.Clamp(_mainCamera.orthographicSize, 1f, 50f);
    }
}