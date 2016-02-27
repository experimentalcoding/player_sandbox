using UnityEngine;
using System.Collections;

public class SpotLightFollow : MonoBehaviour {

    public float followSmoothing = 2f;

    private Transform m_PlayerTransform;

    void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag(Tags.player);
        m_PlayerTransform = player.transform;
    }

	void FixedUpdate ()
    {
        // Lerp the camera's position between it's current position and it's new position.
        transform.position = Vector3.Lerp(transform.position, m_PlayerTransform.position, followSmoothing * Time.deltaTime);
    }
}
