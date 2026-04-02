using Leopotam.EcsLite;
using UnityEngine;

namespace EcsBattle
{
    public class UnitsShootSystem : IEcsRunSystem, IEcsInitSystem
    {
        private GameObject _redBulletPrefab;
        private GameObject _blueBulletPrefab;
        
        public void Init(EcsSystems systems)
        {
            _redBulletPrefab = Resources.Load<GameObject>("RedBulletPrefab");
            _blueBulletPrefab = Resources.Load<GameObject>("BlueBulletPrefab");

            if (_redBulletPrefab == null || _blueBulletPrefab == null)
            {
                Debug.LogError("Bullet prefabs not found in Resources!");
            }
        }

        public void Run(EcsSystems systems)
        {
            var world = systems.GetWorld();

            var filter = world
                .Filter<ReloadTimer>()
                .Inc<ReloadTime>()
                .Inc<AttackRange>()
                .Inc<TeamComponent>()
                .Inc<View>()
                .End();

            var reloadTimerPool = world.GetPool<ReloadTimer>();
            var reloadTimePool = world.GetPool<ReloadTime>();
            var attackRangePool = world.GetPool<AttackRange>();
            var teamPool = world.GetPool<TeamComponent>();
            var viewPool = world.GetPool<View>();

            float dt = Time.deltaTime;

            foreach (int entity in filter)
            {
                ref var reloadTimer = ref reloadTimerPool.Get(entity);
                ref var reloadTime = ref reloadTimePool.Get(entity);

                reloadTimer.Value -= dt;
                if (reloadTimer.Value > 0f)
                    continue;

                reloadTimer.Value = reloadTime.Value;

                ref var myTeam = ref teamPool.Get(entity);
                ref var myView = ref viewPool.Get(entity);
                ref var attackRange = ref attackRangePool.Get(entity);

                Transform myTransform = myView.Transform;

                Transform target = null;
                float minSqrDist = attackRange.Value * attackRange.Value;

                foreach (int enemyEntity in filter)
                {
                    if (enemyEntity == entity) continue;

                    ref var enemyTeam = ref teamPool.Get(enemyEntity);
                    if (enemyTeam.Value == myTeam.Value) continue;

                    ref var enemyView = ref viewPool.Get(enemyEntity);

                    float sqrDist = (enemyView.Transform.position - myTransform.position).sqrMagnitude;

                    if (sqrDist < minSqrDist)
                    {
                        minSqrDist = sqrDist;
                        target = enemyView.Transform;
                    }
                }

                if (target == null)
                    continue;

                Vector3 dir = (target.position - myTransform.position).normalized;
                Quaternion rot = Quaternion.LookRotation(dir);
                Vector3 spawnPos = myTransform.position + Vector3.up * 0.5f;

                GameObject prefab = myTeam.Value == Team.Red ? _redBulletPrefab : _blueBulletPrefab;
                if (prefab == null) continue;

                GameObject bulletGo = Object.Instantiate(prefab, spawnPos, rot);

                int bulletEntity = world.NewEntity();

                var bulletTagPool = world.GetPool<BulletTag>();
                var bulletViewPool = world.GetPool<View>();
                var bulletSpeedPool = world.GetPool<MoveSpeed>();
                var bulletLifePool = world.GetPool<LifeTime>();
                var bulletTeamPool = world.GetPool<TeamComponent>();
                var bulletDirPool = world.GetPool<MoveDirection>();

                bulletTagPool.Add(bulletEntity);

                ref var bulletView = ref bulletViewPool.Add(bulletEntity);
                bulletView.Transform = bulletGo.transform;

                ref var speed = ref bulletSpeedPool.Add(bulletEntity);
                speed.Value = 30f;

                ref var life = ref bulletLifePool.Add(bulletEntity);
                life.Value = 3f;

                ref var bulletTeam = ref bulletTeamPool.Add(bulletEntity);
                bulletTeam.Value = myTeam.Value;

                ref var bulletDir = ref bulletDirPool.Add(bulletEntity);
                bulletDir.Value = dir;
            }
        }
    }
}