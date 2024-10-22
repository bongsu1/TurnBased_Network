using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager
{
    public enum SoundType
    {
        BGM,
        Effect,
        Speech,
    }

    private AudioSource[] _audioSources = null;

    private GameObject _soundRoot = null;

    public void Init()
    {
        if (_soundRoot == null)
        {
            _soundRoot = GameObject.Find("Sound_Root");
            if (_soundRoot == null)
            {
                _soundRoot = new GameObject { name = "Sound_Root" };
                UnityEngine.Object.DontDestroyOnLoad(_soundRoot);

                string[] soundTypeNames = Enum.GetNames(typeof(SoundType));
                _audioSources = new AudioSource[soundTypeNames.Length];
                for (int i = 0; i < soundTypeNames.Length; i++)
                {
                    GameObject go = new GameObject { name = soundTypeNames[i] };
                    _audioSources[i] = Utils.GetOrAddComponent<AudioSource>(go);
                    go.transform.SetParent(_soundRoot.transform);
                }

                _audioSources[(int)SoundType.BGM].loop = true;
            }
        }
    }

    public bool Play(SoundType type, string path, float volume = 1.0f, float pitch = 1.0f)
    {
        if (string.IsNullOrEmpty(path))
            return false;

        AudioClip audioClip = GetAudioClip(path);
        if (audioClip == null)
            return false;

        AudioSource audioSource = _audioSources[(int)type];
        audioSource.volume = volume;
        audioSource.pitch = pitch;

        switch (type)
        {
            case SoundType.BGM:
            case SoundType.Speech:
                if (audioSource.isPlaying)
                    audioSource.Stop();

                audioSource.clip = audioClip;
                audioSource.Play();
                return true;

            case SoundType.Effect:
                audioSource.PlayOneShot(audioClip);
                return true;

            default:
                Debug.Log("사운드 타입 매칭 안됨");
                return false;
        }
    }

    public void Stop(SoundType type)
    {
        AudioSource audioSource = _audioSources[(int)type];
        audioSource.Stop();
    }

    public void SetPitch(SoundType type, float pitch = 1.0f)
    {
        AudioSource audioSource = _audioSources[(int)type];
        if (audioSource == null)
            return;

        audioSource.pitch = pitch;
    }

    public void Clear()
    {
        for (int i = 0; i < _audioSources.Length; i++)
        {
            _audioSources[i].Stop();
        }
    }

    private AudioClip GetAudioClip(string path)
    {
        AudioClip audioClip = Manager.Resource.Load<AudioClip>(path);
        if (audioClip == null)
            return null;

        return audioClip;
    }

    // 추후에 FMOD로 변경
}
