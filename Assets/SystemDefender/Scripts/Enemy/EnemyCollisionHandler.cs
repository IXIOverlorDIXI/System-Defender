using Unity.VisualScripting;
using UnityEngine;

namespace SystemDefender.Scripts.Enemy
{
    public class EnemyCollisionHandler : MonoBehaviour
    {
        private void OnTriggerEnter(Collider __other)
        {
            if(__other.tag.Equals("Player"))
            {
                GetComponentInParent<EnemyBehaviorController>().damage();
            }
            else if(__other.tag.Equals("Bullet"))
            {
                GetComponentInParent<EnemyBehaviorController>().death();
                __other.transform.parent.gameObject.SetActive(false);
            }
        }
    }
}