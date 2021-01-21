using UnityEngine;

using Photon.Pun;

public abstract class TankMovement : MonoBehaviourPunCallbacks
{
    public int m_PlayerNumber = 1;         
    public float m_Speed = 12f;            
    public float m_TurnSpeed = 180f;       
    public AudioSource m_MovementAudio;    
    public AudioClip m_EngineIdling;       
    public AudioClip m_EngineDriving;      
    public float m_PitchRange = 0.2f;

    protected float m_MovementInputValue;
    protected float m_TurnInputValue;

    private Rigidbody m_Rigidbody;         
    private float m_OriginalPitch;         

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    public override void OnEnable ()
    {
        m_Rigidbody.isKinematic = false;
        m_MovementInputValue = 0f;
        m_TurnInputValue = 0f;
    }

    public override void OnDisable ()
    {
        m_Rigidbody.isKinematic = true;
    }

    private void Start()
    {
        m_OriginalPitch = m_MovementAudio.pitch;
    }

    private void Update()
    {
        SetInputValues();
        EngineAudio();
    }

    private void EngineAudio()
    {
        // Check if the tank is not moving
        if(Mathf.Abs(m_MovementInputValue) < 0.1f && Mathf.Abs(m_TurnInputValue) < 0.1f)
        {
            // If idling is not playing, play it
            if(m_MovementAudio.clip != m_EngineIdling)
            {
                PlayAudioClip(m_EngineIdling);
            }
        }
        // Enter here if tank is moving
        else
        {
            // If driving clip is not currently playing, play it
            if(m_MovementAudio.clip != m_EngineDriving)
            {
                PlayAudioClip(m_EngineDriving);
            }
        }
    }

    private void FixedUpdate()
    {
        Move();
        Turn();
    }

    private void Move()
    {
        Vector3 movement = transform.forward * m_MovementInputValue * m_Speed * Time.deltaTime;
        m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
    }

    private void Turn()
    {
        float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;
        Quaternion rotation = Quaternion.Euler(0f, turn, 0f);
        m_Rigidbody.MoveRotation(m_Rigidbody.rotation * rotation);
    }

    private void PlayAudioClip(AudioClip clip)
    {
        m_MovementAudio.clip = clip;
        m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
        m_MovementAudio.Play();
    }

    protected abstract void SetInputValues();
}