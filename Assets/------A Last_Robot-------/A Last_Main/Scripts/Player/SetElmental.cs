using System.Collections.Generic;
using UnityEngine;
using Autohand;
using System;

public class SetElemental : MonoBehaviour
{
    public List<DistanceGrabbable> magneticObjects; // 마그네틱: DistanceGrabbable 리스트
    public List<Grabbable> fireObjects;             // 파이어: Grabbable 리스트
    public bool isMagnetic = false;
    public bool isFire = false;
    public void SetType(int index)
    {
        Debug.Log($"SetType 호출됨, index: {index}");

        isMagnetic = (index == 1);
        isFire = (index == 2);

        SetMagneticObjects(isMagnetic);
        SetGrabbable(fireObjects, isFire);
    }

    private void SetMagneticObjects(bool enable)
    {
        foreach (var distanceGrab in magneticObjects)
        {
            distanceGrab.enabled = enable;
            Debug.Log($"Magnetic Object '{distanceGrab.gameObject.name}' set to {(enable ? "ENABLED" : "DISABLED")}");
        }
    }

    private void SetGrabbable(List<Grabbable> grabbables, bool enable)
    {
        foreach (var grab in grabbables)
        {
            grab.enabled = enable;
            Debug.Log($"Fire Object '{grab.gameObject.name}' set to {(enable ? "ENABLED" : "DISABLED")}");
        }
    }
}