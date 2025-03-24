using UnityEngine;
using UnityEngine.Serialization;

namespace RageRunGames.BowArrowController
{
    [CreateAssetMenu(fileName = "BowConfig", menuName = "ScriptableObjects/BowConfig", order = 1)]
    public class BowConfig : ScriptableObject
    {
        public LayerMask TargetLayer;
        [Header("Bow Settings")] [Tooltip("Prefab of the arrow to be shot")]
        public Arrow arrowPrefab;
        public float bowScale = 0.7f;

        [FormerlySerializedAs("emissionColor")]
        [Tooltip("Emission color for the bow material")]
        public bool useEmission;
        [ColorUsage(true, true)]
        public Color bowEmissionColor;
        public Color secondaryColor;

        [Header("Bow Initial References")] [Tooltip("Initial X rotation angle for the lower bow limb when undrawn")]
        public float lowerBowInitialRotationX = 88.35171f;

        [Tooltip("Initial X rotation angle for the upper bow limb when undrawn")]
        public float upperBowInitialRotationX = -90f;

        [Header("Arrow Settings")] [Tooltip("Local position offset for arrow spawn point")]
        public Vector3 arrowSpawnOffset = new Vector3(0f, 0f, 0.305f);

        [Tooltip("Initial rotation of spawned arrows")]
        public Vector3 arrowSpawnRotation = new Vector3(-4f, 0f, 0f);

        [Tooltip("Initial Z position of arrow when nocked")]
        public float arrowInitialLocalPositionZ = -0.42f;

        [Header("Bow Physics")] [Tooltip("Spring force constant for string tension")]
        public float bowSpringConstant = 5000f;

        [Tooltip("Damping factor for string movement")]
        public float bowDampingFactor = 30f;

        [Tooltip("Whether the bow should be affected by gravity")]
        public bool useGravity = true;

        [Header("Bow position and Rotation Settings")] [Tooltip("Bow position when not drawn")]
        public Vector3 bowIdlePosition = new Vector3(0f, 0f, -0.1f);

        [Tooltip("Z-axis rotation when bow is at rest")]
        public float bowIdleRotationZ = -64f;

        [Tooltip("Bow position when fully drawn")]
        public Vector3 bowArrowDrawPosition = new Vector3(0f, 0f, 0.05f);

        [Tooltip("Z-axis rotation when bow is fully drawn")]
        public float bowDrawRotationZ = -27f;

        [Tooltip("Offset position for the bow holder")]
        public Vector3 bowHolderOffset = new Vector3(0.1f, -0.15f, 0.5f);

        [Header("Bow Bending During Draw")]
        [Tooltip("Maximum bend angle for bow limbs during draw")] [FormerlySerializedAs("maxBowCompressionDuringDraw")]
        public bool ignoreBowLimbBend;
        public float maxBowLimbBend = 30f;

        [Tooltip("Time to return to idle position after releasing arrow")]
        public float bowResetTimeAfterDraw = 0.1f;

        [Header("Bow Recoil After Shooting")]
        [FormerlySerializedAs("bowRotationAfterShooting")] [Tooltip("X-axis rotation added after shooting an arrow")]
        public Vector3 bowRecoilRotation;

        [FormerlySerializedAs("bowPositionAfterShooting")] [Tooltip("Position offset after shooting an arrow")]
        public Vector3 bowRecoilPosition = new Vector3(0f, 0f, 0.125f);

        [FormerlySerializedAs("bowShootEffectDuration")] [Tooltip("Duration of the visual shoot effect")] [FormerlySerializedAs("afterShotBowEffectDuration")]
        public float bowRecoilDuration = 0.05f;

        [Header("String During Arrow Drawing")] [Tooltip("Maximum distance the string can be pulled back")]
        public float maxStringPullDistance = 0.18f;

        [Tooltip("Initial Y position of the string endpoints")]
        public float stringInitialYPosition = 0.65f;

        [Tooltip("Final Y position of string endpoints when drawn")]
        public float stringFinalYPosition = 0.4f;

        [Tooltip("Final Z position of string endpoints when drawn")]
        public float stringFinalZPosition = -0.15f;

        [Tooltip("Smoothing time for string movement")]
        public float stringDisplacementSmoothTime = 0.2f;

        [Header("Shooting Settings")] [Tooltip("Base speed of launched arrows")] [FormerlySerializedAs("arrowSpeed")]
        public float arrowLaunchSpeed = 15f;

        [Tooltip("Number of arrows to shoot simultaneously")]
        public int numberOfArrows = 1;

        [Tooltip("Angle between multiple arrows (in degrees) when spawn")]
        [Range(0, 60)]public float angleBetweenArrows = 10f;

        [Tooltip("Offset between multiple arrows when they are shot and hit the target.")]
        public float offsetMultiplier = 1f;


        [Tooltip("Pattern for arranging multiple arrows")] [FormerlySerializedAs("shapePattern")]
        public ShapePattern arrowPattern = ShapePattern.LinearVertical;

        [Tooltip("Firing sequence type for multiple arrows")]
        public ShootingType shootingType = ShootingType.Parallel;

        [Tooltip("Delay between shots in sequential firing modes")]
        public float shootingDelay = 0.1f;

        [Header("Wave Shooting Settings")] [Tooltip("Frequency of sine wave pattern for arrows")]
        public float waveFrequency = 5f;

        [Header("Multi-Shot Settings")] [Tooltip("Number of arrows per burst in multi-shot mode")]
        public int multiShotCount = 5;

        [Header("IK Settings")] 
        public bool updateLeftHandIKPosition;
        public Vector3 leftHandIKPositionOffset;
        public Vector3 leftHandIKEulerAngles;
    }

    public enum ShootingType
    {
        Parallel,
        Sequential,
        Random,
        Alternating,
        ForwardAndReverse,
        SineWave,
        Mirror,
        MultiShots
    }

    public enum ShapePattern
    {
        LinearVertical,
        LinearHorizontal,
        Circle,
        Square,
        Triangle,
        Star,
        RandomCluster
    }
}