using UnityEngine;
using TMPro;

public class AutoStatus : MonoBehaviour
{
    public UDPBaseDataListener udpSource;
    public GameObject enterText;
    public int dataPointIndex = 11;

    TextMeshProUGUI enterText_text;

    void Start()
    {
        enterText_text = enterText.GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        int automationStatus = (int)udpSource.valArray[dataPointIndex];
        enterText_text.text = (automationStatus == 1) ? "Automation On" : "Automation Off";
    }
}
