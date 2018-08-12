using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketItem : MonoBehaviour {

    public Rocket RocketPrefab;
    [HideInInspector] public Rocket Rocket;
    [HideInInspector] public KartRoot kart;

    // Use this for initialization
    void Start ()
    {
        kart = GetComponent<KartRoot>();
        if (kart.photonView.isMine)
        {
            if (kart.UseGUIControls)
            {
                kart.kartUI.FireBtn.onButtonDown.AddListener(() =>
                {
                    if (Rocket != null)
                    {
                        Fire();
                    }
                    else
                    {
                        Activate();
                    }
                });
            }
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (kart.photonView.isMine)
        {
            if (!kart.UseGUIControls)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (Rocket != null)
                    {
                        Fire();
                    }
                    else
                    {
                        Activate();
                    }
                }
            }
        }
	}

    void Activate()
    {
        Vector3 startPos = transform.position + (transform.up * 2f) + (transform.forward * -0.5f);
        GameObject rocketObj = PhotonNetwork.Instantiate(RocketPrefab.name, startPos, transform.rotation, 0);
        Rocket = rocketObj.GetComponent<Rocket>();
        Rocket.Target = transform;
    }

    void Fire()
    {
        Rocket.Fire();
        Rocket = null;
    }
}
