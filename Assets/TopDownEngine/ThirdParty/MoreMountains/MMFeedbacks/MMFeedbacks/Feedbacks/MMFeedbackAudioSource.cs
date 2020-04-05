using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace MoreMountains.Feedbacks
{
    [AddComponentMenu("")]
    [FeedbackPath("Sounds/AudioSource")]
    [FeedbackHelp("This feedback lets you play a target audio source, with some elements at random.")]
    public class MMFeedbackAudioSource : MMFeedback
    {
        /// the possible ways to interact with the audiosource
        public enum Modes { Play, Pause, UnPause, Stop }

        [Header("Sound")]
        /// the target audio source to play
        public AudioSource TargetAudioSource;
        /// whether we should play the audio source or stop it or pause it
        public Modes Mode = Modes.Play;

        [Header("Random Sound")]
        /// an array to pick a random sfx from
        public AudioClip[] RandomSfx;
        
        [Header("Volume")]
        /// the minimum volume to play the sound at
        public float MinVolume = 1f;
        /// the maximum volume to play the sound at
        public float MaxVolume = 1f;

        [Header("Pitch")]
        /// the minimum pitch to play the sound at
        public float MinPitch = 1f;
        /// the maximum pitch to play the sound at
        public float MaxPitch = 1f;

        [Header("Mixer")]
        /// the audiomixer to play the sound with (optional)
        public AudioMixerGroup SfxAudioMixerGroup;

        protected AudioClip _randomClip;    

        /// <summary>
        /// Custom init to cache the audiosource if required
        /// </summary>
        /// <param name="owner"></param>
        protected override void CustomInitialization(GameObject owner)
        {
            base.CustomInitialization(owner);
        }
        
        /// <summary>
        /// Plays either a random sound or the specified sfx
        /// </summary>
        /// <param name="position"></param>
        /// <param name="attenuation"></param>
        protected override void CustomPlayFeedback(Vector3 position, float attenuation = 1.0f)
        {
            if (Active)
            {
                switch(Mode)
                {
                    case Modes.Play:
                        if (RandomSfx.Length > 0)
                        {
                            _randomClip = RandomSfx[Random.Range(0, RandomSfx.Length)];
                            TargetAudioSource.clip = _randomClip;
                        }
                        float volume = Random.Range(MinVolume, MaxVolume);
                        float pitch = Random.Range(MinPitch, MaxPitch);
                        PlayAudioSource(TargetAudioSource, volume, pitch);
                        break;

                    case Modes.Pause:
                        TargetAudioSource.Pause();
                        break;

                    case Modes.UnPause:
                        TargetAudioSource.UnPause();
                        break;

                    case Modes.Stop:
                        TargetAudioSource.Stop();
                        break;
                }
            }
        }
        
        /// <summary>
        /// Plays the audiosource at the selected volume and pitch
        /// </summary>
        /// <param name="audioSource"></param>
        /// <param name="volume"></param>
        /// <param name="pitch"></param>
        protected virtual void PlayAudioSource(AudioSource audioSource, float volume, float pitch)
        {
            // we set the audio source volume to the one in parameters
            audioSource.volume = volume;
            audioSource.pitch = pitch;
            // we start playing the sound
            audioSource.Play();
        }

        /// <summary>
        /// Stops the audiosource from playing
        /// </summary>
        /// <param name="position"></param>
        /// <param name="attenuation"></param>
        public override void Stop(Vector3 position, float attenuation = 1.0f)
        {
            base.Stop(position, attenuation);
            TargetAudioSource?.Stop();
        }
    }
}
