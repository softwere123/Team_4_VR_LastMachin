using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashCan_CMS : MonoBehaviour
{
    private void Start()
    {
        GetComponent<TriggerZone_CMS>().OnEnterEvent.AddListener(InsideTrash);
    }
    public void InsideTrash(GameObject go)
    {
        go.SetActive(false);
    }
}
