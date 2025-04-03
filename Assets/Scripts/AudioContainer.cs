using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

public class AudioContainer
{   
    const float MAX_VOLUME = 20;
    const float MIN_VOLUME = -80;

    public MuteButton MuteButton {get; private set;}
    public Slider Slider {get; private set;}
    public AudioContainer(string mixerName, MuteButton muteButton, Slider slider, AudioMixer audioMixer)
    {
        MuteButton = muteButton;
        Slider = slider;

        // ミュートボタンの設定
        MuteButton.Muted = (isMuted) => {
            // ミュートのboolenがなぜかないため、ボリュームを最低値にするしかない
            // https://discussions.unity.com/t/how-do-you-mute-a-audiomixergroup-from-code/571066
            if (isMuted) {
                audioMixer.SetFloat(mixerName, MIN_VOLUME);
                return;
            }

            // ミュート解除
            // （スライダーの値の範囲をボリュームと変える場合はMathf.Lerpが必要）
            audioMixer.SetFloat(mixerName, Slider.value);
        };

        // スライダーの設定
        Slider.RegisterValueChangedCallback(evt => {
            MuteButton.Mute(false);
        });     
    }

    public void SetVolumeData(VolumeData data)
    {
        if (data == null) {
            return;
        }

        data.Volume = Slider.value;
        data.IsMuted = MuteButton.IsMuted;
    }
    /// <summary>
    /// ボリューム設定
    /// 少なくともStart後に呼び出すこと
    /// </summary>
    /// <param name="data"></param>
    public void InitFromVolumeData(VolumeData data)
    {
        if (data == null) {
            return;
        }

        Slider.value = data.Volume;
        MuteButton.Mute(data.IsMuted);
    }
}
