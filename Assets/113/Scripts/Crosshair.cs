using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TFI.MathTek.ID113
{
    public class Crosshair : MonoBehaviour
    {
        [SerializeField] Canvas canvas;
        [SerializeField] Transform cameraTransform;
        [SerializeField] Bow bow;
        [SerializeField] private GameObject _predictiveLine;

        public float speedH = 1f;
        public float speedV = 1f;

        private float yaw = 0f;
        private float pitch = 0f;

        private float pixelX;
        private float pixelY;
        private float posX;
        private float posY;

        private string animation = "";
        private Game _game;

        private void Awake()
        {
            _game = FindObjectOfType<Game>();
        }
        private void Start()
        {
            Debug.Log(canvas.worldCamera);
            pixelX = Camera.main.pixelWidth;
            pixelY = Camera.main.pixelHeight;
        }

        private void Update()
        {
            if (EndGame) return;

            if (Input.GetMouseButtonDown(0) && _game.IsReady && !EventSystem.current.IsPointerOverGameObject())
            {
                if (animation == "aim")
                    return;

                animation = "aim";
                bow.Aim();

                if (!_predictiveLine.activeSelf)
                    _predictiveLine.SetActive(true);
            }

            if (Input.GetMouseButton(0))
            {
                yaw += speedH * Input.GetAxis("Mouse X");
                pitch -= speedV * Input.GetAxis("Mouse Y");

                yaw = Mathf.Clamp(yaw, -20f, 20f);
                pitch = Mathf.Clamp(pitch, -15f, 15f);
                cameraTransform.eulerAngles = new Vector3(pitch, yaw, 0.0f);

                CheckRay();
            }

            if (Input.GetMouseButtonUp(0) && _game.IsReady && !EventSystem.current.IsPointerOverGameObject())
            {
                if (animation == "release" || FindObjectOfType<Bow>().Target == Vector3.zero) return;

                animation = "release";
                bow.Release();

                if (_predictiveLine.activeSelf)
                    _predictiveLine.SetActive(false);
            }
        }

        void CheckRay()
        {
            posX = transform.localPosition.x + pixelX / 2f;
            posY = transform.localPosition.y + pixelY / 2f;
            Vector3 cursorPos = new Vector3(posX, posY, 0);
            //OR- Ray ray = Camera.main.ScreenPointToRay(new Vector3(posX, posY, 0));
            Ray ray = Camera.main.ScreenPointToRay(cursorPos);
            Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null)
                {
                    //Debug.Log("hit: " + hit.collider.gameObject.name);
                    if (hit.collider.gameObject.name == "Boundary")
                        FindObjectOfType<Bow>().Target = Vector3.zero;
                    else
                    {
                        FindObjectOfType<Bow>().Target = hit.point;
                    }                    
                }
            }
        }

        public bool EndGame { set; get; }
    }
}