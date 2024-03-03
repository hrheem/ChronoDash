using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class GaugeControl : MonoBehaviour
{

    public UDPBaseDataListener udpSource;
    public GameObject enterText;
    public float sensorVal;
    public bool isAbsoluteValue = true;
    public RectTransform rectTransform;
    //public float leftRotation = -120;
    //public float rightRotation = 120;
    //public float conversionFactor = 2.23693629f;
    //public float valByDegree = 2;
    public int dataPointIndex = 1;
    private Rigidbody arrowObj;

    TextMeshProUGUI enterText_text;

    // Start is called before the first frame update
    void Start()
    {
        //rectTransform = GetComponent<RectTransform>();
        //arrowObj = GetComponent<Rigidbody>();
        enterText_text = enterText.GetComponent<TextMeshProUGUI>();

    }

    // Update is called once per frame
    void Update()
    {

        if (isAbsoluteValue)
        {

            sensorVal = (float)Math.Round(Mathf.Abs(udpSource.valArray[dataPointIndex]), 2);
            //sensorVal = udpSource.valArray[dataPointIndex];
        }
        else
        {

            sensorVal = (float)Math.Round(Mathf.Round(udpSource.valArray[dataPointIndex]), 2);
            //sensorVal = udpSource.valArray[dataPointIndex];
        }
        enterText_text.text = sensorVal.ToString();
        //rectTransform.sizeDelta = new Vector3(2000, sensorVal * 7);

    }

}
