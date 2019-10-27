using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class TreeCollider : MonoBehaviour
    {
        public AudioSource hitSound;
        public ParticleSystem hitParticles;
        private bool hit;

        private void Start()
        {
            gameObject.layer = 2;
        }

        private void OnTriggerEnter(Collider other)
        {
            //if (!other.GetComponent<DiscBehavior>().hitGround)
            if (hit == false && other.GetComponent<Rigidbody>() != null && other.GetComponent<Rigidbody>().velocity.magnitude > 1f)
            {
                hitParticles.gameObject.SetActive(true);
                var pos = other.transform.position;
                hitParticles.transform.position = pos;
                PlaySound();
                hitParticles.Play();
                
                other.GetComponent<DiscBehavior>().HitTreeBranches();

                hit = true;
                Invoke("HitCooldown", 2f);
            }
        }

        private void PlaySound()
        {
            hitSound.pitch = Random.Range(0.8f, 1.2f);
            hitSound.volume = Random.Range(0.4f, 0.6f);
            hitSound.Play();
        }

        private void HitCooldown()
        {
            hit = false;
            hitParticles.gameObject.SetActive(false);
        }
    }
}

