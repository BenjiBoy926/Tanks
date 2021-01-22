using UnityEngine;
using System.Collections;

public class LocalShellExplosion : ShellExplosion
{
    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        Destroy(gameObject);
    }
}
