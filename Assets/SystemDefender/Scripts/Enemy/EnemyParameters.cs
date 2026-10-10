using UnityEngine;

namespace SystemDefender.Scripts.Enemy
{
    [CreateAssetMenu(menuName = "SystemDefender/EnemyParameters")]
    public class EnemyParameters : ScriptableObject
    {
        public float SpawnChance;
        public int SpawnCost;
        public int Speed;
        public int KillValue;
        public float Damage;
        public float GpuDamage;
    }
}