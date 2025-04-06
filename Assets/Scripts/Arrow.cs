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

                // ✅ 判断命中的 Target 是否挂在 Fan 上
                bool shouldFreeze = false;
                Transform current = other.transform;
                while (current != null)
                {
                    if (current.CompareTag("Fan"))
                    {
                        shouldFreeze = true;
                        break;
                    }
                    current = current.parent;
                }

                // ✅ 根据是否在风扇上，决定是否 freeze
                AmmoTeleportManager.Instance.TeleportAmmoBoxToTarget(other.transform.position, shouldFreeze);
            }

            // ✅ 播放动画部分逻辑保持原样
            Transform animCheck = other.transform;
            bool foundAnimationTag = false;
            bool foundAnimationComponent = false;

            while (animCheck != null)
            {
                Debug.Log("🔍 正在检查: " + animCheck.name);

                if (animCheck.CompareTag("Animation"))
                {
                    foundAnimationTag = true;
                    Debug.Log("✅ 找到 tag 为 Animation 的对象: " + animCheck.name);

                    Transform search = animCheck;
                    while (search != null)
                    {
                        Animation anim = search.GetComponent<Animation>();
                        if (anim != null)
                        {
                            foundAnimationComponent = true;

                            if (!anim.isPlaying)
                            {
                                anim.Play();
                                Debug.Log("🚂 播放动画成功: " + search.name);
                            }
                            else
                            {
                                Debug.Log("⚠️ 动画已在播放中: " + search.name);
                            }
                            break;
                        }
                        search = search.parent;
                    }

                    if (!foundAnimationComponent)
                    {
                        Debug.LogWarning("⚠️ 找到 tag，但在父级中没发现 Animation 组件！");
                    }

                    break;
                }

                animCheck = animCheck.parent;
            }

            if (!foundAnimationTag)
            {
                Debug.LogWarning("❌ 没找到任何 tag 为 Animation 的对象！");
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