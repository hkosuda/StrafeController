using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM
{
    public class PlayerSound : MonoBehaviour
    {
        static readonly int landingSoundFrameBuffer = 8;
        static readonly float footstepInterval = 0.32f;

        static readonly float volume = 1.0f;

        static float landingSoundFrameBufferRemain;
        static float footstepIntervalRemain;

        static AudioSource audioSource;
        static AudioClip landingSound;
        static AudioClip footstepSound;

        static float prevVy;

        private void Awake()
        {
            audioSource = gameObject.GetComponent<AudioSource>();
            landingSound = Resources.Load<AudioClip>("Player/landing");
            footstepSound = Resources.Load<AudioClip>("Player/footstep");
        }

        void Start()
        {
            SetEvent(1);
        }

        private void OnDestroy()
        {
            SetEvent(-1);
        }

        static void SetEvent(int indicator)
        {
            if (indicator > 0)
            {
                PM_Landing.Landed += PlayLandingSound;
            }

            else
            {
                PM_Landing.Landed -= PlayLandingSound;
            }
        }

        static void PlayLandingSound(object obj, RaycastHit hit)
        {
            if (landingSoundFrameBufferRemain > 0) { return; }

            audioSource.volume = volume;

            audioSource.PlayOneShot(landingSound);
            landingSoundFrameBufferRemain = landingSoundFrameBuffer;
        }

        void Update()
        {
            var dt = Time.deltaTime;

            landingSoundFrameBufferRemain--;
            if (landingSoundFrameBufferRemain < 0) { landingSoundFrameBufferRemain = 0; }

            if (PM_Landing.LandingIndicator <= 0)
            {
                footstepIntervalRemain = footstepInterval;
                return;
            }

            var speed = PmParams.pm_max_speed_on_ground;
            var velocity = new Vector2(Player.Rb.velocity.x, Player.Rb.velocity.z);

            if (velocity.magnitude < speed * 0.6f)
            {
                footstepIntervalRemain = footstepInterval;
                return;
            }

            footstepIntervalRemain -= dt;

            if (footstepIntervalRemain < 0.0f)
            {
                audioSource.volume = volume;
                footstepIntervalRemain = footstepInterval;
                audioSource.PlayOneShot(footstepSound);
            }
        }

        void LateUpdateMethod()
        {
            prevVy = Player.Rb.velocity.y;
        }
    }
}

