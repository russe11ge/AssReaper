using System;
using System.Collections;
using System.Collections.Generic;
using RageRunGames.BowArrowController.RageRunGames.BowArrowController;
using UnityEngine;
using UnityEngine.Serialization;

namespace RageRunGames.BowArrowController
{
    public class Bow : MonoBehaviour
    {
        [Header("Bow Configuration")]
        [SerializeField] private BowConfig bowConfig;

        [SerializeField] private Transform lowerBow;
        [SerializeField] private Transform upperBow;
        [SerializeField] private Transform arrowSpawnPoint;

        [Header("Visual Handling")]
        [SerializeField] private SkinnedMeshRenderer bowRenderer;
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private int bowEmissionMaterialIndex;

        [Header("IK Handling")]
        [SerializeField] private Transform rightHandTarget;
        [SerializeField] private Transform leftHandTarget;
        [SerializeField] private Transform ikHandTargetHolder;

        [Header("Sound Effects")]
        [SerializeField] private AudioClip drawSound;
        [SerializeField] private AudioClip shootSound;
        private AudioSource audioSource;

        private float currentLowerBowCompression;
        private float initialLowerBowLocalEularAnglesX;
        private float initialUpperBowLocalEularAnglesX;
        private float currentUpperBowCompression;

        private float initialHandTargetPosZ;
        private float stringDisplacement;
        private float stringVelocity;
        private bool isStringReleased = false;

        private Vector3 initialBowLocalEularAngles;
        private Transform bowHolder;

        private List<Arrow> arrows = new List<Arrow>();
        private ArrowSpawner _arrowSpawner;
        private ArrowShooter _arrowShooter;

        public bool IsExecuting { get; private set; }
        public bool IsShooting { get; private set; }

        private float refVel;

        private void Start()
        {
            lineRenderer = GetComponentInChildren<LineRenderer>();
            bowRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
            InitializeBowSettings();

            _arrowSpawner = new ArrowSpawner(bowConfig, arrowSpawnPoint);
            _arrowShooter = new ArrowShooter(bowConfig, Camera.main);

            // 初始化 AudioSource
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
            }
        }

        private void OnEnable()
        {
            InitializeBowSettings();
        }

        private void InitializeBowSettings()
        {
            transform.localScale = Vector3.one * bowConfig.bowScale;

            if (ikHandTargetHolder == null)
            {
                ikHandTargetHolder = transform.parent.Find("IK Target Holder");
                leftHandTarget = ikHandTargetHolder.Find("Left Hand IK_target");
                rightHandTarget = ikHandTargetHolder.Find("Right Hand IK_target");
            }

            ikHandTargetHolder.parent = transform;
            ikHandTargetHolder.localPosition = Vector3.zero;
            ikHandTargetHolder.localRotation = Quaternion.identity;

            initialLowerBowLocalEularAnglesX = bowConfig.lowerBowInitialRotationX;
            initialUpperBowLocalEularAnglesX = bowConfig.upperBowInitialRotationX;

            bowHolder = transform.parent;
            bowHolder.localPosition = bowConfig.bowHolderOffset;
            arrowSpawnPoint.localPosition = bowConfig.arrowSpawnOffset;
            arrowSpawnPoint.localEulerAngles = bowConfig.arrowSpawnRotation;

            transform.localEulerAngles = new Vector3(0, 0, bowConfig.bowIdleRotationZ);
            transform.localPosition = bowConfig.bowIdlePosition;

            initialBowLocalEularAngles = transform.localEulerAngles;

            rightHandTarget.localPosition = new Vector3(rightHandTarget.localPosition.x, rightHandTarget.localPosition.y, -0.7f);

            if (bowConfig.useEmission)
            {
                bowRenderer.materials[bowEmissionMaterialIndex].EnableKeyword("_EMISSION");
                bowRenderer.materials[bowEmissionMaterialIndex].SetColor("_EmissionColor", bowConfig.bowEmissionColor);
                lineRenderer.material.EnableKeyword("_EMISSION");
                lineRenderer.material.SetColor("_EmissionColor", bowConfig.bowEmissionColor);
            }
            else
            {
                bowRenderer.materials[bowEmissionMaterialIndex].DisableKeyword("_EMISSION");
                bowRenderer.materials[bowEmissionMaterialIndex].SetColor("_Color", bowConfig.secondaryColor);
                lineRenderer.material.DisableKeyword("_EMISSION");
                lineRenderer.material.SetColor("_Color", bowConfig.secondaryColor);
            }

            if (bowConfig.updateLeftHandIKPosition)
            {
                leftHandTarget.localPosition = bowConfig.leftHandIKPositionOffset;
                leftHandTarget.localEulerAngles = bowConfig.leftHandIKEulerAngles;
            }
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0) && !IsExecuting)
            {
                IsExecuting = true;
                DestroyArrows();
                arrows = _arrowSpawner.SpawnArrows();

                // 播放拉弓音效
                if (drawSound != null) audioSource.PlayOneShot(drawSound);

                if (!bowConfig.ignoreBowLimbBend)
                    StartCoroutine(UpdateBowCompression());

                initialHandTargetPosZ = arrows[0].transform.localPosition.z - 0.3f;
            }

            if (Input.GetKey(KeyCode.Mouse0) && !IsShooting)
            {
                isStringReleased = false;
                stringDisplacement = Mathf.SmoothDamp(stringDisplacement, bowConfig.maxStringPullDistance, ref refVel,
                    bowConfig.stringDisplacementSmoothTime);

                UpdateStringPosition();
                UpdateArrowsPosition();
                UpdateBowPosition();
                UpdateBowTilt(stringDisplacement / bowConfig.maxStringPullDistance);

                var rightHandTargetPosition = rightHandTarget.localPosition;
                rightHandTargetPosition.z = Mathf.Lerp(initialHandTargetPosZ, -stringDisplacement - 0.3f,
                    stringDisplacement / bowConfig.maxStringPullDistance);
                rightHandTarget.localPosition = rightHandTargetPosition;

                var bowHolderLocalPosition = bowHolder.localPosition;
                bowHolderLocalPosition.x = Mathf.Lerp(bowHolderLocalPosition.x, -0.03f,
                    stringDisplacement / bowConfig.maxStringPullDistance);
                bowHolderLocalPosition.y = Mathf.Lerp(bowHolderLocalPosition.y, -0.1f,
                    stringDisplacement / bowConfig.maxStringPullDistance);
                bowHolder.localPosition = bowHolderLocalPosition;
            }

            if (Input.GetKeyUp(KeyCode.Mouse0) && !IsShooting)
            {
                IsShooting = true;
                isStringReleased = true;

                // 播放射箭音效
                if (shootSound != null) audioSource.PlayOneShot(shootSound);

                ShootArrows();

                if (!bowConfig.ignoreBowLimbBend)
                    ResetBowCompression();

                StopCoroutine(BowRecoilEffectRoutine());
                StartCoroutine(BowRecoilEffectRoutine());
            }

            if (isStringReleased)
            {
                SimulateSpring();
            }
        }

        private void ResetBowCompression()
        {
            StopCoroutine(UpdateBowCompression());
            var lowerBowRotation = lowerBow.localEulerAngles;
            lowerBowRotation.x = initialLowerBowLocalEularAnglesX;
            lowerBow.localEulerAngles = lowerBowRotation;

            var upperBowRotation = upperBow.localEulerAngles;
            upperBowRotation.x = initialUpperBowLocalEularAnglesX;
            upperBow.localEulerAngles = upperBowRotation;
        }

        private void DestroyArrows()
        {
            if (arrows.Count == 0) return;

            for (int i = 0; i < arrows.Count; i++)
            {
                if (arrows[i] != null)
                {
                    Destroy(arrows[i].gameObject);
                }
            }
        }

        private void UpdateBowPosition()
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, bowConfig.bowArrowDrawPosition,
                stringDisplacement / bowConfig.maxStringPullDistance);
        }

        private void UpdateBowTilt(float t)
        {
            float tiltZ = Mathf.Lerp(bowConfig.bowIdleRotationZ, bowConfig.bowDrawRotationZ, t);
            transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y, tiltZ);
        }

        private void ResetBowTilt()
        {
            StopCoroutine(ResetBowTiltRoutine());
            StartCoroutine(ResetBowTiltRoutine());
        }

        private IEnumerator ResetBowTiltRoutine()
        {
            if (bowConfig.bowResetTimeAfterDraw <= 0) yield break;

            float elapsed = 0f;
            while (elapsed < bowConfig.bowResetTimeAfterDraw)
            {
                float tiltZ = Mathf.Lerp(bowConfig.bowDrawRotationZ, bowConfig.bowIdleRotationZ,
                    elapsed / bowConfig.bowResetTimeAfterDraw);

                transform.localRotation = Quaternion.Euler(0f, transform.localEulerAngles.y, tiltZ);
                transform.localPosition = Vector3.Lerp(transform.localPosition, bowConfig.bowIdlePosition,
                    elapsed / bowConfig.bowResetTimeAfterDraw);
                bowHolder.localPosition = Vector3.Lerp(bowHolder.localPosition, bowConfig.bowHolderOffset,
                    elapsed / bowConfig.bowResetTimeAfterDraw);
                rightHandTarget.localPosition = Vector3.Lerp(rightHandTarget.localPosition,
                    new Vector3(rightHandTarget.localPosition.x, rightHandTarget.localPosition.y, -0.7f),
                    elapsed / bowConfig.bowResetTimeAfterDraw);

                elapsed += Time.deltaTime;
                yield return null;
            }

            IsExecuting = false;
        }

        private IEnumerator UpdateBowCompression()
        {
            float t = 0;
            float duration = bowConfig.stringDisplacementSmoothTime;

            float finalLowerBowCompression = initialLowerBowLocalEularAnglesX + bowConfig.maxBowLimbBend;
            float finalUpperBowCompression = initialUpperBowLocalEularAnglesX - bowConfig.maxBowLimbBend;

            while (t < duration)
            {
                t += Time.deltaTime;
                float lowerX = Mathf.Lerp(initialLowerBowLocalEularAnglesX, finalLowerBowCompression, t / duration);
                float upperX = Mathf.Lerp(initialUpperBowLocalEularAnglesX, finalUpperBowCompression, t / duration);

                lowerBow.localRotation = Quaternion.Lerp(
                    lowerBow.localRotation,
                    Quaternion.Euler(lowerX, lowerBow.localRotation.eulerAngles.y,
                        lowerBow.localRotation.eulerAngles.z),
                    t / duration
                );

                upperBow.localRotation = Quaternion.Lerp(
                    upperBow.localRotation,
                    Quaternion.Euler(upperX, upperBow.localRotation.eulerAngles.y,
                        upperBow.localRotation.eulerAngles.z),
                    t / duration
                );
                yield return null;
            }
        }

        private void UpdateArrowsPosition()
        {
            for (int i = 0; i < arrows.Count; i++)
            {
                if (arrows[i] != null)
                {
                    Vector3 arrowPosition = arrows[i].transform.localPosition;
                    arrowPosition.z = bowConfig.arrowInitialLocalPositionZ - stringDisplacement;
                    arrows[i].transform.localPosition = arrowPosition;
                }
            }
        }

        private void SimulateSpring()
        {
            float springForce = -bowConfig.bowSpringConstant * stringDisplacement;
            float dampingForce = -bowConfig.bowDampingFactor * stringVelocity;
            stringVelocity += (springForce + dampingForce) * Time.deltaTime;
            stringDisplacement += stringVelocity * Time.deltaTime;

            UpdateStringPosition();

            if (Mathf.Abs(stringDisplacement) < 0.01f && Mathf.Abs(stringVelocity) < 0.01f)
            {
                isStringReleased = false;
                stringDisplacement = 0f;
                stringVelocity = 0f;
                UpdateStringPosition();
            }
        }

        private void UpdateStringPosition()
        {
            Vector3 lineRendererPos1 = lineRenderer.GetPosition(1);
            lineRenderer.SetPosition(1, new Vector3(lineRendererPos1.x, lineRendererPos1.y, -stringDisplacement));

            Vector3 lerpPosLineRenderer2 = Vector3.Lerp(new Vector3(0f, -bowConfig.stringInitialYPosition, 0f),
                new Vector3(0, -bowConfig.stringFinalYPosition, bowConfig.stringFinalZPosition),
                stringDisplacement / bowConfig.maxStringPullDistance);
            lineRenderer.SetPosition(2, lerpPosLineRenderer2);

            Vector3 lerpPosLineRenderer0 = Vector3.Lerp(new Vector3(0f, bowConfig.stringInitialYPosition, 0f),
                new Vector3(0, bowConfig.stringFinalYPosition, bowConfig.stringFinalZPosition),
                stringDisplacement / bowConfig.maxStringPullDistance);
            lineRenderer.SetPosition(0, lerpPosLineRenderer0);
        }

        private IEnumerator BowRecoilEffectRoutine()
        {
            if (bowConfig.bowRecoilDuration <= 0) yield break;

            Vector3 originalRotation = transform.localEulerAngles;
            Vector3 originalPosition = transform.localPosition;
            Vector3 recoilRotation = bowConfig.bowRecoilRotation;

            float elapsed = 0f;

            while (elapsed < bowConfig.bowRecoilDuration)
            {
                elapsed += Time.deltaTime;
                float interpolation = (-Mathf.Pow(elapsed, 2) + elapsed * 4f);
                transform.localPosition = Vector3.Lerp(originalPosition, bowConfig.bowRecoilPosition, interpolation);
                transform.localEulerAngles = Vector3.Lerp(originalRotation, recoilRotation, interpolation);
                yield return null;
            }

            elapsed = 0f;

            while (elapsed < 0.1f)
            {
                elapsed += Time.deltaTime;
                transform.localPosition =
                    Vector3.Lerp(bowConfig.bowRecoilPosition, bowConfig.bowIdlePosition, elapsed / 0.1f);
                transform.localEulerAngles = Vector3.Lerp(recoilRotation, initialBowLocalEularAngles, elapsed / 0.1f);
                yield return null;
            }
        }

        private void ShootArrows()
        {
            StartCoroutine(ShootArrowsRoutine());
        }

        private IEnumerator ShootArrowsRoutine()
        {
            float pullStrength = Mathf.Clamp01(stringDisplacement / bowConfig.maxStringPullDistance);
            float arrowShootSpeed = pullStrength * bowConfig.arrowLaunchSpeed;

            yield return StartCoroutine(_arrowShooter.ShootArrowsRoutine(arrows, arrowShootSpeed));

            IsShooting = false;
            arrows.Clear();
            ResetBowTilt();
        }
    }
}