using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingControl : MonoBehaviour
{
    private PlayerControl playerControl;
    public GameObject settingPanel;

    private bool stopButton = false;

    // Start is called before the first frame update
    void Start()
    {
        playerControl = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
        settingPanel.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
//#if UNITY_EDITOR
//        stopButton |= Input.GetKeyDown(KeyCode.Escape);
//#endif

        if (stopButton && playerControl.step != PlayerControl.STEP.MISS)
        {
            if (settingPanel.gameObject.activeSelf == false)
            {
                settingPanel.gameObject.SetActive(true);
                Time.timeScale = 0.0f;
            }
            else
            {
                settingPanel.gameObject.SetActive(false);
                Time.timeScale = 1.0f;
            }

            stopButton = false;
        }
        else if(playerControl.step == PlayerControl.STEP.MISS)
        {
            settingPanel.gameObject.SetActive(false);
            Time.timeScale = 1.0f;
        }
    }

    public void OnStopButtonClick()
    {
        stopButton = true;
    }

    public void ToTitle()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("TitleScene");
    }
}
