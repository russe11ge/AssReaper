using UnityEngine;


namespace RageRunGames.BowArrowController
{
    public class TargetMover : MonoBehaviour
    {
        [SerializeField] private float frequency;
        [SerializeField] private float amplitude;

        private Vector3 intialPosition;

        public bool moveZ;
        public bool moveY;
        public bool moveX;

        public bool invertDirection;

        private float timer;

        private void Start()
        {
            intialPosition = transform.position;
        }

        private void Update()
        {
            timer += Time.deltaTime;

            var currentPos = transform.position;

            if (moveZ)
            {
                currentPos.z = intialPosition.z +
                               (invertDirection ? -1 : +1) * Mathf.Sin(timer * frequency) * amplitude;
            }

            if (moveY)
            {
                currentPos.y = intialPosition.y +
                               (invertDirection ? -1 : +1) * Mathf.Sin(timer * frequency) * amplitude;
            }

            if (moveX)
            {
                currentPos.x = intialPosition.x +
                               (invertDirection ? -1 : +1) * Mathf.Sin(timer * frequency) * amplitude;
            }

            transform.position = currentPos;
        }
    }
}