using UnityEngine;

[System.Serializable]
public class AudioClipContainer
{
    [field: SerializeField]
    public AudioClip Clip {get; set;}

    [field: SerializeField]
    public float Volume {get; set;} = 1;

    public void Play(AudioSource source)
    {
        if (source == null) {
            return;
        }
        source.PlayOneShot(Clip, Volume);
    }
}
