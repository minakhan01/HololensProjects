using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class F3DSun : MonoBehaviour
{
    public bool AutoUpdate;

    public F3DPlanet[] Planets;

    public float radius;

    public int PlanetLayer;
    int sunPosRef;

    // Use this for initialization
    void Awake()
    {
        sunPosRef = Shader.PropertyToID("_SunPos");
    }

    // Update is called once per frame
    public void UpdatePlanets()
    {
        if (Planets != null && AutoUpdate)
        {
            for (int i = 0; i < Planets.Length; i++)
            {
                if (Planets[i] == null)
                {
                    Debug.LogWarning("F3DSun : Planet script has been removed from one of the objects in the scene. Please refresh.");
                    Planets = null;
                    return;
                }
                
                Renderer[] planetRenderers = Planets[i].GetComponentsInChildren<Renderer>();

                for (int m = 0; m < planetRenderers.Length; m++)
                {
                    planetRenderers[m].sharedMaterial.SetVector(sunPosRef, transform.position); 
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, transform.localScale.x * 0.55f);
    }

    void Update()
    {
        UpdatePlanets();
    }
}
