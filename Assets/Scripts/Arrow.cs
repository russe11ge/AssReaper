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

                AmmoTeleportManager.Instance.TeleportAmmoBoxToTarget(other.transform.position);
            }

            Transform current = other.transform;
            bool foundAnimationTag = false;
            bool foundAnimationComponent = false;

            while (current != null)
            {
                Debug.Log("🔍 正在检查: " + current.name);

                if (current.CompareTag("Animation"))
                {
                    foundAnimationTag = true;
                    Debug.Log("✅ 找到 tag 为 Animation 的对象: " + current.name);

                    // ✅ 往当前节点及其父级继续找 Animation 组件
                    Transform searchForAnimation = current;
                    while (searchForAnimation != null)
                    {
                        Animation anim = searchForAnimation.GetComponent<Animation>();
                        if (anim != null)
                        {
                            foundAnimationComponent = true;

                            if (!anim.isPlaying)
                            {
                                anim.Play();
                                Debug.Log("🚂 播放动画成功: " + searchForAnimation.name);
                            }
                            else
                            {
                                Debug.Log("⚠️ 动画已在播放中: " + searchForAnimation.name);
                            }
                            break;
                        }
                        searchForAnimation = searchForAnimation.parent;
                    }

                    if (!foundAnimationComponent)
                    {
                        Debug.LogWarning("⚠️ 找到 tag，但在父级中没发现 Animation 组件！");
                    }

                    break; // 找到 tag 就不继续往上找了
                }

                current = current.parent;
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