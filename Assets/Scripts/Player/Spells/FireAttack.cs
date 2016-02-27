//using UnityEngine;
//using System.Collections;

//public class FireAttack : UnityEngine.Object
//{
//    public enum SpellState { Charging, Activating, Idle }

//    // how fast should column of fire extend from players arms
//    public float fireColumnSpeed = 5f;

//    public float lightDropDelay = .15f;
//    public float lightDisableThresholdDistance = 2f;
//    public float lightFadeSpeedMultiplier = 3f;
//    public float maxLightIntensity = 6f;

//    // fireBallLeft and fireBallRight are particle systems that player holds in hand to charge up spell
//    public GameObject fireBallLeft;
//    public GameObject fireBallRight;

//    // fireBeam is fire column particle system prefab used for main spell effect
//    public GameObject fireBeam;
//    public GameObject lightColumn;

//    // flameLight is prefab for point lights that are dropped as flame column moves forward
//    public GameObject flameLight;

//    private SpellState m_SpellState = SpellState.Idle;
//    public SpellState State { get { return m_SpellState; } }

//    private Vector3 m_SpellActivatePosition;
//    private Vector3 m_SpellVelocity;
//    private GameObject m_ActiveFireBeam;
//    private float m_TimeBeforeAddLight;

//    // Player
//    private GameObject m_PlayerGameObject;
//    private Transform m_PlayerTransform;

//    // Use this for initialization
//    void Awake ()
//    {
//        m_PlayerGameObject = GameObject.FindGameObjectWithTag(Tags.player);
//        m_PlayerTransform = m_PlayerGameObject.GetComponent<Transform>();
//    }

//    void Update()
//    {
//        CheckDestroyLights();

//        switch (m_SpellState)
//        {
//            case SpellState.Charging:
//                break;
//            case SpellState.Activating:

//                // Keep moving fire beam particle system in direction of spell
//                m_ActiveFireBeam.transform.Translate(m_SpellVelocity * fireColumnSpeed * Time.deltaTime);

//                // Drop a point light every so often to illuminate scene
//                if (Time.time - m_TimeBeforeAddLight >= lightDropDelay)
//                {
//                    AddFlameLight(m_ActiveFireBeam.transform.position/* + new Vector3(0f, -1f, 0f)*/);
//                }

//                // Check if any lights need to be removed
//                CheckDestroyLights();

//                break;
//            case SpellState.Idle:
//                break;
//            default:
//                break;
//        }
//    }

//    void SetFireBallsActive(bool active)
//    {
//        fireBallLeft.SetActive(active);
//        fireBallRight.SetActive(active);
//    }

//    void AddFlameColumn()
//    {
//        // Get the average position of the 2 charging fireballs to decide where to start the flame column
//        m_SpellActivatePosition = (fireBallLeft.transform.position + fireBallRight.transform.position) / 2f;
//        m_SpellVelocity = m_PlayerTransform.forward;
//        m_ActiveFireBeam = GameObject.Instantiate(fireBeam, m_SpellActivatePosition, Quaternion.identity) as GameObject;
//        m_ActiveFireBeam.SetActive(true);
//        m_TimeBeforeAddLight = Time.time;
//    }

//    void AddFlameLight(Vector3 position)
//    {
//        GameObject flameLightInstance = GameObject.Instantiate(flameLight, position, Quaternion.identity) as GameObject;
//        flameLightInstance.transform.parent = lightColumn.transform;
//        m_TimeBeforeAddLight = Time.time;
//    }

//    void CheckDestroyLights()
//    {
//        // if we have an active fire beam, then compare its position against the light source,
//        // otherwise activeFireBeamTransform is null and all light sources should be disabled
//        Transform activeFireBeamTransform = m_ActiveFireBeam ?
//                                             m_ActiveFireBeam.transform as Transform :
//                                             null;

//        float lightDisableThresholdSqrd = Mathf.Pow(lightDisableThresholdDistance, 2f);
//        Transform lightColumnTransform = lightColumn.transform;
//        foreach (Transform child in lightColumnTransform)
//        {
//            // destroy light if either no active fire beam, or it is not within range anymore,
//            // using squared distance to see if light is within threshold distance
//            Light flameLight = child.GetComponent<Light>();
//            if (!activeFireBeamTransform ||
//                Vector3.SqrMagnitude(activeFireBeamTransform.transform.position - child.position) > lightDisableThresholdSqrd)
//            {
//                // Keep reducing a lights intensity until it has faded out, before destroying completely
//                flameLight.intensity -= Time.deltaTime * lightFadeSpeedMultiplier;
//                if (flameLight.intensity <= 0f)
//                {
//                    GameObject.Destroy(child.gameObject);
//                }
//            }
//            else if (flameLight.intensity < maxLightIntensity)
//            {
//                // if light is in range keep fading it in until reach max intensity
//                flameLight.intensity += Time.deltaTime * lightFadeSpeedMultiplier;
//            }
//        }
//    }

//    void RemoveSpell()
//    {
//        // Destroy with a delay
//        GameObject.Destroy(m_ActiveFireBeam, 1f);
//        m_ActiveFireBeam = null;

//        CheckDestroyLights();
//    }
//}
