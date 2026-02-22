using UnityEngine;

namespace EcsBattle
{
    public enum Team { Red, Blue }
    
    public struct Unit
    {
        public Team Team;
        public int Health;
        public float Speed;
        public float ReloadTimer;
        public float ReloadTime;
        public bool IsStopped;
        public float StopDistance;
        public float AttackRange;
    }
    
    public struct Bullet
    {
        public Team Team;
        public float Speed;
        public float LifeTime;
    }
    
    public struct TransformRef
    {
        public Transform Transform;
    }
}