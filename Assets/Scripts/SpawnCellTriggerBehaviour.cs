using Simulation;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Material = Simulation.Material;

public class SpawnCellTriggerBehaviour : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown cellTypeDropdown;
    [SerializeField] private TMP_Dropdown brushSizeDropdown;

    private EventSystem _eventSystem;
    private int _brushSize = 1;
    private Material _material = Material.Static;
    
    private void Start()
    {
        _eventSystem = EventSystem.current;
        cellTypeDropdown.onValueChanged.AddListener(TypeChanged);
        brushSizeDropdown.onValueChanged.AddListener(BrushSizeChanged);
    }

    private void OnDestroy()
    {
        cellTypeDropdown.onValueChanged.RemoveListener(TypeChanged);
        brushSizeDropdown.onValueChanged.RemoveListener(BrushSizeChanged);
    }

    private void Update()
    {
        if (_eventSystem.IsPointerOverGameObject()) return;
        if (!Input.GetMouseButton(0)) return;
        Dispatcher.Instance.AddCell(Input.mousePosition, new Chemical {Color = Color.yellow, Material = _material}, _brushSize);
    }
    
    private void TypeChanged(int option)
    {
        _material = Material.Materials[option];
    }

    public void BrushSizeChanged(int option)
    {
        _brushSize = option + 1;
    } 
}