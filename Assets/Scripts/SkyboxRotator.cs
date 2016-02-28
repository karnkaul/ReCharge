using UnityEngine;
using System.Collections;

public class SkyboxRotator : MonoBehaviour {

	void Update ()
    {
        transform.Rotate(Vector3.forward, 0.01f);
	}
}
