using System.Collections.Generic;
using System.Linq;
using SystemDefender.Scripts.Core;
using SystemDefender.Scripts.Core.Structures;
using SystemDefender.Scripts.Player;
using SystemDefender.Scripts.Services.Abstractions;
using UnityEngine;

namespace SystemDefender.Scripts.Ground
{
    public class GroundController : MonoBehaviourSinglton<GroundController>
    {
        [SerializeField]
        private GameObject groundUnitPrefab;

        [SerializeField]
        private List<GroundUnitController> _groundUnits = new List<GroundUnitController>();
        private readonly List<Pair<int, int>> _groundUnitsCoordinates = new List<Pair<int, int>>();

        private Transform _playerTransform = null;

        [SerializeField]
        private int _spawnDistance = 2;
        [SerializeField]
        private int _despawnDistance = 4;
        [SerializeField]
        private int _unitDistanceModifier = 1;

        private void Start()
        {
            _playerTransform = PlayerController.Instance.transform;

            if(_playerTransform is null)
            {
                Debug.LogError("Player controller does not exist");
            }

            Pair<int, int> defaultPair = new Pair<int, int>()
            {
                firstItem = 0,
                secondItem = 0,
            };

            _groundUnitsCoordinates.Add(defaultPair);

            _groundUnits.First().position = defaultPair;

            FixedUpdate();
        }


        private void FixedUpdate()
        {
            List<GroundUnitController> groundUnitsToDestroy = new List<GroundUnitController>();
            List<GroundUnitController> groundUnitControllersToRegister = new List<GroundUnitController>();

            int deltaDistance = _spawnDistance * _unitDistanceModifier;

            Pair<int, int> playerCoordinates = new Pair<int, int>()
            {
                firstItem = Mathf.FloorToInt(_playerTransform.position.x / 100),
                secondItem = Mathf.FloorToInt(_playerTransform.position.z / 100)
            };


            foreach (GroundUnitController groundUnitController in _groundUnits)
            {
                float a = groundUnitController.position.Distance(playerCoordinates);
                if (a > _despawnDistance)
                {
                    groundUnitsToDestroy.Add(groundUnitController);
                    _groundUnitsCoordinates.Remove(groundUnitController.position);
                }
            }

            foreach (GroundUnitController groundUnitController in groundUnitsToDestroy)
            {
                _groundUnits.Remove(groundUnitController);
                Destroy(groundUnitController.gameObject);
            }

            int xMin = playerCoordinates.firstItem - _spawnDistance;
            int yMin = playerCoordinates.secondItem - _spawnDistance;

            int limit = _spawnDistance * 2 + 1;

            for (int i = 0; i < limit; ++i)
            {
                for (int j = 0; j < limit; ++j)
                {
                    Pair<int, int> newPair = new Pair<int, int>()
                    {
                        firstItem = xMin + i,
                        secondItem = yMin + j,
                    };


                    if (_groundUnitsCoordinates.Any(
                        x => x.firstItem == newPair.firstItem
                        && x.secondItem == newPair.secondItem))
                    {
                        continue;
                    }

                    GameObject newGroundUnit = Instantiate(
                        groundUnitPrefab,
                        new Vector3
                        {
                            x = newPair.firstItem * _unitDistanceModifier,
                            y = transform.position.y,
                            z = newPair.secondItem * _unitDistanceModifier,
                        },
                        Quaternion.identity,
                        transform);
                    GroundUnitController newGroundUnitController = newGroundUnit.GetComponent<GroundUnitController>();

                    newGroundUnitController.position = newPair;

                    _groundUnits.Add(newGroundUnitController);
                    _groundUnitsCoordinates.Add(newPair);
                }
            }

        }

        public Vector3? getSpawnLocation(float __minDistance, float __maxDistance)
        {
            List<GroundUnitController> validTiles = new List<GroundUnitController>();

            foreach (GroundUnitController tile in _groundUnits)
            {
                float distanceToPlayer = Vector3.Distance(_playerTransform.position, tile.transform.position);

                if (distanceToPlayer >= __minDistance && distanceToPlayer <= __maxDistance)
                {
                    validTiles.Add(tile);
                }
            }

            if (validTiles.Count == 0)
            {
                Debug.LogWarning("No tiles available");
                return null;
            }

            GroundUnitController selectedTileForSpawn = validTiles[Random.Range(0, validTiles.Count)];

            Collider tileCollider = selectedTileForSpawn.GetComponentInChildren<MeshCollider>();

            if (tileCollider == null)
            {
                Debug.LogError("Tile has not collider\n" + selectedTileForSpawn.name);

                return null;
            }

            Bounds bounds = tileCollider.bounds;

            Vector3 randomPosition = new Vector3(
                    Random.Range(bounds.min.x, bounds.max.x),
                    bounds.max.y,
                    Random.Range(bounds.min.z, bounds.max.z));

            return randomPosition;
        }
    }
}