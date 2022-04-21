using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingControl : MonoBehaviour
{
    private PlayerControl playerControl;
    public GameObject settingPanel;

    private Slider volumeSlider = null;

    // Start is called before the first frame update
    void Start()
    {
        playerControl = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
        volumeSlider = settingPanel.GetComponentInChildren<Slider>();
        volumeSlider.value = TitleRoot.Instance.sound_volume;
        settingPanel.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && playerControl.step != PlayerControl.STEP.MISS)
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
        }
        else if(playerControl.step == PlayerControl.STEP.MISS)
        {
            settingPanel.gameObject.SetActive(false);
            Time.timeScale = 1.0f;
        }

        TitleRoot.Instance.sound_volume = volumeSlider.value;
    }

    public void ToTitle()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("TitleScene");
    }
}
