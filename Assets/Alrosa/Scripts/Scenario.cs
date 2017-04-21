using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
//[CreateAssetMenu(fileName = "NewScenario", menuName = "Scenario", order = 1)]
public class Scenario
{
    public bool show = false;
    public string ScenarioName;
    public List<ScenarioStep> scenarioSteps = new List<ScenarioStep>();    
}
