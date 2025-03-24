using UnityEngine;

namespace RageRunGames.BowArrowController
{
    public class Sway : MonoBehaviour
    {
        [Header("Sway Settings")] public float swayAmount = 0.02f;
        public float maxSwayAmount = 0.06f;
        public float smoothSpeed = 6f;

        private Vector3 initialPosition;

        private void Start()
        {
            initialPosition = transform.localPosition;
        }

        private void Update()
        {
            HandleSway();
        }

        private void HandleSway()
        {
            float mouseX = Input.GetAxis("Mouse X") * swayAmount;
            float mouseY = Input.GetAxis("Mouse Y") * swayAmount;

            mouseX = Mathf.Clamp(mouseX, -maxSwayAmount, maxSwayAmount);
            mouseY = Mathf.Clamp(mouseY, -maxSwayAmount, maxSwayAmount);

            Vector3 targetPosition = new Vector3(mouseX, mouseY, 0) + initialPosition;
            transform.localPosition =
                Vector3.Lerp(transform.localPosition, targetPosition, smoothSpeed * Time.deltaTime);
        }
    }
}