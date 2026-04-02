using Leopotam.EcsLite;
using UnityEngine;

namespace EcsBattle
{
    public class BulletsMoveSystem : IEcsRunSystem
    {
        public void Run(EcsSystems systems)
        {
            var world = systems.GetWorld();

            var filter = world
                .Filter<BulletTag>()
                .Inc<View>()
                .Inc<MoveSpeed>()
                .Inc<LifeTime>()
                .Inc<MoveDirection>()
                .End();

            var viewPool = world.GetPool<View>();
            var speedPool = world.GetPool<MoveSpeed>();
            var lifePool = world.GetPool<LifeTime>();
            var dirPool = world.GetPool<MoveDirection>();

            float dt = Time.deltaTime;

            foreach (int entity in filter)
            {
                ref var view = ref viewPool.Get(entity);
                ref var speed = ref speedPool.Get(entity);
                ref var life = ref lifePool.Get(entity);
                ref var dir = ref dirPool.Get(entity);

                life.Value -= dt;

                if (life.Value <= 0f)
                {
                    Object.Destroy(view.Transform.gameObject);
                    world.DelEntity(entity);
                    continue;
                }

                view.Transform.position += dir.Value * speed.Value * dt;
            }
        }
    }
}