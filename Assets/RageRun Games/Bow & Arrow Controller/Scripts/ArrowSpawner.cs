using System.Collections.Generic;
using UnityEngine;

namespace RageRunGames.BowArrowController
{
    public class ArrowSpawner
    {
        private BowConfig _bowConfig;
        private Transform _arrowSpawnPoint;

        public ArrowSpawner(BowConfig bowConfig, Transform arrowSpawnPoint)
        {
            _bowConfig = bowConfig;
            _arrowSpawnPoint = arrowSpawnPoint;
        }

        public List<Arrow> SpawnArrows()
        {
            List<Arrow> arrows = new List<Arrow>();

            for (int i = 0; i < _bowConfig.numberOfArrows; i++)
            {
                Quaternion rotation = GetArrowRotation(i);
                Arrow arrow = Object.Instantiate(_bowConfig.arrowPrefab, _arrowSpawnPoint.position,
                    _arrowSpawnPoint.rotation * rotation);
                arrows.Add(arrow);

                if (_bowConfig.useEmission)
                {
                    arrow.UpdateArrowEmissionColors(_bowConfig.bowEmissionColor);
                }
                else
                {
                    arrow.UpdateArrowColors(_bowConfig.secondaryColor);
                }

                arrow.transform.parent = _arrowSpawnPoint;
                arrow.transform.localPosition = Vector3.zero;
            }

            return arrows;
        }

        private Quaternion GetArrowRotation(int index)
        {
            switch (_bowConfig.arrowPattern)
            {
                case ShapePattern.LinearVertical:
                    float desiredAngle = _bowConfig.angleBetweenArrows * (index - (_bowConfig.numberOfArrows - 1) / 2f);
                    return Quaternion.Euler(desiredAngle, 0, 0);

                case ShapePattern.LinearHorizontal:
                    desiredAngle = _bowConfig.angleBetweenArrows * (index - (_bowConfig.numberOfArrows - 1) / 2f);
                    return Quaternion.Euler(0f, desiredAngle, 0);

                case ShapePattern.Circle:
                    float angle = 360f / _bowConfig.numberOfArrows * index;
                    return Quaternion.Euler(Mathf.Sin(angle * Mathf.Deg2Rad) * _bowConfig.angleBetweenArrows,
                        Mathf.Cos(angle * Mathf.Deg2Rad) * _bowConfig.angleBetweenArrows, 0);

                case ShapePattern.Triangle:
                    int triangleIndex = index % 3;
                    int layer = index / 3;
                    float layerMultiplier = layer + 1;
                    float[] basePitchAngles =
                    {
                        -_bowConfig.angleBetweenArrows, -_bowConfig.angleBetweenArrows, _bowConfig.angleBetweenArrows
                    };
                    float[] baseYawAngles =
                    {
                        _bowConfig.angleBetweenArrows, -_bowConfig.angleBetweenArrows,
                        _bowConfig.angleBetweenArrows / 2f
                    };
                    float xRotation = basePitchAngles[triangleIndex] * layerMultiplier;
                    float yRotation = baseYawAngles[triangleIndex] * layerMultiplier;
                    return Quaternion.Euler(xRotation, yRotation, 0);

                case ShapePattern.Square:
                    int side = index % 4;
                    layer = index / 4;
                    layerMultiplier = layer + 1;
                    float basePitch = 0f;
                    float baseYaw = 0f;
                    switch (side)
                    {
                        case 0:
                            basePitch = _bowConfig.angleBetweenArrows;
                            baseYaw = _bowConfig.angleBetweenArrows;
                            break;
                        case 1:
                            basePitch = _bowConfig.angleBetweenArrows;
                            baseYaw = -_bowConfig.angleBetweenArrows;
                            break;
                        case 2:
                            basePitch = -_bowConfig.angleBetweenArrows;
                            baseYaw = -_bowConfig.angleBetweenArrows;
                            break;
                        case 3:
                            basePitch = -_bowConfig.angleBetweenArrows;
                            baseYaw = _bowConfig.angleBetweenArrows;
                            break;
                    }

                    xRotation = basePitch * layerMultiplier;
                    yRotation = baseYaw * layerMultiplier;
                    return Quaternion.Euler(xRotation, yRotation, 0);

                case ShapePattern.Star:
                    float starAngle = (360f / _bowConfig.numberOfArrows) * index;
                    float starRadius = (index % 2 == 0)
                        ? _bowConfig.angleBetweenArrows * 1.5f
                        : _bowConfig.angleBetweenArrows;
                    return Quaternion.Euler(0, starAngle, starRadius);

                case ShapePattern.RandomCluster:
                    float randomPitch = Random.Range(-_bowConfig.angleBetweenArrows, _bowConfig.angleBetweenArrows);
                    float randomYaw = Random.Range(-_bowConfig.angleBetweenArrows, _bowConfig.angleBetweenArrows);
                    return Quaternion.Euler(randomPitch, randomYaw, 0);

                default:
                    return Quaternion.identity;
            }
        }
    }
}