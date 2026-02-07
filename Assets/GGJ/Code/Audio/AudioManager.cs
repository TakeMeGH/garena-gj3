using System.Collections.Generic;
using UnityEngine;
using GGJ.Code.Utils.Singleton;

namespace GGJ.Code.Audio
{
    public sealed class AudioManager : Singleton<AudioManager>
    {
        [System.Serializable]
        public sealed class AudioClipGroup
        {
            public string Id;
            public AudioClip[] Clips;
            public bool Randomize;
        }

        [Header("Library")]
        [SerializeField]
        AudioClipGroup[] bgmLibrary;

        [SerializeField]
        AudioClipGroup[] sfxLibrary;

        [SerializeField]
        AudioClipGroup[] loopedSfxLibrary;

        [Header("Sources")]
        [SerializeField]
        AudioSource bgmSource;

        [SerializeField]
        AudioSource sfxSource;

        [SerializeField]
        AudioSource loopedSfxSource;

        readonly Dictionary<AudioClip, AudioSource> _loopedSfxByClip = new();
        readonly List<AudioSource> _loopedSfxPool = new();

        public void PlayBgm(string id, bool restartIfSame = false)
        {
            AudioClip clip = GetClipFromLibrary(bgmLibrary, id);
            PlayBgm(clip, restartIfSame);
        }

        public void PlayBgm(AudioClip clip, bool restartIfSame = false)
        {
            if (!clip)
            {
                return;
            }

            if (bgmSource.isPlaying && bgmSource.clip == clip && !restartIfSame)
            {
                return;
            }

            bgmSource.clip = clip;
            bgmSource.Play();
        }

        public void StopBgm()
        {
            if (bgmSource)
            {
                bgmSource.Stop();
                bgmSource.clip = null;
            }
        }

        public void PauseBgm()
        {
            if (bgmSource)
            {
                bgmSource.Pause();
            }
        }

        public void ResumeBgm()
        {
            if (bgmSource && bgmSource.clip)
            {
                bgmSource.UnPause();
            }
        }

        public void PlaySfx(string id, float volume = 1f)
        {
            AudioClip clip = GetClipFromLibrary(sfxLibrary, id);
            PlaySfx(clip, volume);
        }

        public void PlaySfx(AudioClip clip, float volume = 1f)
        {
            if (!clip)
            {
                return;
            }

            sfxSource.PlayOneShot(clip, Mathf.Clamp01(volume));
        }

        public void PlayLoopedSfx(string id, bool restartIfSame = false)
        {
            AudioClip clip = GetClipFromLibrary(loopedSfxLibrary, id);
            PlayLoopedSfx(clip, restartIfSame);
        }

        public void PlayLoopedSfx(AudioClip clip, bool restartIfSame = false)
        {
            if (!clip)
            {
                return;
            }

            if (_loopedSfxByClip.TryGetValue(clip, out AudioSource existing))
            {
                if (existing.isPlaying && !restartIfSame)
                {
                    return;
                }

                existing.clip = clip;
                existing.loop = true;
                existing.Play();
                return;
            }

            AudioSource source = GetLoopedSfxSource();
            source.clip = clip;
            source.loop = true;
            source.Play();
            _loopedSfxByClip[clip] = source;
        }

        public void StopLoopedSfx()
        {
            StopAllLoopedSfx();
        }
        
        public void StopLoopedSfx(string id)
        {
            AudioClip clip = GetClipFromLibrary(loopedSfxLibrary, id);
            StopLoopedSfx(clip);
        }

        public void StopLoopedSfx(AudioClip clip)
        {
            if (!clip)
            {
                return;
            }

            if (_loopedSfxByClip.TryGetValue(clip, out AudioSource source))
            {
                source.Stop();
                source.clip = null;
                _loopedSfxByClip.Remove(clip);
                ReturnLoopedSfxSource(source);
            }
        }

        public void StopAllLoopedSfx()
        {
            foreach (AudioSource source in _loopedSfxByClip.Values)
            {
                source.Stop();
                source.clip = null;
                ReturnLoopedSfxSource(source);
            }

            _loopedSfxByClip.Clear();
        }

        public void StopAllSfx()
        {
            if (sfxSource)
            {
                sfxSource.Stop();
            }

            StopAllLoopedSfx();
        }

        AudioSource GetLoopedSfxSource()
        {
            if (loopedSfxSource && !IsLoopedSourceInUse(loopedSfxSource) && !_loopedSfxPool.Contains(loopedSfxSource))
            {
                ConfigureLoopedSource(loopedSfxSource);
                return loopedSfxSource;
            }

            if (_loopedSfxPool.Count > 0)
            {
                AudioSource pooled = _loopedSfxPool[0];
                _loopedSfxPool.RemoveAt(0);
                ConfigureLoopedSource(pooled);
                return pooled;
            }

            AudioSource created = gameObject.AddComponent<AudioSource>();
            ConfigureLoopedSource(created);
            return created;
        }

        void ReturnLoopedSfxSource(AudioSource source)
        {
            if (!source || source == loopedSfxSource)
            {
                return;
            }

            if (!_loopedSfxPool.Contains(source))
            {
                _loopedSfxPool.Add(source);
            }
        }

        bool IsLoopedSourceInUse(AudioSource source)
        {
            foreach (AudioSource active in _loopedSfxByClip.Values)
            {
                if (active == source)
                {
                    return true;
                }
            }

            return false;
        }

        void ConfigureLoopedSource(AudioSource source)
        {
            source.playOnAwake = false;
            source.loop = true;
        }

        AudioClip GetClipFromLibrary(AudioClipGroup[] library, string id)
        {
            AudioClipGroup group = FindGroup(library, id);
            if (group == null || group.Clips == null || group.Clips.Length == 0)
            {
                return null;
            }

            if (!group.Randomize || group.Clips.Length == 1)
            {
                return group.Clips[0];
            }

            int index = Random.Range(0, group.Clips.Length);
            return group.Clips[index];
        }

        AudioClipGroup FindGroup(AudioClipGroup[] library, string id)
        {
            if (library == null || library.Length == 0 || string.IsNullOrEmpty(id))
            {
                return null;
            }

            for (int i = 0; i < library.Length; i++)
            {
                AudioClipGroup group = library[i];
                if (group != null && group.Id == id)
                {
                    return group;
                }
            }

            return null;
        }
    }
}
