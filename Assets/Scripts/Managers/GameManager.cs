using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine.SceneManagement;
using UnityEngine.UI;

using Photon.Pun;

public abstract class GameManager : MonoBehaviourPunCallbacks
{
    public int m_NumRoundsToWin = 5;        
    public float m_StartDelay = 3f;         
    public float m_EndDelay = 3f;
    public GameObject m_TankPrefab;
    public CameraControl m_CameraControl;   
    public Text m_MessageText;                      
    public TankManager[] m_Tanks;           

    private int m_RoundNumber;              
    private WaitForSeconds m_StartWait;     
    private WaitForSeconds m_EndWait;
    private WaitUntil m_RoundEndWait;
    private TankManager m_RoundWinner;
    private TankManager m_GameWinner;

    // SETUP

    protected virtual void Start()
    {
        m_StartWait = new WaitForSeconds(m_StartDelay);
        m_EndWait = new WaitForSeconds(m_EndDelay);
        m_RoundEndWait = new WaitUntil(OneTankLeft);
    }

    public void GameplayBegin()
    {
        SetupTanks();
        SetupCameraTargets();
        StartCoroutine(GameLoop());
    }
    private void SetupTanks()
    {
        GameObject[] instances = GetTankInstances();

        // Check the lengths of the instances with the length of the tank managers
        if(instances.Length != m_Tanks.Length)
        {
            Debug.LogWarning(m_Tanks.Length + " tank managers provided but only " + instances.Length + " tank instances found.  " +
                "Feel free to play with the available tanks, but if you try to start the main game loop, you will get severe " +
                "null reference exceptions");
        }

        // Setup the tank managers
        for(int i = 0; i < m_Tanks.Length; i++)
        {
            if(instances[i] != null)
            {
                m_Tanks[i].Setup(instances[i], i + 1);
            }
        }
    }
    protected void SetupCameraTargets()
    {
        GameObject[] instances = GetTankInstances();
        m_CameraControl.m_Targets = instances.Where(x =>
        {
            return x != null;
        })
        .Select(x =>
        {
            return x.transform;
        })
        .ToArray();
    }

    // GAME LOOP

    private IEnumerator GameLoop()
    {
        yield return StartCoroutine(RoundStarting());
        yield return StartCoroutine(RoundPlaying());
        yield return StartCoroutine(RoundEnding());

        if (m_GameWinner != null)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            StartCoroutine("GameLoop");
        }
    }

    private IEnumerator RoundStarting()
    {
        ResetAllTanks();
        DisableTankControl();

        m_CameraControl.SetStartPositionAndSize();

        m_RoundNumber++;
        m_MessageText.text = "ROUND " + m_RoundNumber;

        yield return m_StartWait;
    }

    private IEnumerator RoundPlaying()
    {
        EnableTankControl();
        m_MessageText.text = "";
        yield return m_RoundEndWait;
    }

    private IEnumerator RoundEnding()
    {
        DisableTankControl();

        // Check for the round winner
        m_RoundWinner = GetRoundWinner();
        if (m_RoundWinner != null) m_RoundWinner.Win();

        m_GameWinner = GetGameWinner();
        m_MessageText.text = EndMessage();

        yield return m_EndWait;
    }

    // HELPERS

    private bool OneTankLeft()
    {
        int numTanksLeft = 0;
        
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Instance.activeSelf)
                numTanksLeft++;
        }

        return numTanksLeft <= 1;
    }

    private TankManager GetRoundWinner()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Instance.activeSelf)
                return m_Tanks[i];
        }

        return null;
    }

    private TankManager GetGameWinner()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Wins == m_NumRoundsToWin)
                return m_Tanks[i];
        }

        return null;
    }

    private string EndMessage()
    {
        string message = "DRAW!";

        if (m_RoundWinner != null)
            message = m_RoundWinner.m_ColoredPlayerText + " WINS THE ROUND!";

        message += "\n\n\n\n";

        for (int i = 0; i < m_Tanks.Length; i++)
        {
            message += m_Tanks[i].m_ColoredPlayerText + ": " + m_Tanks[i].m_Wins + " WINS\n";
        }

        if (m_GameWinner != null)
            message = m_GameWinner.m_ColoredPlayerText + " WINS THE GAME!";

        return message;
    }

    private void ResetAllTanks()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].Reset();
        }
    }

    private void EnableTankControl()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].EnableControl();
        }
    }

    private void DisableTankControl()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].DisableControl();
        }
    }

    protected abstract GameObject[] GetTankInstances();
}