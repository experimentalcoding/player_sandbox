using UnityEngine;
using System.Collections;

// The CameraMovement class smoothly follows the player, 
// and also interpolates to various different camera angles when spell is activated
public class CameraMovement : MonoBehaviour {

    // how fast camera should follow player
    public float smooth = 1.5f;

    // spellCameraAnchors is used to cycle between different camera angles when activating spells 
    public GameObject[] spellCameraAnchors;

    // private variables
    private GameObject m_ActiveSpellAnchor;
    private Transform m_PlayerTransform;
    private FireAttack m_FireAttack;
    private Vector3 m_RelativeCameraPosition;
    private float m_RelativeCameraPositionMagnitude;
    private Vector3 m_TargetPosition;
    private int m_SpellAnchorIndex;
    private bool m_DefaultState;

    void Awake ()
    {
        // store the player transform for making the camera follow the player
        GameObject player = GameObject.FindGameObjectWithTag(Tags.player);
        m_PlayerTransform = player.transform;

        // store the fire attack component for querying which transform the camera should lerp to
        m_FireAttack = player.GetComponent<FireAttack>();

        // store the initial offset, allows easy setting of relative camera angle in editor
        m_RelativeCameraPosition = transform.position - m_PlayerTransform.position;
        m_RelativeCameraPositionMagnitude = m_RelativeCameraPosition.magnitude - .5f;


        m_TargetPosition = new Vector3();
        m_DefaultState = true;
    }

    void FixedUpdate()
    {
        if (m_FireAttack.State == FireAttack.SpellState.Idle)
        {
            // The standard position of the camera is the relative position of the camera from the player.
            m_TargetPosition = m_PlayerTransform.position + m_RelativeCameraPosition;

            // The abovePos is directly above the player at the same distance as the standard position.
            Vector3 abovePos = m_PlayerTransform.position + Vector3.up * m_RelativeCameraPositionMagnitude;
            m_DefaultState = true;
        }
        else
        {
            // If have just transitioned to tracking the fireball, then choose a new anchor at random
            if (m_DefaultState)
            {
                m_ActiveSpellAnchor = GetNewAnchor();
            }

            m_TargetPosition = m_ActiveSpellAnchor.transform.position;
            m_DefaultState = false;
        }

        // Lerp the camera's position between it's current position and it's new position.
        transform.position = Vector3.Lerp(transform.position, m_TargetPosition, smooth * Time.deltaTime);

        // Make sure the camera is looking at the player.
        SmoothLookAt();
    }

    void SmoothLookAt()
    {
        // if player is not casting a spell, then ensure that camera is looking at them
        Quaternion lookAtRotation;
        if (m_FireAttack.State == FireAttack.SpellState.Idle)
        {
            // Create a vector from the camera towards the player.
            Vector3 relPlayerPosition = m_PlayerTransform.position - transform.position;

            // Create a rotation based on the relative position of the player being the forward vector.
            lookAtRotation = Quaternion.LookRotation(relPlayerPosition, Vector3.up);
        }
        else
        {
            lookAtRotation = Quaternion.LookRotation(m_ActiveSpellAnchor.transform.forward, Vector3.up);
        }

        // Lerp the camera's rotation between it's current rotation and the rotation that looks at the player.
        transform.rotation = Quaternion.Lerp(transform.rotation, lookAtRotation, smooth * Time.deltaTime);
    }

    GameObject GetNewAnchor()
    {
        // Cycle through list of anchor positions, wrapping around to beginning
        GameObject anchor = spellCameraAnchors[m_SpellAnchorIndex++];
        if (m_SpellAnchorIndex >= spellCameraAnchors.Length)
        {
            m_SpellAnchorIndex = 0;
        }

        return anchor;
    }
}
