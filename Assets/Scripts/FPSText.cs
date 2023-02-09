using TMPro;
using UnityEngine;

public class FPSText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    private void Update()
    {
        text.text = (1.0f / Time.deltaTime).ToString("F0");
    }
}