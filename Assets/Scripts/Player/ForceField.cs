using UnityEngine;
using System.Collections;

// The ForceField class toggles on a transparent sphere with an animated procedural texture applied
public class ForceField : MonoBehaviour {

    // Property which specifies if ForceField is active.
    // If it is toggled on, it is set active and faded in
    private bool m_Enabled = false;
    public bool Enabled
    {
        get { return m_Enabled; }
        set
        {
            m_Enabled = value;
            if (m_Enabled)
            {
                gameObject.SetActive(true);
            }
        }
    }

    // how fast should effect fade in
    public float fadeTimeScale = 2f;

    // For modifying opacity of effect
    private float m_Opacity;
    private Renderer m_Renderer;

	void Awake ()
    {
        // Store the initial opacity of the material before 
        m_Renderer = GetComponent<Renderer>();
        m_Opacity = m_Renderer.material.color.a;

        // Now set alpha to 0 to be fully transparent since starting disabled
        Color color = m_Renderer.material.color;
        color.a = 0f;
        m_Renderer.material.color = color;
    }
	
	void Update ()
    {
        // Store the color of the material before modifying
        Color color = m_Renderer.material.color;

        // fade in if enabled, out otherwise
        if (m_Enabled && color.a < m_Opacity)
        {
            color.a = Mathf.Lerp(color.a, m_Opacity, Time.deltaTime * fadeTimeScale);
        }
        else if (!m_Enabled && color.a > 0f)
        {
            color.a = Mathf.Lerp(color.a, 0f, Time.deltaTime * fadeTimeScale);

            // If have faded out completely, toggle to not active
            if (color.a <= 0f)
            {
                gameObject.SetActive(false);
            }
        }

        // Now finally set the color
        m_Renderer.material.color = color;
    }
}
