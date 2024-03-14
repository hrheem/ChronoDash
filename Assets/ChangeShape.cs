using UnityEngine;
using System;

public class ChangeShape : MonoBehaviour
{

    public UDPBaseDataListener udpSource;

    public float sensorVal;
    public bool isAbsoluteValue = true;
    public RectTransform rectTransform;
    public int dataPointIndex = 1;
    private Rigidbody arrowObj;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        arrowObj = GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void Update()
    {
        sensorVal = (float)Math.Round(Mathf.Abs(udpSource.valArray[dataPointIndex]), 2);
        rectTransform.sizeDelta = new Vector3(2000, sensorVal * 35);

    }

}
