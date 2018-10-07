using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructoidController : MonoBehaviour {

    Rigidbody destructoidRigidbody;

    [HideInInspector] public int color;
    [HideInInspector] Material desctructoidMaterial;

    public float destructoidForce;
    public float destructoidSize;
    public int destructoidBounces = 0;
    public int destructoidBounceLimit = 50;



    // Use this for initialization
    void Awake() {

        // increment the destructoid count
        GameManager.instance.destructoidCount++;

        destructoidRigidbody = gameObject.GetComponent<Rigidbody>();

        // set the color and material for this destructoid
        desctructoidMaterial = GetComponent<Renderer>().material;
        // randomize the player's initial color
        color = Random.Range(0, 4);
        if (color == 0) {
            desctructoidMaterial.color = Color.green;
            gameObject.layer = LayerMask.NameToLayer("Green");
        }
        if (color == 1) {
            desctructoidMaterial.color = Color.red;
            gameObject.layer = LayerMask.NameToLayer("Red");
        }
        if (color == 2) {
            desctructoidMaterial.color = Color.yellow;
            gameObject.layer = LayerMask.NameToLayer("Yellow");
        }
        if (color == 3) {
            desctructoidMaterial.color = Color.blue;
            gameObject.layer = LayerMask.NameToLayer("Blue");
        }

        // set the direction for this destructoid
        transform.rotation = Random.rotation;

        // shoot the destructoid with a random force
        destructoidForce = Random.Range(500f, 2000f);
        destructoidRigidbody.AddForce(transform.forward * destructoidForce);
    }

    // Starts the destroy coroutine
    public void DestroyDestructoid() {
        IEnumerator destroyDestructoid = DestroyThisDestructoid();
        StartCoroutine(destroyDestructoid);
    }

    // Coroutine that shrinks the destructoid then destroys it
    IEnumerator DestroyThisDestructoid() {
        while (destructoidSize >= 0) {
            destructoidSize -= 45f * Time.deltaTime;
            gameObject.transform.localScale = new Vector3(destructoidSize, destructoidSize, destructoidSize);
            yield return null;
        }
        // decrement the destructoid count
        GameManager.instance.destructoidCount--;
        Destroy(this.gameObject);
    }

    // Method that counts how many times this destructoid has bounced
    private void OnCollisionEnter(Collision collision) {
        //destructoidBounces++;
        // destroy this destructoid after it has bounced the maximum number of times
        //if (destructoidBounces == destructoidBounceLimit) {
          //  DestroyDestructoid();
        //}
    }
}