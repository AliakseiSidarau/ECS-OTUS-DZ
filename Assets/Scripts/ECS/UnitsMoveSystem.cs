using Leopotam.EcsLite;
using UnityEngine;

namespace EcsBattle
{
    public class UnitsMoveSystem : IEcsRunSystem
    {
        public void Run(EcsSystems systems)
        {
            var world = systems.GetWorld();
            
            var filter = world
                .Filter<MoveSpeed>()
                .Inc<View>()
                .Inc<MoveDirection>()
                .Inc<TeamComponent>()
                .Inc<StopDistance>()
                .End();

            var viewPool = world.GetPool<View>();
            var speedPool = world.GetPool<MoveSpeed>();
            var dirPool = world.GetPool<MoveDirection>();
            var teamPool = world.GetPool<TeamComponent>();
            var stopDistancePool = world.GetPool<StopDistance>();
            var stoppedPool = world.GetPool<IsStopped>();

            float dt = Time.deltaTime;

            foreach (int entity in filter)
            {
                if (stoppedPool.Has(entity)) continue;

                ref var view = ref viewPool.Get(entity);
                ref var speed = ref speedPool.Get(entity);
                ref var dir = ref dirPool.Get(entity);
                ref var team = ref teamPool.Get(entity);
                ref var stopDistance = ref stopDistancePool.Get(entity);

                bool shouldStop = false;

                foreach (int enemyEntity in filter)
                {
                    if (enemyEntity == entity) continue;

                    ref var enemyTeam = ref teamPool.Get(enemyEntity);
                    if (enemyTeam.Value == team.Value) continue;

                    ref var enemyView = ref viewPool.Get(enemyEntity);

                    float sqrDist = (enemyView.Transform.position - view.Transform.position).sqrMagnitude;

                    if (sqrDist < stopDistance.Value * stopDistance.Value)
                    {
                        shouldStop = true;
                        break;
                    }
                }

                if (shouldStop)
                {
                    stoppedPool.Add(entity);
                }
                else
                {
                    view.Transform.position += dir.Value * speed.Value * dt;
                }
            }
        }
    }
}