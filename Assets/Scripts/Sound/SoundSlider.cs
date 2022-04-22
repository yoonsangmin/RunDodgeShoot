using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SoundSlider : MonoBehaviour
{
    [SerializeField] private Slider MasterSlider;
    [SerializeField] private Slider BgmSlider;
    [SerializeField] private Slider SfxSlider;

    private AudioMixer mixer;

    private void Start()
    {
        mixer = SoundManager.Instance.Mixer;
        float volume;

        if (MasterSlider != null)
        {
            mixer.GetFloat("MasterVolume", out volume);
            volume = Mathf.Pow(10, (volume / 20));
            MasterSlider.value = volume;
        }

        if (SfxSlider != null)
        {
            mixer.GetFloat("SfxVolume", out volume);
            volume = Mathf.Pow(10, (volume / 20));
            SfxSlider.value = volume;
        }

        if (BgmSlider != null)
        {
            mixer.GetFloat("BgmVolume", out volume);
            volume = Mathf.Pow(10, (volume / 20));
            BgmSlider.value = volume;
        }
    }

    // 볼륨 조절 슬라이드 함수
    public void SetMasterVolume(float val)
    {
        if (val < 0.0001f)
            val = 0.0001f;
        mixer.SetFloat("MasterVolume", Mathf.Log10(val) * 20);
    }

    public void SetBgmVolume(float val)
    {
        if (val < 0.0001f)
            val = 0.0001f;
        mixer.SetFloat("BgmVolume", Mathf.Log10(val) * 20);
    }

    public void SetSfxVolume(float val)
    {
        if (val < 0.0001f)
            val = 0.0001f;
        mixer.SetFloat("SfxVolume", Mathf.Log10(val) * 20);
    }
}
