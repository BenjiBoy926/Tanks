using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;

public abstract class TankShooting : MonoBehaviourPunCallbacks
{
    public int m_PlayerNumber = 1;       
    public GameObject m_Shell;            
    public Transform m_FireTransform;    
    public Slider m_AimSlider;           
    public AudioSource m_ShootingAudio;  
    public AudioClip m_ChargingClip;     
    public AudioClip m_FireClip;         
    public float m_MinLaunchForce = 15f; 
    public float m_MaxLaunchForce = 30f; 
    public float m_MaxChargeTime = 0.75f;

    private float m_CurrentLaunchForce;
    private float m_ChargeSpeed;
    private bool m_Fired;

    public override void OnEnable()
    {
        m_CurrentLaunchForce = m_MinLaunchForce;
        m_AimSlider.value = m_MinLaunchForce;
    }

    protected virtual void Start()
    {
        m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;
    }

    protected virtual void Update()
    {
        // Track the current state of the fire button and make decisions based on the current launch force.
        m_AimSlider.value = m_MinLaunchForce;

        // If we have exceeded the max charge, fire at max charge
        if(m_CurrentLaunchForce >= m_MaxLaunchForce && !m_Fired)
        {
            Fire();
        }
        // As soon as the button is pressed, start charging up
        else if(GetButtonDown())
        {
            m_Fired = false;
            m_CurrentLaunchForce = m_MinLaunchForce;

            // Play the audio
            m_ShootingAudio.clip = m_ChargingClip;
            m_ShootingAudio.Play();
        }
        // If we are still holding the fire button, continue charging up
        else if(GetButton() && !m_Fired)
        {
            m_CurrentLaunchForce += m_ChargeSpeed * Time.deltaTime;
            m_AimSlider.value = m_CurrentLaunchForce;
        }
        else if(GetButtonUp() && !m_Fired)
        {
            Fire();
        }
    }

    private void Fire()
    {
        m_Fired = true;

        Rigidbody shellInstance = InstantiateShell();
        shellInstance.velocity = m_FireTransform.forward * m_CurrentLaunchForce;

        // Play audio
        m_ShootingAudio.clip = m_FireClip;
        m_ShootingAudio.Play();

        // Reduce launch force to min
        m_CurrentLaunchForce = m_MinLaunchForce;
    }

    // ABSTRACT INTERFACE
    protected abstract bool GetButtonDown();
    protected abstract bool GetButton();
    protected abstract bool GetButtonUp();
    protected abstract Rigidbody InstantiateShell();
}