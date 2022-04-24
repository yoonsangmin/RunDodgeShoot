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
        float mixerVolume;

        if (MasterSlider != null)
        {
            //mixer.GetFloat("MasterVolume", out mixerVolume);
            //mixerVolume = Mathf.Pow(10, (mixerVolume / 20));
            //MasterSlider.value = mixerVolume;

            MasterSlider.value = SaveManager.Instance.gameData.masterVolume;
        }

        if (BgmSlider != null)
        {
            //mixer.GetFloat("BgmVolume", out mixerVolume);
            //mixerVolume = Mathf.Pow(10, (mixerVolume / 20));
            //BgmSlider.value = mixerVolume;

            BgmSlider.value = SaveManager.Instance.gameData.bgmVolume;
        }

        if (SfxSlider != null)
        {
            //mixer.GetFloat("SfxVolume", out mixerVolume);
            //mixerVolume = Mathf.Pow(10, (mixerVolume / 20));
            //SfxSlider.value = mixerVolume;

            SfxSlider.value = SaveManager.Instance.gameData.sfxVolume;
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

    private void OnDisable()
    {
        SaveManager.Instance.gameData.masterVolume = MasterSlider.value;
        SaveManager.Instance.gameData.bgmVolume = BgmSlider.value;
        SaveManager.Instance.gameData.sfxVolume = SfxSlider.value;
    }
}
