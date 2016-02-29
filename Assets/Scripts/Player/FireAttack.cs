using UnityEngine;
using System.Collections;

// The FireAttack class is responsible for triggering a column of fire from the players hands at the correct time.
// It achieves this by first toggling on small flame particle systems attached to characters hands (fireBallLeft and fireBallRight).
// OnSpellStateEvent is called from PlayerInteractions::OnAnimationEvent and through this, 
// can hook into animation events from the players animation controller.
//
// When adding new spells a new 'Spell' interface should be added that each type of spell conforms to.
public class FireAttack : MonoBehaviour
{
    // The state of the spell is controlled by animation events.
    // This can either be Charging, Activating, or Idle
    public enum SpellState { Charging, Activating, Idle }
    protected SpellState m_SpellState = SpellState.Idle;
    public SpellState State { get { return m_SpellState; } }

    // how fast should column of fire extend from players arms
    public float fireColumnSpeed = 5f;
    
    // how many seconds should pass before each point light is created for fire light effect
    public float lightDropDelay = .15f;

    // how many units should front of flame column should travel before disabling a light
    public float lightDisableThresholdDistance = 2f;

    // used to scale the speed at which a light will fade to 0 intensity when deactivated
    public float lightFadeSpeedMultiplier = 3f;

    // maximum intensity that lights should reach to light up column of fire
    public float maxLightIntensity = 6f;

    // fireBallLeft and fireBallRight are particle systems that player holds in hand to charge up spell
    public GameObject fireBallLeft;
    public GameObject fireBallRight;

    // fireBeam is fire column particle system prefab used for main spell effect
    public GameObject fireBeam;
    public GameObject lightColumn;

    // flameLight is prefab for point lights that are dropped as flame column moves forward
    public GameObject flameLight;

    // private variables used for tracking fire effect behaviour
    private Transform m_PlayerTransform;
    private Vector3 m_SpellActivatePosition;
    private Vector3 m_SpellVelocity;
    private GameObject m_ActiveFireBeam;
    private float m_TimeBeforeAddLight;

    // OnSpellStateEvent parses the eventParameter string as the SpellState enum,
    // and is used to change the state of the fire effect to be either charging up, activating, or idle.
    public void OnSpellStateEvent(string eventParameter)
    {
        // Parse state as enum and handle state change
        SpellState state = (SpellState)System.Enum.Parse(typeof(SpellState), eventParameter);
        SetSpellState(state);
    }

    void Awake()
    {
        // store references to player game
        GameObject playerGameObject = GameObject.FindGameObjectWithTag(Tags.player);
        m_PlayerTransform = playerGameObject.GetComponent<Transform>();
    }

    void Update()
    {
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
                break;
            case SpellState.Idle:
                break;
            default:
                break;
        }

        // Finally, check if any lights should be destroyed
        CheckDestroyLights();
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

                // state has been set to charging, so activate balls of flame in characters hands
                SetFireBallsActive(true);

                break;
            case SpellState.Activating:

                // instantiate flame column prefab and set it moving in direction of spell
                AddFlameColumn();

                break;
            case SpellState.Idle:

                // disable fire balls and destroy flame column
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

        // use the players forward direction as velocity for fire column
        m_SpellVelocity = m_PlayerTransform.forward;

        // create fire beam instance, will be moved forward in Update
        m_ActiveFireBeam = Instantiate(fireBeam, m_SpellActivatePosition, Quaternion.identity) as GameObject;
        m_ActiveFireBeam.SetActive(true);

        // store the current time for deciding when to add a new point light for illuminating fire
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

        // loop over each point light child in the light column
        Transform lightColumnTransform = lightColumn.transform;
        foreach (Transform child in lightColumnTransform)
        {
            // safety against any child game objects that do not have light components
            Light flameLight = child.GetComponent<Light>();
            if (!flameLight)
            {
                continue;
            }

            // destroy light if either no active fire beam, or it is not within range anymore,
            // using squared distance to see if light is within threshold distance
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
