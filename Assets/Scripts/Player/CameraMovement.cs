using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {

    public float smooth = 1.5f;

    public GameObject spellCameraAnchor;

    private Transform m_PlayerTransform;
    private FireAttack m_FireAttack;
    private Vector3 m_RelativeCameraPosition;
    private float m_RelativeCameraPositionMagnitude;
    private Vector3 m_TargetPosition;

    void Awake ()
    {
        GameObject player = GameObject.FindGameObjectWithTag(Tags.player);
        m_PlayerTransform = player.transform;
        m_FireAttack = player.GetComponent<FireAttack>();
        m_RelativeCameraPosition = transform.position - m_PlayerTransform.position;
        m_RelativeCameraPositionMagnitude = m_RelativeCameraPosition.magnitude - .5f;
        m_TargetPosition = new Vector3();
    }

    void FixedUpdate()
    {
        if (m_FireAttack.State == FireAttack.SpellState.Idle)
        {
            // The standard position of the camera is the relative position of the camera from the player.
            m_TargetPosition = m_PlayerTransform.position + m_RelativeCameraPosition;

            // The abovePos is directly above the player at the same distance as the standard position.
            Vector3 abovePos = m_PlayerTransform.position + Vector3.up * m_RelativeCameraPositionMagnitude;
        }
        else
        {
            m_TargetPosition = spellCameraAnchor.transform.position;
        }

        // Lerp the camera's position between it's current position and it's new position.
        transform.position = Vector3.Lerp(transform.position, m_TargetPosition, smooth * Time.deltaTime);

        // Make sure the camera is looking at the player.
        SmoothLookAt();
    }

    void SmoothLookAt()
    {
        // If player is not casting a spell, then 
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
            lookAtRotation = Quaternion.LookRotation(spellCameraAnchor.transform.forward, Vector3.up);
        }

        // Lerp the camera's rotation between it's current rotation and the rotation that looks at the player.
        transform.rotation = Quaternion.Lerp(transform.rotation, lookAtRotation, smooth * Time.deltaTime);
    }
}
