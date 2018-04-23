using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleScript : MonoBehaviour {
    public GameObject explosion;

    public float radius = 25.0f;
    public float power = 100.0f;

    private void OnTriggerEnter(Collider other)
    {
        StartCoroutine(Explode());
    }

    IEnumerator Explode()
    {
        yield return new WaitForSeconds(0.5f);
        GameObject explosionInstantiate = Instantiate(explosion, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - 0.5f, gameObject.transform.position.z), Quaternion.identity);

        Vector3 explosionPos = explosionInstantiate.transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            if (rb != null)
                rb.AddExplosionForce(power, explosionPos, radius, 1.0f);
        }

        Destroy(gameObject, 1.0f);
    }
}
