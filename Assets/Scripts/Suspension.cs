using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Suspension : MonoBehaviour
{
    public KartRoot KartRoot;
    public bool IsGrouded = false;
    public Vector3 Point;
    public Vector3 Normal;
    public float Ratio;

    int layerMask;

    void Awake()
    {
        layerMask = 1 << LayerMask.NameToLayer("Kart");
        layerMask = ~layerMask;
    }

    void Update()
    {

    }

    void FixedUpdate()
    {
        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, KartRoot.SuspensionHeight, layerMask))
        {
            Ratio = 1.0f - (hit.distance / KartRoot.SuspensionHeight);
            Vector3 appliedForce = Vector3.up * KartRoot.SuspensionForce * Ratio;
            KartRoot.Body.AddForceAtPosition(appliedForce, transform.position);
            Point = hit.point;
            Normal = hit.normal;

            IsGrouded = true;
        }
        else
        {
            IsGrouded = false;
            Point = transform.position - (transform.up * KartRoot.SuspensionHeight);
        }

        if (KartRoot.transform.position.y > transform.position.y)
        {
            KartRoot.Body.AddForceAtPosition(transform.up * KartRoot.Gravity, transform.position);
        }
        else
        {
            // Apply gravity force
            KartRoot.Body.AddForceAtPosition(-transform.up * KartRoot.Gravity, transform.position);
        }
    }

    void OnDrawGizmos()
    {
        if (KartRoot.GizmosOn)
        {
            if (IsGrouded)
            {
                Gizmos.color = Color.green;
            }
            else
            {
                Gizmos.color = Color.red;
            }

            // show suspension contact
            Gizmos.DrawLine(transform.position, Point);
            Gizmos.DrawSphere(Point, 0.1f);

#if UNITY_EDITOR
            Handles.Label(Point, Ratio.ToString());
#endif
        }
    }
}