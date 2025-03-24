using UnityEngine;

namespace RageRunGames.BowArrowController
{
    public class Arrow : MonoBehaviour
    {
        [SerializeField] private TrailRenderer trailRenderer;
        [SerializeField] private MeshRenderer arrowMeshRenderer;

        [SerializeField] private int arrowMaterialIndex;

        [SerializeField] private float lifeTime = 3f;
        private Rigidbody rb;

        private float timer;
        private bool isShot;
        private bool isCollided;

        BowConfig bowConfig;

        private Collider collider;

        private void Awake()
        {
            collider = GetComponent<Collider>();
            
            rb = GetComponent<Rigidbody>();
            timer = lifeTime;
        }

        private void Update()
        {
            if (!isShot) return;

            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                Destroy(gameObject);
            }

            if (isCollided) return;

            if (bowConfig.useGravity && rb.linearVelocity.sqrMagnitude > 0.01f)
            {
                transform.rotation = Quaternion.LookRotation(rb.linearVelocity, Vector3.up);
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.layer == 8)
            {
                isCollided = true;
                rb.isKinematic = true;
            }

            if (other.gameObject.CompareTag("Target"))
            {
                trailRenderer.enabled = false;
                collider.isTrigger = true;
                rb.isKinematic = true;
                transform.parent = other.transform;
                isCollided = true;
            }
        }

        public void Shoot(BowConfig bowConfig, Vector3 direction, float force)
        {
            this.bowConfig = bowConfig;
            rb.isKinematic = false;
            rb.useGravity = bowConfig.useGravity;
            rb.linearVelocity = direction * force;
            isShot = true;
            trailRenderer.gameObject.SetActive(true);
        }

        public void UpdateArrowEmissionColors(Color bowConfigEmissionColor)
        {
            trailRenderer.material.EnableKeyword("_EMISSION");
            trailRenderer.material.SetColor("_EmissionColor", bowConfigEmissionColor);
            
            arrowMeshRenderer.materials[arrowMaterialIndex].EnableKeyword("_EMISSION");
            arrowMeshRenderer.materials[arrowMaterialIndex].SetColor("_EmissionColor", bowConfigEmissionColor);
        }

        public void UpdateArrowColors(Color bowConfigColor)
        {
            trailRenderer.sharedMaterial.DisableKeyword("_EMISSION");
            trailRenderer.material.SetColor("_Color", bowConfigColor);
            
            arrowMeshRenderer.materials[arrowMaterialIndex].DisableKeyword("_EMISSION");
            arrowMeshRenderer.materials[arrowMaterialIndex].SetColor("_Color", bowConfigColor);
        }
    }
}