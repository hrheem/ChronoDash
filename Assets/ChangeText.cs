using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class ChangeText : MonoBehaviour
{

    public UDPBaseDataListener udpSource;
    public GameObject enterText;
    public float sensorVal;
    public bool isAbsoluteValue = true;
    public RectTransform rectTransform;
    public int dataPointIndex = 1;
    private Rigidbody arrowObj;

    TextMeshProUGUI enterText_text;

    // Start is called before the first frame update
    void Start()
    {
        enterText_text = enterText.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
	sensorVal = (float)Math.Round(Mathf.Abs(udpSource.valArray[dataPointIndex]), 2);
        enterText_text.text = sensorVal.ToString();
    }

}
