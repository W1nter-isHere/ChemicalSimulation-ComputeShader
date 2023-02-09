using TMPro;
using UnityEngine;

public class FPSTextBehaviour : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    private void Start()
    {
        InvokeRepeating(nameof(UpdateText), 0, 0.1f);
    }

    private void UpdateText()
    {
        text.text = "FPS: " + (1.0f / Time.deltaTime).ToString("F0");
    }
}