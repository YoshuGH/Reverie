using UnityEngine;

public class AudioSystem : MonoBehaviour
{
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioSource _soundsSource;

    public void PlayMusic(AudioClip _audioclip)
    {
        _musicSource.clip = _audioclip;
        _musicSource.Play();
    }

    public void PlaySound(AudioClip _audioclip, float _vol = 1)
    {
        _soundsSource.PlayOneShot(_audioclip, _vol);
    }

    public void PlaySound(AudioClip _audioclip, Vector3 _pos, float _vol = 1)
    {
        _soundsSource.transform.position = _pos;
        PlaySound(_audioclip, _vol);
    }
}
