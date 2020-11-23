using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TFI.MathTek.ID113
{
    public class Arrow : MonoBehaviour
    {
        private bool isHit;
        [SerializeField] AudioSource sfxHit;
        private Game _game;

        private void Awake()
        {
            _game = FindObjectOfType<Game>();
        }

        void OnCollisionEnter(Collision collision)
        {
            if (isHit) return;

            sfxHit.Play();
            isHit = true;
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            Debug.Log("shot: " + collision.collider.gameObject.name);
            string answer = collision.collider.transform.parent.GetComponentInChildren<Text>().text;
            Debug.Log("answer: " + answer);
            GetComponent<BoxCollider>().enabled = false;

            GameManager.Answer = answer;
            _game.CheckAnswer();
        }
    }
}