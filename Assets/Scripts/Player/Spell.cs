//using UnityEngine;
//using System.Collections;

//public class Spell : UnityEngine.Object
//{
//    public enum SpellState { Charging, Activating, Idle }
//    protected SpellState m_SpellState = SpellState.Idle;

//    // Player
//    protected GameObject m_PlayerGameObject;
//    private Transform m_PlayerTransform;

//    public Spell()
//    {
//        m_PlayerGameObject = GameObject.FindGameObjectWithTag(Tags.player);
//        m_PlayerTransform = m_PlayerGameObject.GetComponent<Transform>();
//    }

//    public void OnSpellStateEvent(string eventParameter)
//    {
//        Debug.Log(eventParameter);

//        // Parse state as enum and handle state change
//        Spell.SpellState state = (Spell.SpellState)System.Enum.Parse(typeof(Spell.SpellState), eventParameter);
//        SetSpellState(state);
//    }

//    void SetSpellState(SpellState state)
//    {
//        if (m_SpellState != state)
//        {
//            m_SpellState = state;
//            OnSpellStateChanged(m_SpellState);
//        }
//    }

//    virtual protected void OnSpellStateChanged(SpellState state) { }
//}
