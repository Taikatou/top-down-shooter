﻿using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;

namespace MoreMountains.Feedbacks
{
    /// <summary>
    /// A struct used to trigger sounds
    /// </summary>
    public struct MMSfxEvent
    {
        public delegate void Delegate(AudioClip clipToPlay, AudioMixerGroup audioGroup = null, float volume = 1f, float pitch = 1f);
        static private event Delegate OnEvent;

        static public void Register(Delegate callback)
        {
            OnEvent += callback;
        }

        static public void Unregister(Delegate callback)
        {
            OnEvent -= callback;
        }

        static public void Trigger(AudioClip clipToPlay, AudioMixerGroup audioGroup = null, float volume = 1f, float pitch = 1f)
        {
            OnEvent?.Invoke(clipToPlay, audioGroup, volume, pitch);
        }
    }

    [ExecuteInEditMode]
    [AddComponentMenu("")]
    [FeedbackPath("Audio/Sound")]
    [FeedbackHelp("This feedback lets you play the specified AudioClip, either via event (you'll need something to catch a MMSfxEvent, that's not included in this package, but that's how it's done in the Corgi Engine and TopDown Engine), or cached (AudioSource gets created on init, and is then ready to be played), or on demand (instantiated on Play). For all these methods you can define a random volume between min/max boundaries (just set the same value in both fields if you don't want randomness), random pitch, and an optional AudioMixerGroup.")]
    public class MMFeedbackSound : MMFeedback
    {
        /// sets the inspector color for this feedback
        public override Color FeedbackColor { get { return MMFeedbacksInspectorColors.SoundsColor; } }

        /// <summary>
        /// The possible methods to play the sound with. 
        /// Event : sends a MMSfxEvent, you'll need a class to catch this event and play the sound
        /// Cached : creates and stores an audiosource to play the sound with, parented to the owner
        /// OnDemand : creates an audiosource and destroys it everytime you want to play the sound
        /// </summary>
        public enum PlayMethods { Event, Cached, OnDemand, Pool }

        [Header("Sound")]
        /// the sound clip to play
        public AudioClip Sfx;

        [Header("Random Sound")]
        /// an array to pick a random sfx from
        public AudioClip[] RandomSfx;

        [Header("Test")]
        [MMFInspectorButton("TestPlaySound")]
        public bool TestButton;

        [Header("Method")]
        /// the play method to use when playing the sound (event, cached or on demand)
        public PlayMethods PlayMethod = PlayMethods.Cached;
        [MMFEnumCondition("PlayMethod", (int)PlayMethods.Pool)]
        public int PoolSize = 10;

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

        /// the duration of this feedback is the duration of the clip being played
        public override float FeedbackDuration { get { return _duration; } }

        protected AudioClip _randomClip;
        protected AudioSource _cachedAudioSource;
        protected AudioSource[] _pool;
        protected AudioSource _tempAudioSource;
        protected float _duration;

        /// <summary>
        /// Custom init to cache the audiosource if required
        /// </summary>
        /// <param name="owner"></param>
        protected override void CustomInitialization(GameObject owner)
        {
            base.CustomInitialization(owner);
            if (PlayMethod == PlayMethods.Cached)
            {
                _cachedAudioSource = CreateAudioSource(owner, "CachedFeedbackAudioSource");
            }
            if (PlayMethod == PlayMethods.Pool)
            {
                // create a pool
                _pool = new AudioSource[PoolSize];
                for (int i = 0; i < PoolSize; i++)
                {
                    _pool[i] = CreateAudioSource(owner, "PooledAudioSource"+i);
                }
            }
        }

        protected virtual AudioSource CreateAudioSource(GameObject owner, string audioSourceName)
        {
            // we create a temporary game object to host our audio source
            GameObject temporaryAudioHost = new GameObject(audioSourceName);
            // we set the temp audio's position
            temporaryAudioHost.transform.position = owner.transform.position;
            temporaryAudioHost.transform.SetParent(owner.transform);
            // we add an audio source to that host
            _tempAudioSource = temporaryAudioHost.AddComponent<AudioSource>() as AudioSource;
            _tempAudioSource.playOnAwake = false;
            return _tempAudioSource; 
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
                if (Sfx != null)
                {
                    _duration = Sfx.length;
                    PlaySound(Sfx, position);
                    return;
                }

                if (RandomSfx.Length > 0)
                {
                    _randomClip = RandomSfx[Random.Range(0, RandomSfx.Length)];

                    if (_randomClip != null)
                    {
                        _duration = _randomClip.length;
                        PlaySound(_randomClip, position);
                    }
                    
                }
            }
        }

        /// <summary>
        /// Plays a sound differently based on the selected play method
        /// </summary>
        /// <param name="sfx"></param>
        /// <param name="position"></param>
        protected virtual void PlaySound(AudioClip sfx, Vector3 position)
        {
            float volume = Random.Range(MinVolume, MaxVolume);
            float pitch = Random.Range(MinPitch, MaxPitch);

            if (PlayMethod == PlayMethods.Event)
            {
                MMSfxEvent.Trigger(sfx, SfxAudioMixerGroup, volume, pitch);
                return;
            }

            if (PlayMethod == PlayMethods.OnDemand)
            {
                // we create a temporary game object to host our audio source
                GameObject temporaryAudioHost = new GameObject("TempAudio");
                // we set the temp audio's position
                temporaryAudioHost.transform.position = position;
                // we add an audio source to that host
                AudioSource audioSource = temporaryAudioHost.AddComponent<AudioSource>() as AudioSource;
                PlayAudioSource(audioSource, sfx, volume, pitch);
                // we destroy the host after the clip has played
                Destroy(temporaryAudioHost, sfx.length);
            }

            if (PlayMethod == PlayMethods.Cached)
            {
                // we set that audio source clip to the one in paramaters
                PlayAudioSource(_cachedAudioSource, sfx, volume, pitch);
            }

            if (PlayMethod == PlayMethods.Pool)
            {
                _tempAudioSource = GetAudioSourceFromPool();
                if (_tempAudioSource != null)
                {
                    PlayAudioSource(_tempAudioSource, sfx, volume, pitch);
                }
            }
        }

        /// <summary>
        /// Plays the audio source with the specified volume and pitch
        /// </summary>
        /// <param name="audioSource"></param>
        /// <param name="sfx"></param>
        /// <param name="volume"></param>
        /// <param name="pitch"></param>
        protected virtual void PlayAudioSource(AudioSource audioSource, AudioClip sfx, float volume, float pitch)
        {
            // we set that audio source clip to the one in paramaters
            audioSource.clip = sfx;
            // we set the audio source volume to the one in parameters
            audioSource.volume = volume;
            audioSource.pitch = pitch;
            // we set our loop setting
            audioSource.loop = false;
            // we start playing the sound
            audioSource.Play();
        }

        /// <summary>
        /// Gets an audio source from the pool if possible
        /// </summary>
        /// <returns></returns>
        protected virtual AudioSource GetAudioSourceFromPool()
        {
            for (int i = 0; i < PoolSize; i++)
            {
                if (!_pool[i].isPlaying)
                {
                    return _pool[i];
                }
            }
            return null;
        }
        
        /// <summary>
        /// A test method that creates an audiosource, plays it, and destroys itself after play
        /// </summary>
        protected virtual async void TestPlaySound()
        {
            AudioClip tmpAudioClip = null;

            if (Sfx != null)
            {
                tmpAudioClip = Sfx;
            }

            if (RandomSfx.Length > 0)
            {
                tmpAudioClip = RandomSfx[Random.Range(0, RandomSfx.Length)];
            }

            if (tmpAudioClip == null)
            {
                Debug.LogError(Label + " on "+this.gameObject.name + " can't play in editor mode, you haven't set its Sfx.");
                return;
            }

            float volume = Random.Range(MinVolume, MaxVolume);
            float pitch = Random.Range(MinPitch, MaxPitch);
            GameObject temporaryAudioHost = new GameObject("EditorTestAS_WillAutoDestroy");
            temporaryAudioHost.transform.position = this.transform.position;
            AudioSource audioSource = temporaryAudioHost.AddComponent<AudioSource>() as AudioSource;
            PlayAudioSource(audioSource, tmpAudioClip, volume, pitch);
            float length = 1000 * tmpAudioClip.length;
            await Task.Delay((int)length);
            DestroyImmediate(temporaryAudioHost);
        }
    }
}
