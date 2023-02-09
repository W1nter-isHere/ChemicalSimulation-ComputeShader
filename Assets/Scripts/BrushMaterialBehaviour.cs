using TMPro;
using UnityEngine;

public class BrushMaterialBehaviour : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown cellTypeDropdown;
    [SerializeField] private TMP_Dropdown brushSizeDropdown;
        
    private void Start()
    {
    }

    private void OnDestroy()
    {
        cellTypeDropdown.onValueChanged.RemoveListener(TypeChanged);
        brushSizeDropdown.onValueChanged.RemoveListener(BrushSizeChanged);
    }

    private void TypeChanged(int option)
    {
    }

    public void BrushSizeChanged(int option)
    {
    } 
}