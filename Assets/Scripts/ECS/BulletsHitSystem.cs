using Leopotam.EcsLite;
using UnityEngine;

namespace EcsBattle
{
    public class BulletsHitSystem : IEcsRunSystem
    {
        public void Run(EcsSystems systems)
        {
            var world = systems.GetWorld();
            
            var bulletFilter = world.Filter<Bullet>().Inc<TransformRef>().End();
            var unitFilter = world.Filter<Unit>().Inc<TransformRef>().End();
            
            var bullets = world.GetPool<Bullet>();
            var bulletTrs = world.GetPool<TransformRef>();
            var units = world.GetPool<Unit>();
            var unitTrs = world.GetPool<TransformRef>();
            
            const float hitRadius = 0.6f;
            
            foreach (int bEntity in bulletFilter)
            {
                ref var bullet = ref bullets.Get(bEntity);
                ref var bTr = ref bulletTrs.Get(bEntity);
                
                bool hit = false;
                
                foreach (int uEntity in unitFilter)
                {
                    ref var unit = ref units.Get(uEntity);
                    if (unit.Team == bullet.Team) continue;
                    
                    ref var uTr = ref unitTrs.Get(uEntity);
                    
                    float sqrDist = (uTr.Transform.position - bTr.Transform.position).sqrMagnitude;
                    if (sqrDist <= hitRadius * hitRadius)
                    {
                        unit.Health -= 1;
                        
                        if (unit.Health <= 0)
                        {
                            Object.Destroy(uTr.Transform.gameObject);
                            world.DelEntity(uEntity);
                        }
                        
                        hit = true;
                        break;
                    }
                }
                
                if (hit)
                {
                    Object.Destroy(bTr.Transform.gameObject);
                    world.DelEntity(bEntity);
                }
            }
        }
    }
}
