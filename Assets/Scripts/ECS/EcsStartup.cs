using Leopotam.EcsLite;
using UnityEngine;

namespace EcsBattle
{
    public class EcsStartup : MonoBehaviour
    {
        private EcsWorld _world;
        private EcsSystems _systems;
        
        [Header("Prefabs")]
        public GameObject RedUnitPrefab;
        public GameObject BlueUnitPrefab;
        public GameObject RedBulletPrefab;
        public GameObject BlueBulletPrefab;
        
        [Header("Config")]
        public int UnitsPerSide = 48;
        public float LineLength = 20f;

        void Start()
        {
            _world = new EcsWorld();
            
            _systems = new EcsSystems(_world);
            _systems
                .Add(new UnitsMoveSystem())
                .Add(new UnitsShootSystem())
                .Add(new BulletsMoveSystem())
                .Add(new BulletsHitSystem());
            _systems.Init();
            
            SpawnArmies();
        }

        void Update()
        {
            _systems?.Run();
        }
        
        void OnDestroy()
        {
            _systems?.Destroy();
            _world?.Destroy();
        }

        void SpawnArmies()
        {
            float halfLine = LineLength * 0.5f;
            float unitsPerRow = UnitsPerSide / 3f;
            float[] redZs = { -18f, -15f, -12f };
            float[] blueZs = { 12f, 15f, 18f };

            for (int row = 0; row < 3; row++)
            {
                for (int i = 0; i < unitsPerRow; i++)
                {
                    if (i >= unitsPerRow - 1) continue;
                    
                    float t = (float)i / (unitsPerRow - 1);
                    float x = Mathf.Lerp(-halfLine, halfLine, t);
                    
                    Vector3 redPos = new Vector3(x, 0, redZs[row]);
                    SpawnUnit(redPos, Team.Red, Quaternion.LookRotation(Vector3.forward), RedUnitPrefab);
                    
                    Vector3 bluePos = new Vector3(x, 0, blueZs[row]);
                    SpawnUnit(bluePos, Team.Blue, Quaternion.LookRotation(Vector3.back), BlueUnitPrefab);
                }
            }
        }

        void SpawnUnit(Vector3 pos, Team team, Quaternion rotation, GameObject prefab)
        {
            var go = Object.Instantiate(prefab, pos, rotation);
            go.name = team == Team.Red ? "RedUnit" : "BlueUnit";
            
            int entity = _world.NewEntity();

            var trPool = _world.GetPool<TransformRef>();
            var unitPool = _world.GetPool<Unit>();

            ref TransformRef tr = ref trPool.Add(entity);
            tr.Transform = go.transform;

            ref Unit unit = ref unitPool.Add(entity);
            unit.Team = team;
            unit.Health = 3;
            unit.Speed = 2f;
            unit.ReloadTime = 0.5f;
            unit.ReloadTimer = Random.Range(0f, 0.5f);
            unit.StopDistance = 1f;
            unit.AttackRange = 10f;
            unit.IsStopped = false;
        }
    }
}
