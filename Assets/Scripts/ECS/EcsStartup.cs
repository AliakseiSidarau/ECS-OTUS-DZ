using Leopotam.EcsLite;
using UnityEngine;

namespace EcsBattle
{
    public class EcsStartup : MonoBehaviour
    {
        private EcsWorld _world;
        private EcsSystems _systems;
        
        [Header("Prefabs")]
        [SerializeField] private GameObject redUnitPrefab;
        [SerializeField] private GameObject blueUnitPrefab;
        
        [Header("Config")]
        [SerializeField] private int unitsPerSide = 48;
        [SerializeField] private float lineLength = 20f;

        private void Start()
        {
            InitEcs();
            Spawn();
        }

        private void Update()
        {
            _systems?.Run();
        }

        private void OnDestroy()
        {
            _systems?.Destroy();
            _world?.Destroy();
        }
        
        private void InitEcs()
        {
            _world = new EcsWorld();

            _systems = new EcsSystems(_world)
                .Add(new UnitsMoveSystem())
                .Add(new UnitsShootSystem())
                .Add(new BulletsMoveSystem())
                .Add(new BulletsHitSystem());

            _systems.Init();
        }

        private void Spawn()
        {
            var spawner = new ArmySpawner(_world);
            spawner.Spawn(unitsPerSide, lineLength, redUnitPrefab, blueUnitPrefab);
        }
    }
}