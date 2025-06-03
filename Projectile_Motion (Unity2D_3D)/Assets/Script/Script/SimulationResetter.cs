using System;
using UnityEngine;

public class SimulationResetter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject simulationTemplate;  
    [SerializeField] private Transform parentContainer;     
    
    private GameObject currentInstance;

    private void Start()
    {
        ResetSimulation();
    }

    public void ResetSimulation()
    {
    
        if (currentInstance != null)
            Destroy(currentInstance);


        currentInstance = Instantiate(simulationTemplate, parentContainer);
        currentInstance.SetActive(true);
    }
}