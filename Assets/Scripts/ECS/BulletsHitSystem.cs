using Leopotam.EcsLite;
using UnityEngine;

namespace EcsBattle
{
    public class BulletsHitSystem : IEcsRunSystem
    {
        public void Run(EcsSystems systems)
        {
            var world = systems.GetWorld();

            var bulletFilter = world
                .Filter<BulletTag>()
                .Inc<View>()
                .Inc<TeamComponent>()
                .End();

            var unitFilter = world
                .Filter<Health>()
                .Inc<View>()
                .Inc<TeamComponent>()
                .End();

            var bulletViewPool = world.GetPool<View>();
            var bulletTeamPool = world.GetPool<TeamComponent>();

            var unitViewPool = world.GetPool<View>();
            var unitTeamPool = world.GetPool<TeamComponent>();
            var unitHealthPool = world.GetPool<Health>();

            const float hitRadius = 0.6f;
            float hitRadiusSqr = hitRadius * hitRadius;

            foreach (int bEntity in bulletFilter)
            {
                ref var bView = ref bulletViewPool.Get(bEntity);
                ref var bTeam = ref bulletTeamPool.Get(bEntity);

                bool hit = false;

                foreach (int uEntity in unitFilter)
                {
                    ref var uTeam = ref unitTeamPool.Get(uEntity);

                    if (uTeam.Value == bTeam.Value)
                        continue;

                    ref var uView = ref unitViewPool.Get(uEntity);

                    float sqrDist = (uView.Transform.position - bView.Transform.position).sqrMagnitude;

                    if (sqrDist <= hitRadiusSqr)
                    {
                        ref var health = ref unitHealthPool.Get(uEntity);
                        health.Value -= 1;

                        if (health.Value <= 0)
                        {
                            Object.Destroy(uView.Transform.gameObject);
                            world.DelEntity(uEntity);
                        }

                        hit = true;
                        break;
                    }
                }

                if (hit)
                {
                    Object.Destroy(bView.Transform.gameObject);
                    world.DelEntity(bEntity);
                }
            }
        }
    }
}