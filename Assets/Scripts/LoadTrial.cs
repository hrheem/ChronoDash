using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadTrial : MonoBehaviour
{
    int[] nums = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17,
        18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33 };
    List<int> imageList;
    //bool showOptions = false;
    //bool showStimulus = true;
    bool leftSelected;
    int trialsCount = 0;
    int stimulusCount = 0;
    int index;
    int imgIndex;
    StringBuilder dataRow = new StringBuilder();
    float startTime;
    float RTbase;
    string answerLocation;
    string leftImg, rightimg;
    int left; //can have value 4 and 5, 4 means answer is on the left

    public GameObject stimulus;
    public GameObject option1;
    public GameObject option2;
    public GameObject submitButton;
    public GameObject interimCanvas;
    public GameObject leftHighlight;
    public GameObject rightHighlight;

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
        //SetTrialIndex();
        stimulus.SetActive(true);
        option1.SetActive(false);
        option2.SetActive(false);
        leftHighlight.SetActive(false);
        rightHighlight.SetActive(false);
        dataRow.Append((trialsCount + 1) + ",");

        imageList = new List<int>(nums);
        SetTrialIndex();
        StartCoroutine(LoadStimulus());
        FileManagement.init();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SetTrialIndex()
    {
        index = Random.Range(0, imageList.Count - 1);
        imgIndex = imageList[index];
        dataRow.Append((imgIndex) + ",");
    }

    IEnumerator LoadStimulus()
    {
        //Debug.Log("index == " + index);
        if (stimulusCount > 2)
        {
            stimulusCount = 0; // reset the stimulusCount for next trial
            LoadOptions();
            //showStimulus = false;
            //showOptions = true;
            StopAllCoroutines();
        }
        else
        {
            stimulusCount++;
            stimulus.SetActive(true);
            option1.SetActive(false);
            option2.SetActive(false);
            submitButton.SetActive(false);
            interimCanvas.SetActive(false);
            string imgname = "Set" + imgIndex + "/set" + imgIndex + "_sl" + stimulusCount;
            //Debug.Log("img path===" + imgname);

            stimulus.GetComponent<Image>().sprite = Resources.Load<Sprite>(imgname);
            dataRow.Append((Time.time-startTime) + ",");
            if (stimulusCount > 1)
            {
                stimulus.GetComponent<CanvasRenderer>().SetAlpha(0.1f);
                stimulus.GetComponent<Image>().CrossFadeAlpha(1f, 1f, false);
            }
            yield return new WaitForSeconds(5);
            StartCoroutine(LoadStimulus());
        }
    }

    void LoadOptions()
    {
        StopAllCoroutines();
        stimulus.SetActive(false);
        option1.SetActive(true);
        option2.SetActive(true);
        interimCanvas.SetActive(false);

        left = Random.Range(4, 6);
        if (left == 4)
        {
            answerLocation = "left";
            leftImg = "Set" + imgIndex + "/set" + imgIndex + "_sl4_ans";
            rightimg = "Set" + imgIndex + "/set" + imgIndex + "_sl4_noans";
        }
        else
        {
            answerLocation = "right";
            rightimg = "Set" + imgIndex + "/set" + imgIndex + "_sl4_ans";
            leftImg = "Set" + imgIndex + "/set" + imgIndex + "_sl4_noans";
        }
        //Debug.Log("leftImg path===" + leftImg);
        //Debug.Log("rightimg path===" + rightimg);
        option1.GetComponent<Image>().sprite = Resources.Load<Sprite>(leftImg);
        option2.GetComponent<Image>().sprite = Resources.Load<Sprite>(rightimg);
        dataRow.Append((Time.time - startTime) + ",");
        RTbase = Time.time;

        option1.GetComponent<CanvasRenderer>().SetAlpha(0.1f);
        option1.GetComponent<Image>().CrossFadeAlpha(1f, 1f, false);

        option2.GetComponent<CanvasRenderer>().SetAlpha(0.1f);
        option2.GetComponent<Image>().CrossFadeAlpha(1f, 1f, false);
    }

    public void SubmitOption()
    {
        logResponseData();
        FileManagement.dumpRow(dataRow.ToString());
        if (trialsCount == 32)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        else
        {
            stimulus.SetActive(false);
            option1.SetActive(false);
            option2.SetActive(false);
            submitButton.SetActive(false);
            interimCanvas.SetActive(true);
            leftHighlight.SetActive(false);
            rightHighlight.SetActive(false);
        }
    }

    public void LoadNextTrial()
    {
        //Debug.Log("trialsCount ==" + trialsCount);
        trialsCount++;
        dataRow = new StringBuilder();
        dataRow.Append((trialsCount + 1) + ",");
        //showStimulus = true;
        imageList.RemoveAt(index);
        //Debug.Log("imageList.Count ==" + imageList.Count);
        SetTrialIndex();
        StartCoroutine(LoadStimulus());
    }

    public void selectLeftImageOnClick()
    {
        leftSelected = true;
        submitButton.SetActive(true);
        leftHighlight.SetActive(true);
        rightHighlight.SetActive(false);
    }

    public void selectRightImageOnClick()
    {
        leftSelected = false;
        submitButton.SetActive(true);
        leftHighlight.SetActive(false);
        rightHighlight.SetActive(true);
    }

    void logResponseData()
    {
        dataRow.Append((Time.time - startTime) + ",");
        dataRow.Append((Time.time - RTbase) + ",");
        dataRow.Append(answerLocation + ",");
        if(leftSelected)
            dataRow.Append("left,");
        else
            dataRow.Append("right,");

        if (left == 4)
        {
            dataRow.Append(leftImg + ",");
            if (leftSelected)
                dataRow.Append(leftImg + ",Correct");
            else
                dataRow.Append(rightimg + ",Incorrect");
        }
        else
        {
            dataRow.Append(rightimg + ",");
            if (leftSelected)
                dataRow.Append(leftImg + ",Incorrect");
            else
                dataRow.Append(rightimg + ",Correct");
        }
    }
}
