using Leopotam.EcsLite;
using UnityEngine;

namespace EcsBattle
{
    public class UnitsMoveSystem : IEcsRunSystem
    {
        public void Run(EcsSystems systems)
        {
            var world = systems.GetWorld();
            var filter = world.Filter<Unit>().Inc<TransformRef>().End();
            var units = world.GetPool<Unit>();
            var transforms = world.GetPool<TransformRef>();
            
            float dt = Time.deltaTime;
            
            foreach (int entity in filter)
            {
                ref var unit = ref units.Get(entity);
                if (unit.IsStopped) continue;
                
                ref var tr = ref transforms.Get(entity);
                bool shouldStop = false;
                
                var enemyFilter = world.Filter<Unit>().Inc<TransformRef>().End();
                foreach (int enemyEntity in enemyFilter)
                {
                    if (enemyEntity == entity) continue;
                    
                    ref var enemy = ref units.Get(enemyEntity);
                    if (enemy.Team == unit.Team) continue;
                    
                    ref var enemyTr = ref transforms.Get(enemyEntity);
                    float dist = Vector3.Distance(tr.Transform.position, enemyTr.Transform.position);
                    
                    if (dist < unit.StopDistance)
                    {
                        shouldStop = true;
                        break;
                    }
                }
                
                if (shouldStop)
                {
                    unit.IsStopped = true;
                }
                else
                {
                    tr.Transform.position += tr.Transform.forward * unit.Speed * dt;
                }
            }
        }
    }
}