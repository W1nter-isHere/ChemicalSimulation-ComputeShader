using TMPro;
using UnityEngine;

public class FPSTextBehaviour : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    private void Update()
    {
        text.text = (1.0f / Time.deltaTime).ToString("F0");
    }
}