using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TFI.MathTek.ID113
{
    public class Bow : MonoBehaviour
    {
        [SerializeField] GameObject arrow;
        [SerializeField] Animator animator;
        [SerializeField] AudioSource sfxAim;
        [SerializeField] AudioSource sfxRelease;

        public Vector3 Target { set; get; }

        public void Shoot()
        {
            if (Target == null) return;

            GameObject _arrow = Instantiate(arrow, arrow.transform.parent, true);
            _arrow.gameObject.SetActive(true);

            _arrow.transform.SetParent(null);
            Rigidbody rb = _arrow.GetComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.AddForce((Target - transform.position) * 4.5f, ForceMode.Impulse);
        }

        public void Idle()
        {
            animator.SetTrigger("idle");
        }

        public void Aim()
        {
            animator.SetTrigger("aim");
            sfxAim.Play();
        }

        public void Release()
        {
            animator.SetTrigger("shoot");
            sfxRelease.Play();
            Shoot();
        }
    }
}