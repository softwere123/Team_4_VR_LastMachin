using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetMaterialFromList : MonoBehaviour
{
    public List<Material> materials; // Material 리스트로 변경

    public void SetMaterial(int index)
    {
        if (index >= 0 && index < materials.Count && materials[index] != null)
        {
            GetComponent<Renderer>().material = materials[index]; // Material 교체
        }
        else
        {
            Debug.LogWarning("Invalid material index or material is null!");
        }
    }
}