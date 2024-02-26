using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private float impulseForce = 10.0f;

    private void OnCollisionEnter(Collision other)
    {
        // if player collides with money play sound and destroy money object
        if (other.gameObject.CompareTag("Obstacle"))
        {
            Rigidbody enemyRigidbody = other.gameObject.GetComponent<Rigidbody>();
            Vector3 awayFromPlayer =  other.gameObject.transform.position - transform.position;

            enemyRigidbody.AddForce(awayFromPlayer * impulseForce, ForceMode.Impulse);
        }
    }
}
