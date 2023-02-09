using UnityEngine;
using UnityEngine.EventSystems;

public class SpawnCellTriggerBehaviour : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
        
    private EventSystem _eventSystem;
        
    private void Start()
    {
        _eventSystem = EventSystem.current;
    }

    private void Update()
    {
        if (_eventSystem.IsPointerOverGameObject()) return;
        if (!Input.GetMouseButton(0)) return;
        Dispatcher.Instance.AddCell(Input.mousePosition, Color.white);
    }
}