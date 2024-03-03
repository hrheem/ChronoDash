using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;

public class ChangeScene : MonoBehaviour
{
    public TextMeshProUGUI pid;
    public TextMeshProUGUI cond;
    public TextMeshProUGUI note;

    public void LoadNextScene()
    {
        SavePlayerData();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    void SavePlayerData()
    {
        //Debug.Log("pid.text == " + pid.text);
        //Debug.Log("Condition.text == " + cond.text);
        //Debug.Log("Note.text == " + note.text);
        PlayerPrefs.SetString("PID", pid.text);
        PlayerPrefs.SetString("Condition", cond.text);
        //PlayerPrefs.SetString("Note", note.text);
    }
}
