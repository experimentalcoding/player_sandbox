using UnityEngine;
using System.Collections;

// The PlayerInteractions class is mainly used to listen to animation events, 
// before passing them through to the equipped spell, currently have just implemented FireAttack
// It also retrieves player input and uses it to activate spells by either 
// toggling on the force field directly, or triggering an animation state change on the player animator controller
public class PlayerInteractions : MonoBehaviour
{
    private Animator m_Animator;
    private HashIDs m_HashIDs;

    // Spell effect scripts
    public FireAttack fireAttack;
    public ForceField forceField;

    void Awake ()
    {
        // Get required references
        m_Animator = GetComponent<Animator>();

        // Retrieve HashIDs instance, relies on gameController tag being set in editor
        m_HashIDs = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<HashIDs>();
    }

    void Update ()
    {
        // If player presses Ctrl or A on controller, send a fire ball attack
        bool attack = Input.GetButton("Fire1");

        // Trigger transition to attacking state in animator controller
        m_Animator.SetBool(m_HashIDs.attackingBool, attack);

        // If player presses left shift key or X button on controller, toggle force field
        if (Input.GetButtonDown("Fire3"))
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
