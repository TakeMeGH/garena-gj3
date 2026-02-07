using UnityEngine;
using GGJ.Code.Utils.Singleton;

namespace GGJ.Code.Audio
{
    public sealed class AudioManager : Singleton<AudioManager>
    {
        [Header("Sources")]
        [SerializeField]
        AudioSource bgmSource;

        [SerializeField]
        AudioSource sfxSource;

        [SerializeField]
        AudioSource loopedSfxSource;

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

        public void PlaySfx(AudioClip clip, float volume = 1f)
        {
            if (!clip)
            {
                return;
            }

            sfxSource.PlayOneShot(clip);
        }

        public void PlayLoopedSfx(AudioClip clip, bool restartIfSame = false)
        {
            if (!clip)
            {
                return;
            }


            if (loopedSfxSource.isPlaying && loopedSfxSource.clip == clip && !restartIfSame)
            {
                return;
            }

            loopedSfxSource.clip = clip;
            loopedSfxSource.loop = true;
            loopedSfxSource.Play();
        }

        public void StopLoopedSfx()
        {
            if (!loopedSfxSource) return;
            loopedSfxSource.Stop();
            loopedSfxSource.clip = null;
        }

        public void StopAllSfx()
        {
            if (sfxSource)
            {
                sfxSource.Stop();
            }

            StopLoopedSfx();
        }
    }
}