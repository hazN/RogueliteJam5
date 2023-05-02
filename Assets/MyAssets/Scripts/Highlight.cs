using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlight : MonoBehaviour
{
    // Assign all the renderers here through the inspector
    [SerializeField]
    private List<Renderer> renderers;

    [SerializeField]
    private Color color = Color.white;

    // Helper list to cache all the materials of this object
    private List<Material> materials;

    // Gets all the materials from each renderer
    private void Awake()
    {
        materials = new List<Material>();
        foreach (var renderer in renderers)
        {
            //A single child-object might have mutliple materials on it
            materials.AddRange(new List<Material>(renderer.materials));
        }
    }

    public void ToggleHighlight(bool toggle)
    {
        if (toggle)
        {
            foreach (var material in materials)
            {
                // We need to enable the EMISSION before we set the color
                material.EnableKeyword("_EMISSION");
                material.SetColor("_EmissionColor", color);
            }
        }
        else
        {
            foreach (var material in materials)
            {
                material.DisableKeyword("_EMISSION");
            }
        }

    }
}