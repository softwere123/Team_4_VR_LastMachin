using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Autohand;

public class SetElemental : MonoBehaviour
{
    public Grabbable[] Fwood;
    public Grabbable[] Ffire;
    // public XRRayInteractor[] Fteleport; // 필요 시 사용

    public void SetType(int index)
    {
        SetFwood(false);
        SetFfire(false);
        // SetFteleport(false);

        if (index == 1)
            SetFwood(true);
        else if (index == 2)
            SetFfire(true);
        // else if (index == 3)
        //     SetFteleport(true);
    }

    private void SetFwood(bool enabled)
    {
        foreach (var obj in Fwood)
        {
            if (obj != null)
                obj.enabled = enabled;
        }
    }

    private void SetFfire(bool enabled)
    {
        foreach (var obj in Ffire)
        {
            if (obj != null)
                obj.enabled = enabled;
        }
    }

    //private void SetFteleport(bool enabled)
    //{
    //    foreach (var obj in Fteleport)
    //    {
    //        if (obj != null)
    //            obj.enabled = enabled;
    //    }
    //}
}