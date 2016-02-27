using UnityEngine;
using System.Collections;

// spell reference: https://dl.dropboxusercontent.com/u/45747146/WebPlayer/NWParticleFX03/NWParticleFX03.html
public class FireAttack : MonoBehaviour
{
    // The state of the spell is controlled by animation events.
    // This can either be Charging, Activating, or Idle
    public enum SpellState { Charging, Activating, Idle }
    protected SpellState m_SpellState = SpellState.Idle;
    public SpellState State { get { return m_SpellState; } }

    // how fast should column of fire extend from players arms
    public float fireColumnSpeed = 5f;

    // parameters for controlling how the column of fire should behave
    public float lightDropDelay = .15f;
    public float lightDisableThresholdDistance = 2f;
    public float lightFadeSpeedMultiplier = 3f;
    public float maxLightIntensity = 6f;

    // fireBallLeft and fireBallRight are particle systems that player holds in hand to charge up spell
    public GameObject fireBallLeft;
    public GameObject fireBallRight;

    // fireBeam is fire column particle system prefab used for main spell effect
    public GameObject fireBeam;
    public GameObject lightColumn;

    // flameLight is prefab for point lights that are dropped as flame column moves forward
    public GameObject flameLight;

    // Private variables used for tracking fire effect behaviour
    private Vector3 m_SpellActivatePosition;
    private Vector3 m_SpellVelocity;
    private GameObject m_ActiveFireBeam;
    private float m_TimeBeforeAddLight;

    // Player
    private GameObject m_PlayerGameObject;
    private Transform m_PlayerTransform;

    void Awake()
    {
        m_PlayerGameObject = GameObject.FindGameObjectWithTag(Tags.player);
        m_PlayerTransform = m_PlayerGameObject.GetComponent<Transform>();
    }

    void Update()
    {
        CheckDestroyLights();

        switch (m_SpellState)
        {
            case SpellState.Charging:
                break;
            case SpellState.Activating:

                // Keep moving fire beam particle system in direction of spell
                m_ActiveFireBeam.transform.Translate(m_SpellVelocity * fireColumnSpeed * Time.deltaTime);

                // Drop a point light every so often to illuminate scene
                if (Time.time - m_TimeBeforeAddLight >= lightDropDelay)
                {
                    AddFlameLight(m_ActiveFireBeam.transform.position);
                }

                // Check if any lights need to be removed
                CheckDestroyLights();

                break;
            case SpellState.Idle:
                break;
            default:
                break;
        }
    }

    public void OnSpellStateEvent(string eventParameter)
    {
        Debug.Log(eventParameter);

        // Parse state as enum and handle state change
        SpellState state = (SpellState)System.Enum.Parse(typeof(SpellState), eventParameter);
        SetSpellState(state);
    }

    void SetSpellState(SpellState state)
    {
        if (m_SpellState != state)
        {
            m_SpellState = state;
            OnSpellStateChanged(m_SpellState);
        }
    }

    void OnSpellStateChanged(SpellState state)
    {
        switch (state)
        {
            case SpellState.Charging:
                SetFireBallsActive(true);
                break;
            case SpellState.Activating:
                AddFlameColumn();
                break;
            case SpellState.Idle:
                SetFireBallsActive(false);
                if (m_ActiveFireBeam)
                {
                    RemoveSpell();
                }
                break;
            default:
                break;
        }
    }

    void SetFireBallsActive(bool active)
    {
        fireBallLeft.SetActive(active);
        fireBallRight.SetActive(active);
    }

    void AddFlameColumn()
    {
        // Get the average position of the 2 charging fireballs to decide where to start the flame column
        m_SpellActivatePosition = (fireBallLeft.transform.position + fireBallRight.transform.position) / 2f;
        m_SpellVelocity = m_PlayerTransform.forward;
        m_ActiveFireBeam = Instantiate(fireBeam, m_SpellActivatePosition, Quaternion.identity) as GameObject;
        m_ActiveFireBeam.SetActive(true);
        m_TimeBeforeAddLight = Time.time;
    }

    void AddFlameLight(Vector3 position)
    {
        GameObject flameLightInstance = Instantiate(flameLight, position, Quaternion.identity) as GameObject;
        flameLightInstance.transform.parent = lightColumn.transform;
        m_TimeBeforeAddLight = Time.time;
    }

    void CheckDestroyLights()
    {
        // if we have an active fire beam, then compare its position against the light source,
        // otherwise activeFireBeamTransform is null and all light sources should be disabled
       Transform activeFireBeamTransform =  m_ActiveFireBeam ?
                                            m_ActiveFireBeam.transform as Transform :
                                            null ;

        float lightDisableThresholdSqrd = Mathf.Pow(lightDisableThresholdDistance, 2f);
        Transform lightColumnTransform = lightColumn.transform;
        foreach (Transform child in lightColumnTransform)
        {
            // destroy light if either no active fire beam, or it is not within range anymore,
            // using squared distance to see if light is within threshold distance
            Light flameLight = child.GetComponent<Light>();
            if (!activeFireBeamTransform ||
                Vector3.SqrMagnitude(activeFireBeamTransform.transform.position - child.position) > lightDisableThresholdSqrd)
            {
                // Keep reducing a lights intensity until it has faded out, before destroying completely
                flameLight.intensity -= Time.deltaTime * lightFadeSpeedMultiplier;
                if (flameLight.intensity <= 0f)
                {
                    Destroy(child.gameObject);
                }
            }
            else if (flameLight.intensity < maxLightIntensity)
            {
                // if light is in range keep fading it in until reach max intensity
                flameLight.intensity += Time.deltaTime * lightFadeSpeedMultiplier;
            }
        }
    }

    void RemoveSpell()
    {
        // Destroy with a delay
        Destroy(m_ActiveFireBeam, 1f);
        m_ActiveFireBeam = null;

        CheckDestroyLights();
    }
}
