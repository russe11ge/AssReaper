using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RageRunGames.BowArrowController.RageRunGames.BowArrowController
{
    public class ArrowShooter
    {
        private BowConfig _bowConfig;
        private Camera _mainCam;
        private List<Arrow> _arrows;

        public ArrowShooter(BowConfig bowConfig, Camera mainCam)
        {
            _bowConfig = bowConfig;
            _mainCam = mainCam;
        }

        public IEnumerator ShootArrowsRoutine(List<Arrow> arrows, float arrowShootSpeed)
        {
            _arrows = arrows;

            switch (_bowConfig.shootingType)
            {
                case ShootingType.Parallel:
                    yield return ShootParallel(arrowShootSpeed);
                    break;
                case ShootingType.Sequential:
                    yield return ShootSequential(arrowShootSpeed);
                    break;
                case ShootingType.Random:
                    yield return ShootRandom(arrowShootSpeed);
                    break;
                case ShootingType.Alternating:
                    yield return ShootAlternating(arrowShootSpeed);
                    break;
                case ShootingType.ForwardAndReverse:
                    yield return ShootForwardAndReverse(arrowShootSpeed);
                    break;
                case ShootingType.SineWave:
                    yield return ShootSineWave(arrowShootSpeed);
                    break;
                case ShootingType.Mirror:
                    yield return ShootMirror(arrowShootSpeed);
                    break;
                case ShootingType.MultiShots:
                    yield return ShootMultiShots(arrowShootSpeed);
                    break;
            }

            _arrows.Clear();
        }

        private IEnumerator ShootParallel(float arrowShootSpeed)
        {
            for (int i = 0; i < _arrows.Count; i++)
            {
                if (_arrows[i] != null)
                {
                    HandleShooting(_arrows[i], arrowShootSpeed);
                }
            }
            yield return null;
        }

        private IEnumerator ShootSequential(float arrowShootSpeed)
        {
            ResetArrowPositions();
            for (int i = 0; i < _arrows.Count; i++)
            {
                if (_arrows[i] != null)
                {
                    HandleShooting(_arrows[i], arrowShootSpeed);
                    yield return new WaitForSeconds(_bowConfig.shootingDelay);
                }
            }
        }

        private IEnumerator ShootRandom(float arrowShootSpeed)
        {
            ResetArrowPositions();
            List<int> indices = Enumerable.Range(0, _arrows.Count).ToList();
            indices = indices.OrderBy(x => Random.value).ToList(); // Shuffle indices

            for (int i = 0; i < indices.Count; i++)
            {
                int randomIndex = indices[i];
                if (_arrows[randomIndex] != null)
                {
                    HandleShooting(_arrows[randomIndex], arrowShootSpeed);
                    yield return new WaitForSeconds(_bowConfig.shootingDelay);
                }
            }
        }

        private IEnumerator ShootAlternating(float arrowShootSpeed)
        {
            ResetArrowPositions();
            for (int i = 0; i < _arrows.Count; i++)
            {
                _arrows[i].transform.Rotate(0, (i % 2 == 0 ? _bowConfig.angleBetweenArrows : -_bowConfig.angleBetweenArrows), 0);
                HandleShooting(_arrows[i], arrowShootSpeed);
                yield return new WaitForSeconds(_bowConfig.shootingDelay);
            }
        }

        private IEnumerator ShootForwardAndReverse(float arrowShootSpeed)
        {
            ResetArrowPositions();
            for (int i = 0; i < _arrows.Count / 2; i++)
            {
                if (_arrows[i] != null)
                {
                    HandleShooting(_arrows[i], arrowShootSpeed);
                    yield return new WaitForSeconds(_bowConfig.shootingDelay);
                }
            }

            yield return new WaitForSeconds(_bowConfig.shootingDelay);
            for (int i = _arrows.Count - 1; i >= _arrows.Count / 2; i--)
            {
                if (_arrows[i] != null)
                {
                    HandleShooting(_arrows[i], arrowShootSpeed);
                    yield return new WaitForSeconds(_bowConfig.shootingDelay);
                }
            }
        }

        private IEnumerator ShootSineWave(float arrowShootSpeed)
        {
            ResetArrowPositions();
            for (int i = 0; i < _arrows.Count; i++)
            {
                float xRotation = Mathf.Sin(i * _bowConfig.waveFrequency) * _bowConfig.angleBetweenArrows;
                float yRotation = Mathf.Cos(i * _bowConfig.waveFrequency) * _bowConfig.angleBetweenArrows;
                _arrows[i].transform.Rotate(xRotation, yRotation, 0);
                HandleShooting(_arrows[i], arrowShootSpeed);
                yield return new WaitForSeconds(_bowConfig.shootingDelay);
            }
        }

        private IEnumerator ShootMirror(float arrowShootSpeed)
        {
            ResetArrowPositions();
            for (int i = 0; i < Mathf.CeilToInt(_arrows.Count / 2f); i++)
            {
                if (i < _arrows.Count)
                {
                    _arrows[i].transform.Rotate(0, _bowConfig.angleBetweenArrows, 0);
                    HandleShooting(_arrows[i], arrowShootSpeed);
                }

                // Shoot right side arrow
                int mirroredIndex = _arrows.Count - 1 - i;
                if (mirroredIndex > i)
                {
                    _arrows[mirroredIndex].transform.Rotate(0, -_bowConfig.angleBetweenArrows, 0);
                    HandleShooting(_arrows[mirroredIndex], arrowShootSpeed);
                }

                yield return new WaitForSeconds(_bowConfig.shootingDelay);
            }
        }

        private IEnumerator ShootMultiShots(float arrowShootSpeed)
        {
            ResetArrowPositions();
            for (int i = 0; i < _arrows.Count; i += _bowConfig.multiShotCount)
            {
                for (int j = 0; j < _bowConfig.multiShotCount && (i + j) < _arrows.Count; j++)
                {
                    _arrows[i + j].transform.Rotate(0, (j - 1) * _bowConfig.angleBetweenArrows, 0);
                    HandleShooting(_arrows[i + j], arrowShootSpeed);
                }

                yield return new WaitForSeconds(_bowConfig.shootingDelay);
            }
        }

        private void ResetArrowPositions()
        {
            for (int i = 0; i < _arrows.Count; i++)
            {
                if (_arrows[i] != null)
                {
                    var localPosition = _arrows[i].transform.localPosition;
                    localPosition.z = _bowConfig.arrowInitialLocalPositionZ;
                    _arrows[i].transform.localPosition = localPosition;
                }
            }
        }

        private void HandleShooting(Arrow arrow, float arrowShootSpeed)
        {
            if (arrow == null) return;

            Vector3 initialLocalPosition = arrow.transform.TransformDirection(arrow.transform.localPosition);
            arrow.transform.parent = null;

            Vector3 direction = _mainCam.transform.forward;

            if (Physics.Raycast(_mainCam.transform.position, _mainCam.transform.forward, out RaycastHit hit, 1000f,
                    _bowConfig.TargetLayer))
            {
                Vector3 worldOffset = initialLocalPosition;
                worldOffset.Normalize();
                Vector3 adjustedTarget = hit.point + (worldOffset * _bowConfig.offsetMultiplier);
                direction = (adjustedTarget - arrow.transform.position).normalized;
            }
            else
            {
                direction = arrow.transform.forward;
            }

            arrow.Shoot(_bowConfig, direction, arrowShootSpeed);
            arrow.transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}