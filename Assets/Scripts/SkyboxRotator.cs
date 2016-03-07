using UnityEngine;
using System.Collections;

public class SkyboxRotator : MonoBehaviour {

    private int roll;
    private float angularSpeed;

    void Start ()
    {
        roll = Random.Range(0, 2);
        angularSpeed = (roll < 1) ? -1f : 1f;
    }

	void Update ()
    {
        transform.Rotate(Vector3.forward, angularSpeed * Time.deltaTime);
	}
}
