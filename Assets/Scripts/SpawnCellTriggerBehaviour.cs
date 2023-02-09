using System.Linq;
using Simulation;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpawnCellTriggerBehaviour : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown cellTypeDropdown;
    [SerializeField] private TMP_Dropdown brushSizeDropdown;
    [SerializeField] private Toggle precisionMode;

    private EventSystem _eventSystem;
    private int _brushSize = 1;
    private Chemical _chemical = Chemical.Hydrogen;
    
    private void Start()
    {
        _eventSystem = EventSystem.current;
        cellTypeDropdown.onValueChanged.AddListener(TypeChanged);
        brushSizeDropdown.onValueChanged.AddListener(BrushSizeChanged);
        cellTypeDropdown.AddOptions(Chemical.DefaultChemicals.Select(chem => chem.Name).ToList());
    }

    private void OnDestroy()
    {
        cellTypeDropdown.onValueChanged.RemoveListener(TypeChanged);
        brushSizeDropdown.onValueChanged.RemoveListener(BrushSizeChanged);
    }

    private void Update()
    {
        if (_eventSystem.IsPointerOverGameObject()) return;
        if (precisionMode.isOn)
        {
            if (!Input.GetMouseButtonDown(0)) return;
        }
        else
        {
            if (!Input.GetMouseButton(0)) return;
        }
        Dispatcher.Instance.AddCell(Input.mousePosition, _chemical, _brushSize);
    }
    
    private void TypeChanged(int option)
    {
        _chemical = Chemical.DefaultChemicals[option];
    }

    private void BrushSizeChanged(int option)
    {
        _brushSize = option + 1;
    } 
}