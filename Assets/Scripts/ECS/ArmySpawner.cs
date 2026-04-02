using Leopotam.EcsLite;
using UnityEngine;

namespace EcsBattle
{
    public class ArmySpawner
    {
        private readonly EcsWorld _world;

        public ArmySpawner(EcsWorld world)
        {
            _world = world;
        }

        public void Spawn(int unitsPerSide, float lineLength, GameObject redPrefab, GameObject bluePrefab)
        {
            float halfLine = lineLength * 0.5f;
            int unitsPerRow = unitsPerSide / 3;

            float[] redZ = { -18f, -15f, -12f };
            float[] blueZ = { 12f, 15f, 18f };

            for (int row = 0; row < 3; row++)
            {
                for (int i = 0; i < unitsPerRow; i++)
                {
                    float t = (float)i / (unitsPerRow - 1);
                    float x = Mathf.Lerp(-halfLine, halfLine, t);

                    SpawnUnit(new Vector3(x, 0, redZ[row]), Team.Red, Vector3.forward, redPrefab);
                    SpawnUnit(new Vector3(x, 0, blueZ[row]), Team.Blue, Vector3.back, bluePrefab);
                }
            }
        }

        private void SpawnUnit(Vector3 position, Team team, Vector3 forward, GameObject prefab)
        {
            var go = Object.Instantiate(prefab, position, Quaternion.LookRotation(forward));
            go.name = $"{team}Unit";

            int entity = _world.NewEntity();

            AddView(entity, go.transform);
            AddTeam(entity, team);
            AddHealth(entity);
            AddMovement(entity, team);
            AddAttack(entity);
            AddStopLogic(entity);
        }
        
        private void AddView(int entity, Transform transform)
        {
            var pool = _world.GetPool<View>();
            ref var view = ref pool.Add(entity);
            view.Transform = transform;
        }

        private void AddTeam(int entity, Team team)
        {
            var pool = _world.GetPool<TeamComponent>();
            ref var comp = ref pool.Add(entity);
            comp.Value = team;
        }

        private void AddHealth(int entity)
        {
            var pool = _world.GetPool<Health>();
            ref var health = ref pool.Add(entity);
            health.Value = 3;
        }

        private void AddMovement(int entity, Team team)
        {
            var speedPool = _world.GetPool<MoveSpeed>();
            ref var speed = ref speedPool.Add(entity);
            speed.Value = 2f;

            var dirPool = _world.GetPool<MoveDirection>();
            ref var dir = ref dirPool.Add(entity);
            dir.Value = team == Team.Red ? Vector3.forward : Vector3.back;
        }

        private void AddAttack(int entity)
        {
            var reloadTimePool = _world.GetPool<ReloadTime>();
            ref var reloadTime = ref reloadTimePool.Add(entity);
            reloadTime.Value = 0.5f;

            var reloadTimerPool = _world.GetPool<ReloadTimer>();
            ref var reloadTimer = ref reloadTimerPool.Add(entity);
            reloadTimer.Value = Random.Range(0f, 0.5f);

            var attackRangePool = _world.GetPool<AttackRange>();
            ref var range = ref attackRangePool.Add(entity);
            range.Value = 10f;
        }

        private void AddStopLogic(int entity)
        {
            var pool = _world.GetPool<StopDistance>();
            ref var stop = ref pool.Add(entity);
            stop.Value = 1f;
        }
    }
}