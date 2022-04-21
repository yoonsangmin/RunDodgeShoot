using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundPanelControl : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponentInChildren<Slider>().value = TitleRoot.Instance.sound_volume;
    }

    // Update is called once per frame
    void Update()
    {
        TitleRoot.Instance.sound_volume = this.GetComponentInChildren<Slider>().value;
    }

    public void SoundPanelSwichButton()
    {
        if (this.gameObject.activeSelf == true)
        {
            this.gameObject.SetActive(false);
        }
        else if(this.gameObject.activeSelf == false)
        {
            this.gameObject.SetActive(true);
        }
    }
}
