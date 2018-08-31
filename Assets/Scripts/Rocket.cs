using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : Photon.PunBehaviour
{
    public float Lifetime = 3.0f;
    public Vector3 Offset;
    public float DistanceDamp;
    public float AngleDamp;
    public float Speed;
    public ParticleSystem Explosion;
    public float ExplosionRadius = 5f;
    public float ExplosionForce = 10f;
    [HideInInspector] public Transform Target;

    private Vector3 velocity;
    private Vector3 angle;
    private Rigidbody rb;
    private Collider col;
    private KartRoot kart;

    private bool fired;

    private void Start()
    {
        col = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
        fired = false;
        transform.localScale = Vector3.one * 0.125f;
    }
    void FixedUpdate()
    {
        if (!fired)
        {
            followSmoothDamp();
        }
        else
        {
            Vector3 force = (Vector3.forward * Speed) + (Vector3.right * kart.turnInput * 100f);
            Vector3 torque = (Vector3.up * kart.turnInput * 0.5f);
            rb.AddRelativeForce(force);
            rb.AddRelativeTorque(torque);
        }
    }

    void followSmoothDamp()
    {
        if (Target != null)
        {
            Vector3 toPos = Target.position + (Target.rotation * Offset);
            Vector3 curPos = Vector3.SmoothDamp(transform.position, toPos, ref velocity, DistanceDamp);
            transform.position = curPos;

            Vector3 curRot = Vector3.SmoothDamp(transform.rotation.eulerAngles, Target.eulerAngles, ref angle, AngleDamp);
            transform.rotation = Quaternion.Euler(curRot);
        }
    }

    public void Fire()
    {
        col.enabled = true;
        fired = true;
        rb.useGravity = true;
        kart = Target.GetComponent<KartRoot>();
        rb.velocity = kart.Body.velocity;
        Destroy(gameObject, Lifetime);
    }

    private void explode()
    {

    }

    void OnCollisionEnter(Collision collision)
    {
        PhotonNetwork.Destroy(gameObject);
        //Destroy(gameObject);
    }

    private void OnDestroy()
    {
        ParticleSystem ps = Instantiate(Explosion, transform.position, transform.rotation);
        Destroy(ps.gameObject, ps.duration);

        RaycastHit[] hits = Physics.SphereCastAll(new Ray(transform.position, transform.forward), ExplosionRadius);
        foreach(RaycastHit hit in hits)
        {
            if(hit.rigidbody != null)
            {
                Vector3 direction = transform.position - hit.transform.position;
                direction.y = 3f;
                Vector3 explosionForce = direction * (1f - (hit.distance / ExplosionRadius)) * ExplosionForce;
                Debug.Log(explosionForce.ToString());
                hit.rigidbody.AddForce(explosionForce, ForceMode.Impulse);
            }
        }
    }
}