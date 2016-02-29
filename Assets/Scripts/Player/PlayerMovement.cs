using UnityEngine;
using System.Collections;

// The PlayerMovement class is based on the class of the same name from the Unity 4 'Stealth' tutorial.
// It sets the 'Speed' and 'AngularSpeed' variables in the player animator controller used to blend
// between the animations in a 2D cartesian grid.
public class PlayerMovement : MonoBehaviour
{
    public float turnSmoothing = 15f;
    public float speedDampTime = 0.1f;

    //  constant values
    private const float m_MaxSpeed = 3.98f; // from PlayerAnimator/Locomotion/standard_run

    private Animator m_Animator;
    private HashIDs m_HashIDs;
    private Rigidbody m_RigidBody;
    private FireAttack m_FireAttack;

	void Awake ()
    {
        // Get required references
        m_Animator = GetComponent<Animator>();
        m_RigidBody = GetComponent<Rigidbody>();
        m_HashIDs = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<HashIDs>();
        m_FireAttack = gameObject.GetComponent<FireAttack>();
    }

    void FixedUpdate()
    {
        // Cache the inputs.
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        MoveCharacter(h, v);
    }

    void MoveCharacter(float horizontal, float vertical)
    {
        // Only update movement if not casting a spell
        if (m_FireAttack.State != FireAttack.SpellState.Activating)
        {
            if (horizontal != 0f || vertical != 0f)
            {
                Rotating(horizontal, vertical);
                m_Animator.SetFloat(m_HashIDs.speedFloat, m_MaxSpeed, speedDampTime, Time.deltaTime);
            }
            else
            {
                m_Animator.SetFloat(m_HashIDs.speedFloat, 0f, speedDampTime, Time.deltaTime);
            }
        }
    }

    public void Rotating(float horizontal, float vertical, float extraSmoothScale = 1f)
    {
        // Create a new vector of the horizontal and vertical inputs.
        Vector3 targetDirection = new Vector3(horizontal, 0f, vertical);

        // Create a rotation based on this new vector assuming that up is the global y axis.
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
        Quaternion newRotation = Quaternion.Lerp(m_RigidBody.rotation, targetRotation, turnSmoothing * extraSmoothScale * Time.deltaTime);

        // Change the players rotation to this new rotation.
        m_RigidBody.MoveRotation(newRotation);
    }
}
