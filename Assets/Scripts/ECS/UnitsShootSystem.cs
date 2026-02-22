using Leopotam.EcsLite;
using UnityEngine;

namespace EcsBattle
{
    public class UnitsShootSystem : IEcsRunSystem
    {
        private GameObject _redBulletPrefab;
        private GameObject _blueBulletPrefab;
        private bool _initialized;

        public void Run(EcsSystems systems)
        {
            if (!_initialized)
            {
                _redBulletPrefab = Resources.Load<GameObject>("RedBulletPrefab");
                _blueBulletPrefab = Resources.Load<GameObject>("BlueBulletPrefab");
                _initialized = true;
            }

            var world = systems.GetWorld();
            var filter = world.Filter<Unit>().Inc<TransformRef>().End();
            var units = world.GetPool<Unit>();
            var transforms = world.GetPool<TransformRef>();

            float dt = Time.deltaTime;

            foreach (int entity in filter)
            {
                ref var unit = ref units.Get(entity);

                unit.ReloadTimer -= dt;
                if (unit.ReloadTimer > 0f)
                    continue;
                unit.ReloadTimer = unit.ReloadTime;
                
                Transform target = null;
                float minDist = unit.AttackRange;

                foreach (int e in filter)
                {
                    if (e == entity) continue;
                    ref var enemy = ref units.Get(e);
                    if (enemy.Team == unit.Team) continue;

                    ref var enemyTr = ref transforms.Get(e);
                    ref var myTr = ref transforms.Get(entity);

                    float dist = Vector3.Distance(myTr.Transform.position, enemyTr.Transform.position);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        target = enemyTr.Transform;
                    }
                }

                if (target == null)
                    continue;
                
                ref var shooterTr = ref transforms.Get(entity);
                Vector3 dir = (target.position - shooterTr.Transform.position).normalized;
                Quaternion rot = Quaternion.LookRotation(dir);
                Vector3 spawnPos = shooterTr.Transform.position + Vector3.up * 0.5f;

                GameObject prefab = unit.Team == Team.Red ? _redBulletPrefab : _blueBulletPrefab;
                if (prefab == null)
                {
                    Debug.LogError("Bullet prefab not found in Resources!");
                    continue;
                }

                GameObject bulletGo = Object.Instantiate(prefab, spawnPos, rot);

                int bulletEntity = world.NewEntity();
                var bulletPool = world.GetPool<Bullet>();
                var bulletTrPool = world.GetPool<TransformRef>();

                ref var bullet = ref bulletPool.Add(bulletEntity);
                bullet.Team = unit.Team;
                bullet.Speed = 30f;
                bullet.LifeTime = 3f;

                ref var bulletTr = ref bulletTrPool.Add(bulletEntity);
                bulletTr.Transform = bulletGo.transform;
            }
        }
    }
}
