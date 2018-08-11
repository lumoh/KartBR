using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class KartRoot : Photon.PunBehaviour, IPunObservable
{
    public static GameObject LocalPlayerInstance;

    public KartUI KartUIPrefab;
    private KartUI kartUI;

    [Header("Attributes")]
    public Rigidbody Body;
    public float Gravity = 10f;
    public float Speed = 90f;
    public float GroundDrag = 5f;
    public float TurnDrag = 10f;
    public float TurnSpeed = 5f;
    public float SuspensionForce;
    public float SuspensionHeight;
    public List<Suspension> Suspensions;
    public List<GameObject> Wheels;
    public ParticleSystem[] dustTrails = new ParticleSystem[2];

    [Header("Debug")]
    public bool GizmosOn;

    private Vector3 localVel;
    private bool reverse;
    private float powerInput;
    [HideInInspector] public float turnInput;
    private bool slideInput;
    int layerMask;

    void Awake()
    {
        if (photonView.isMine)
        {
            LocalPlayerInstance = gameObject;

            if (Camera.main != null)
            {
                FollowCamera cam = Camera.main.gameObject.GetComponent<FollowCamera>();
                cam.Target = transform;
            }
        }

        Body = GetComponent<Rigidbody>();
        Body.centerOfMass = Vector3.down;

        layerMask = 1 << LayerMask.NameToLayer("Kart");
        layerMask = ~layerMask;

        if(KartUIPrefab != null)
        {
            kartUI = Instantiate(KartUIPrefab);
        }

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void initGUIControls()
    {
            kartUI.GoBtn.onButtonDown.AddListener(() =>
            {
                powerInput = 1.0f;
            });
        kartUI.GoBtn.onButtonUp.AddListener(() =>
            {
                powerInput = 0f;
            });
        kartUI.FireBtn.onButtonDown.AddListener(() =>
            {

            });
    }


    void Update()
    {
        if (photonView.isMine)
        {
            powerInput = Input.GetAxis("Vertical");
            turnInput = Input.GetAxis("Horizontal");
            slideInput = Input.GetKey(KeyCode.Space);

            if (isGrounded())
            {
                Body.drag = GroundDrag;
            }
            else
            {
                Body.drag = 0;
                powerInput = 0;
            }
        }

        dustTrail();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(this.slideInput);
        }
        else
        {
            // Network player, receive data
            this.slideInput = (bool)stream.ReceiveNext();
        }
    }

    void turnWheels()
    {
        for (int i = 0; i < Wheels.Count; i++)
        {
            GameObject wheel = Wheels[i];
            wheel.transform.RotateAround(wheel.transform.position, wheel.transform.right, localVel.z);
        }
    }

    void FixedUpdate()
    {
        if(!photonView.isMine)
        {
            return;
        }

        localVel = transform.InverseTransformDirection(Body.velocity);
        reverse = false;
        if(localVel.z < -1f)
        {
            reverse = true;
        }

        turnAndThrust();
        turnWheels();
    }

    void dustTrail()
    {
        float emissionRate = 0f;
        if (photonView.isMine)
        {
            if (isGrounded() && this.slideInput)
            {
                Vector3 localVel = transform.InverseTransformDirection(Body.velocity);
                if (localVel.z > 5f)
                {
                    emissionRate = 5;
                }
            }
        }
        else
        {
            if (this.slideInput)
            {
                emissionRate = 100f;
            }
        }

        for (int i = 0; i < dustTrails.Length; i++)
        {
            var emission = dustTrails[i].emission;
            emission.rateOverDistance = new ParticleSystem.MinMaxCurve(emissionRate);
        }
    }

    void turnAndThrust()
    {
        if (isGrounded())
        {
            Vector3 normal = transform.forward;
            foreach (Suspension s in Suspensions)
            {
                if (s.IsGrouded)
                {
                    normal = s.Normal;
                    break;
                }
            }

            Vector3 localVel = transform.InverseTransformDirection(Body.velocity);
            float forwardForce = powerInput * Speed;
            float turnDrag = -localVel.x * TurnDrag;
            if (slideInput)
            {
                forwardForce *= 1.25f;
                turnDrag = 0;
            }
            
            if(reverse)
            {
                turnInput = -turnInput;
            } 

            Vector3 sideVector = Vector3.Cross(normal, transform.forward);
            Vector3 forwardVector = -Vector3.Cross(normal, sideVector).normalized;

            // forward force 
            Body.AddForce(forwardVector * forwardForce);
            // turn torque
            Body.AddRelativeTorque(0f, turnInput * TurnSpeed, 0);
            // turn drag
            Body.AddRelativeForce(new Vector3(turnDrag, 0, 0));
        }
    }

    bool isGrounded()
    {
        foreach (Suspension s in Suspensions)
        {
            if (s.IsGrouded)
            {
                return true;
            }
        }
        return false;
    }

    public void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode loadingMode)
    {
        this.CalledOnLevelWasLoaded(scene.buildIndex);
    }

    void CalledOnLevelWasLoaded(int level)
    {
        // nothing
    }

    void OnDrawGizmos()
    {
        if (GizmosOn)
        {
            if (isGrounded())
            {
                Vector3 normal = transform.forward;
                foreach (Suspension s in Suspensions)
                {
                    if (s.IsGrouded)
                    {
                        normal = s.Normal;
                        break;
                    }
                }

                Vector3 sideVector = Vector3.Cross(normal, transform.forward);
                Vector3 forwardVector = -Vector3.Cross(normal, sideVector);

                // forward vector along plane
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(transform.position, transform.position + forwardVector * 3f);
            }

            // forward vector of kart
            // Gizmos.color = Color.blue;
            // Gizmos.DrawLine(transform.position, transform.position + transform.forward * 3f);

            // vector straight down from kart
            // Vector3 origin = transform.position + (transform.up * 0.5f);
            // Gizmos.DrawLine(origin, origin + (Vector3.down * (SuspensionHeight + 0.5f)));

#if UNITY_EDITOR
            // velocity
            Vector3 localVel = transform.InverseTransformDirection(Body.velocity);
            Handles.Label(transform.position + transform.up * 2f, localVel.ToString());
#endif
        }
    }
}