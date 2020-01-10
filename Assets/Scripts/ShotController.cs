using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotController : MonoBehaviour
{

    //Gets the bullet moving
    public void SpawnShot(float speed, float lifeTime)
    {
        GetComponent<Rigidbody2D>().velocity = transform.up * speed;
        StartCoroutine(ShotDeath(lifeTime));
    }

    //Destroys the bullet if it doesn't hit anything in the required time
    IEnumerator ShotDeath(float lifeTime) {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }
}
