using System.Collections;
using UnityEngine;

namespace RageRunGames.BowArrowController
{
    [RequireComponent(typeof(CharacterController))]
    public class FPSController : MonoBehaviour
    {
        [Header("Movement Settings")] public float walkSpeed = 5f;
        public float runSpeed = 10f;
        public float crouchSpeed = 2.5f;
        public float jumpHeight = 1.5f;
        public float gravity = -9.81f;
        public float crouchHeight = 1f;
        public float standingHeight = 2f;
        public float crouchTransitionSpeed = 5f;

        [Header("Mouse Look Settings")] public float mouseSensitivity = 100f;
        public Transform cameraTransform;
        public float cameraSmoothness = 10f;

        [Header("Head Bobbing Settings")] public Transform cameraHolder;
        public float idleBobSpeed = 1.5f;
        public float idleBobAmount = 0.02f;
        public float walkBobSpeed = 3.5f;
        public float walkBobAmount = 0.08f;
        public float runBobSpeed = 6.5f;
        public float runBobAmount = 0.15f;
        public float crouchBobSpeed = 2f;
        public float crouchBobAmount = 0.05f;

        [Header("Sway Settings")] public Transform swayTransform;
        public float swayAmount = 0.02f;
        public float maxSwayAmount = 0.06f;
        public float smoothSpeed = 6f;

        [Header("Audio Settings")] public AudioClip[] footstepSounds;
        public float footstepInterval = 0.5f;

        private CharacterController controller;
        private AudioSource audioSource;
        private Vector3 velocity;
        private bool isGrounded;
        private bool isCrouching;
        private bool isSprinting;
        private float xRotation = 0f;
        private float defaultCamPosY;
        private float defaultYPos;
        private float defaultXPos = 0f;
        private float defaultZPos = 0f;
        private float timer;
        private float currentBobSpeed;
        private float currentBobAmount;
        private Vector3 initialSwayTransformPos;

        private Bow bow;

        private void Start()
        {
            bow = GetComponentInChildren<Bow>();

            controller = GetComponent<CharacterController>();
            audioSource = GetComponent<AudioSource>();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            defaultYPos = cameraHolder.localPosition.y;
            defaultXPos = cameraHolder.localPosition.x;
            defaultZPos = cameraHolder.localPosition.z;
            initialSwayTransformPos = swayTransform.localPosition;

            defaultCamPosY = cameraTransform.localPosition.y;
        }

        private void Update()
        {
            HandleMovement();
            HandleMouseLook();
            HandleJump();
            HandleCrouch();
            HandleSprint();
            HandleHeadBobbing();
            HandleSway();
        }


        private void HandleMovement()
        {
            isGrounded = controller.isGrounded;

            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }

            float moveSpeed = isCrouching ? crouchSpeed : (isSprinting ? runSpeed : walkSpeed);
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            Vector3 move = transform.right * x + transform.forward * z;
            controller.Move(move * moveSpeed * Time.deltaTime);

            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }

        private void HandleMouseLook()
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            cameraTransform.localRotation = Quaternion.Lerp(
                cameraTransform.localRotation,
                Quaternion.Euler(xRotation, 0f, 0f),
                cameraSmoothness * Time.deltaTime
            );

            transform.Rotate(Vector3.up * mouseX);
        }

        private void HandleJump()
        {
            if (Input.GetButtonDown("Jump") && isGrounded && !isCrouching)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }

        private void HandleCrouch()
        {
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                isCrouching = !isCrouching;
                StopAllCoroutines();
                StartCoroutine(CrouchTransition());
            }
        }

        private IEnumerator CrouchTransition()
        {
            float targetHeight = isCrouching ? crouchHeight : standingHeight;
            Vector3 targetCameraPos = new Vector3(
                cameraTransform.localPosition.x,
                defaultCamPosY - (standingHeight - targetHeight),
                cameraTransform.localPosition.z
            );

            while (Mathf.Abs(controller.height - targetHeight) > 0.01f)
            {
                controller.height = Mathf.Lerp(controller.height, targetHeight, crouchTransitionSpeed * Time.deltaTime);
                cameraTransform.localPosition = Vector3.Lerp(
                    cameraTransform.localPosition,
                    targetCameraPos,
                    crouchTransitionSpeed * Time.deltaTime
                );
                yield return null;
            }
        }

        private void HandleSprint()
        {
            isSprinting = Input.GetKey(KeyCode.LeftShift) && !isCrouching && Input.GetAxis("Vertical") > 0;
        }


        private void HandleHeadBobbing()
        {
            if (!isGrounded) return;

            float movementMagnitude = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).magnitude;

            // Adjust bobbing speed and amount based on movement state
            if (movementMagnitude > 0.1f)
            {
                currentBobSpeed = isCrouching ? crouchBobSpeed : (isSprinting ? runBobSpeed : walkBobSpeed);
                currentBobAmount = isCrouching ? crouchBobAmount : (isSprinting ? runBobAmount : walkBobAmount);

                // Calculate bobbing offsets
                timer += Time.deltaTime * currentBobSpeed;
                float bobbingOffsetY = Mathf.Sin(timer) * currentBobAmount;
                float bobbingOffsetX = Mathf.Cos(timer * 0.5f) * (currentBobAmount * 0.5f); // Horizontal sway

                // Apply bobbing to camera position
                Vector3 targetPosition = new Vector3(
                    defaultXPos + bobbingOffsetX,
                    defaultYPos + bobbingOffsetY,
                    defaultZPos
                );

                cameraHolder.localPosition =
                    Vector3.Lerp(cameraHolder.localPosition, targetPosition, Time.deltaTime * 10f);

                float tiltAmount = Mathf.Sin(timer) * 2f;

                tiltAmount = Mathf.Clamp(tiltAmount, -2f, 2f);
                Quaternion targetRotation = Quaternion.Euler(
                    cameraHolder.localEulerAngles.x,
                    cameraHolder.localEulerAngles.y,
                    tiltAmount
                );

                cameraHolder.localRotation =
                    Quaternion.Slerp(cameraHolder.localRotation, targetRotation, Time.deltaTime * 5f);
            }
            else
            {
                timer += Time.deltaTime * idleBobSpeed;
                float idleBobbingOffsetY = Mathf.Sin(timer) * idleBobAmount;
                float idleBobbingOffsetX = Mathf.Cos(timer * 0.5f) * (idleBobAmount * 0.3f);

                Vector3 targetPosition = new Vector3(
                    defaultXPos + idleBobbingOffsetX,
                    defaultYPos + idleBobbingOffsetY,
                    defaultZPos
                );

                cameraHolder.localPosition =
                    Vector3.Lerp(cameraHolder.localPosition, targetPosition, Time.deltaTime * 5f);

                // Apply subtle idle rotation
                float idleTiltAmount = Mathf.Sin(timer * 0.5f) * (idleBobAmount * 0.05f);
                Quaternion targetRotation = Quaternion.Euler(
                    cameraHolder.localEulerAngles.x,
                    cameraHolder.localEulerAngles.y,
                    idleTiltAmount
                );

                cameraHolder.localRotation =
                    Quaternion.Slerp(cameraHolder.localRotation, targetRotation, Time.deltaTime * 3f);
            }
        }

        private void HandleSway()
        {
            if (bow.IsExecuting) return;

            float mouseX = Input.GetAxis("Mouse X") * swayAmount;
            float mouseY = Input.GetAxis("Mouse Y") * swayAmount;

            float movementSwayMultiplier = 1f;
            float swayInAir = 0;

            if (isSprinting)
            {
                movementSwayMultiplier = 2f;
            }

            if (!isGrounded)
            {
                swayInAir = 0.1f;
            }

            mouseX *= movementSwayMultiplier;
            mouseY *= movementSwayMultiplier;

            mouseX = Mathf.Clamp(mouseX, -maxSwayAmount, maxSwayAmount) +
                     swayInAir * -Mathf.Sign(controller.velocity.y) * 0.25f;
            mouseY = Mathf.Clamp(mouseY, -maxSwayAmount, maxSwayAmount) + swayInAir * Mathf.Sign(controller.velocity.y);

            Vector3 targetPosition = new Vector3(mouseX, mouseY, 0) + initialSwayTransformPos;
            swayTransform.localPosition =
                Vector3.Lerp(swayTransform.localPosition, targetPosition, smoothSpeed * Time.deltaTime);
        }
    }
}