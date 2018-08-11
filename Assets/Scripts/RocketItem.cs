using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketItem : MonoBehaviour {

    public Rocket RocketPrefab;
    [HideInInspector] public Rocket Rocket;

    // Use this for initialization
    void Start ()
    {

	}
	
	// Update is called once per frame
	void Update ()
    {
		if(Input.GetKeyDown(KeyCode.E))
        {
            if(Rocket != null)
            {
                Fire();
            }
            else
            {
                Activate();
            }
        }
	}

    void Activate()
    {
        Vector3 startPos = transform.position + (transform.up * 2f) + (transform.forward * -0.5f);
        Rocket = Instantiate(RocketPrefab, startPos, transform.rotation) as Rocket;
        Rocket.Target = transform;
    }

    void Fire()
    {
        Rocket.Fire();
        Rocket = null;
    }
}
