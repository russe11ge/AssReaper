using UnityEngine;

namespace RageRunGames.BowArrowController
{
    public class MouseRotation : MonoBehaviour
    {
        [SerializeField] private float mouseSensitivity = 100f;
        private float xRotation = 0f;
        private float yRotation = 0f;


        private void Start()
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;
        }

        private void Update()
        {
            HandleRotation();
        }

        private void HandleRotation()
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            yRotation += mouseX;

            transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
        }
    }
}