using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalTankShooting : TankShooting
{
    protected override bool GetButtonDown()
    {
        return Input.GetButtonDown("Fire" + m_PlayerNumber);
    }
    protected override bool GetButton()
    {
        return Input.GetButton("Fire" + m_PlayerNumber);
    }
    protected override bool GetButtonUp()
    {
        return Input.GetButtonUp("Fire" + m_PlayerNumber);
    }
    protected override Rigidbody InstantiateShell()
    {
        GameObject instance = Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation) as GameObject;
        return instance.GetComponent<Rigidbody>();
    }
}
