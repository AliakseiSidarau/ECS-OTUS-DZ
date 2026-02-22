using Leopotam.EcsLite;
using UnityEngine;

namespace EcsBattle
{
    public class BulletsMoveSystem : IEcsRunSystem
    {
        public void Run(EcsSystems systems)
        {
            var world = systems.GetWorld();
            var filter = world.Filter<Bullet>().Inc<TransformRef>().End();
            var bullets = world.GetPool<Bullet>();
            var transforms = world.GetPool<TransformRef>();
            
            float dt = Time.deltaTime;
            
            foreach (int entity in filter)
            {
                ref var bullet = ref bullets.Get(entity);
                ref var tr = ref transforms.Get(entity);
                
                bullet.LifeTime -= dt;
                if (bullet.LifeTime <= 0f)
                {
                    Object.Destroy(tr.Transform.gameObject);
                    world.DelEntity(entity);
                    continue;
                }
                
                tr.Transform.position += tr.Transform.forward * bullet.Speed * dt;
            }
        }
    }
}