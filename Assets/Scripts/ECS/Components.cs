using UnityEngine;

namespace EcsBattle
{
    public enum Team { Red, Blue }
    
    public struct AttackRange
    {
        public float Value;
    }

    public struct TeamComponent
    {
        public Team Value;
    }
    
    public struct Health
    {
        public int Value;
    }
    
    public struct MoveSpeed
    {
        public float Value;
    }

    public struct ReloadTimer
    {
        public float Value;
    }

    public struct ReloadTime
    {
        public float Value;
    }

    public struct IsStopped
    {
        
    }

    public struct StopDistance
    {
        public float Value;
    }
    
    public struct BulletTag { }

    public struct LifeTime
    {
        public float Value;
    }

    public struct Damage
    {
        public int Value;
    }
    
    public struct TransformRef
    {
        public Transform Transform;
    }

    public struct View
    {
        public Transform Transform;
    }
    
    public struct MoveDirection
    {
        public Vector3 Value;
    }
}