using UnityEngine;
using System.Collections;

public class PlayerInteractions : MonoBehaviour
{
    // Useful to query 
    public Transform leftHand;  
    public Transform rightHand; 

    private Animator m_Animator;
    private HashIDs m_HashIDs;

    // Whether or not force field is active or not

    // Spell effect scripts
    public FireAttack fireAttack;
    public ForceField forceField;

    void Awake ()
    {
        // Get required references
        m_Animator = GetComponent<Animator>();
        m_HashIDs = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<HashIDs>();
    }

    void Update ()
    {
        // If player presses Ctrl or A on controller, send a fire ball attack
        bool attack = Input.GetButton("Fire1");

        // Trigger transition to attacking state in animator controller
        m_Animator.SetBool(m_HashIDs.attackingBool, attack);

        // If player presses 
        if (Input.GetButtonDown("Fire2"))
        {
            // Toggle force field on/off, can only be active if fire attack is not
            forceField.Enabled = (!forceField.Enabled);
        }

        // If fire attack is active then disable force field
        bool fireAttackActive = fireAttack.State != FireAttack.SpellState.Idle;
        if (fireAttack.State != FireAttack.SpellState.Idle)
        {
            forceField.Enabled = false;
        }
    }

    void OnAnimationEvent(string eventParameter)
    {
        // Delegate to the equipped spell
        fireAttack.OnSpellStateEvent(eventParameter);
    }
}
