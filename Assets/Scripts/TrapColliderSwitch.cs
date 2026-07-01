using System.Collections.Generic;
using UnityEngine;

public class TrapColliderSwitch : MonoBehaviour
{
    public List<Collider2D> colliders;

    public void SetDang(int index)
    {
        for (int i = 0; i < colliders.Count; i++)
        {
            colliders[i].enabled = (i == index);
        }
    }
}