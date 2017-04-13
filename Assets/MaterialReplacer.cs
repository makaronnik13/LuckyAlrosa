using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using HighlightingSystem;

public class MaterialReplacer : MonoBehaviour
{
	public Material selectionMaterial;

    private struct SavedMaterials
    {
        public SavedMaterials(Renderer rend, List<Material> mat)
        {
            renderer = rend;
            materials = mat.ToArray();
        }
        public Renderer renderer;
        public Material[] materials;
    }

    private List<SavedMaterials> savedMaterials = new List<SavedMaterials>();

	public void SelectItem(GameObject go)
	{
		Debug.Log ("select "+go);

		foreach(Highlighter hl in go.GetComponentsInChildren<Highlighter>())
		{
			hl.ConstantOn(new Color(0, 120, 250, 1));
		}
		foreach(Renderer rend in go.GetComponentsInChildren<Renderer>())
		{
			Replace (rend, selectionMaterial);
		}
	}

	public void DeselectItem(GameObject go)
	{
		Debug.Log ("Deselect "+go);
		foreach(Highlighter hl in go.GetComponentsInChildren<Highlighter>())
		{
			hl.ConstantOff();
		}
		foreach(Renderer rend in go.GetComponentsInChildren<Renderer>())
		{
			GetMaterialsBack (rend);
		}
	}


	public void Replace(Renderer rend, Material newMaterial)
    {
        Debug.Log(newMaterial);

		SavedMaterials sm = new SavedMaterials(rend, rend.sharedMaterials.ToList());
		savedMaterials.Add (sm);
		List<Material> newMaterials = new List<Material> ();

        foreach (Material m in rend.materials)
        {
			newMaterials.Add(ReplacedMaterial(m, newMaterial));
        }

		rend.materials = newMaterials.ToArray();
    }

	public void GetMaterialsBack(Renderer rend){
		SavedMaterials sm = savedMaterials.Find (r => r.renderer == rend);

        if (!savedMaterials.Contains(sm))
        {
            return;
        }
		rend.materials = sm.materials;
		savedMaterials.Remove (sm);
	}

	private Material ReplacedMaterial(Material m, Material newMaterial)
    {
		m.shader = newMaterial.shader;
		return m;
    }
}
