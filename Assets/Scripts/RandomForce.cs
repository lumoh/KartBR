using UnityEngine;
using System.Collections;

public class RandomForce : MonoBehaviour
{
    Rigidbody body;
    public float Force = 100f;

	// Use this for initialization
	void Start ()
    {
        body = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            body.AddRelativeForce(new Vector3(Random.Range(10f, Force), Random.Range(10f, Force), Random.Range(10f, Force)));
            body.AddRelativeTorque(new Vector3(Random.Range(10f, Force), Random.Range(10f, Force), Random.Range(10f, Force)));
        }
	}
}
