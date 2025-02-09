using UnityEngine;
using UnityEngine.Audio;

public class MiniAudioManager : MonoBehaviour
{
    [System.Serializable]
    public class AudioData
    {
        public AudioSource MusicSource;
        public AudioSource SoundSource;
        public AudioMixer GameAudio;
    }

    [SerializeField] 
    private AudioData _audioData;
    private AudioSource _musicSource => _audioData.MusicSource;
    private AudioSource _soundSource => _audioData.SoundSource;
    private AudioMixer _gameAudio => _audioData.GameAudio;

    private float _volume;
    private bool _isMuted;

    public void PlayMusic(string musicClipName)
    {
        _musicSource.Stop();
        AudioClip musicClip = Resources.Load<AudioClip>("Audio/Music/" + musicClipName);
        
        if (musicClip == null)
        {
            Debug.Log(musicClipName + " doesn't exist");
        }
        else
        {
            _musicSource.PlayOneShot(musicClip);
        }
    }

    public void PlaySound(string soundClipName, bool randomPitch=false)
    {
        AudioClip soundClip = Resources.Load<AudioClip>("Audio/Sounds/" + soundClipName);
        if (soundClip == null)
        {
            Debug.Log(soundClip + " doesn't exist");
            return;
        }

        if (randomPitch)
        {
            float pitch = Random.Range(0.75f, 1.25f);
            _soundSource.pitch = pitch;
        }
        _soundSource.PlayOneShot(soundClip);
        _soundSource.pitch = 1;
    }

    public void SetVolume(float volume)
    {
        if (volume == -35)
        {
            volume = -80;
        }
        _gameAudio.SetFloat("Volume", volume);
        _volume = volume;
    }
    public void ToggleMute()
    {
        if (!_isMuted)
        {
            _isMuted = true;
            _gameAudio.SetFloat("Volume", -80);
        }
        else
        {
            _gameAudio.SetFloat("Volume", _volume);
            _isMuted = false;
        }
    }
}
