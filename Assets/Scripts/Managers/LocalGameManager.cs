using UnityEngine;
using System.Linq;

public class LocalGameManager : GameManager
{
    private GameObject[] instances;

    protected override void Start()
    {
        base.Start();
        SpawnAllTanks();
        GameplayBegin();
    }

    private void SpawnAllTanks()
    {
        instances = new GameObject[m_Tanks.Length];

        for (int i = 0; i < m_Tanks.Length; i++)
        {
            instances[i] = Instantiate(m_TankPrefab, m_Tanks[i].m_SpawnPoint.position, m_Tanks[i].m_SpawnPoint.rotation) as GameObject;
        }
    }

    protected override GameObject[] GetTankInstances()
    {
        return instances;
    }
}
