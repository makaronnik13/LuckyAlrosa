using HoloToolkit.Unity.InputModule;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScenariosPlayer : MonoBehaviour, IInputClickHandler
{

    public List<Scenario> scenarios = new List<Scenario>();

    private int currentStep;

    private bool nextStepEnabled;

    private Scenario currentScenario;
    private bool moveTime;
    private float time;

    private float scenarioTime;

    // Use this for initialization
    void Start () {
       // PlayScenario(0);
	}
	
	// Update is called once per frame
	void Update () {
        if (moveTime && currentScenario!=null)
        {
            Debug.Log(time);

            time += Time.deltaTime;

            foreach (ScenarioStep step in currentScenario.scenarioSteps)
            {
                if (!step.activated && step.time<time)
                {
                    PlayStep(step);
                }
            }

            if (time>scenarioTime)
            {
                StopScenario();
            }
        }
    }

    public void PlayScenario(int id)
    {
        PlayScenario(scenarios[id]);
    }

    public void PlayScenario(string name)
    {
        PlayScenario(scenarios.ToList().Find(s=>s.ScenarioName == name));
    }

    private void PlayScenario(Scenario scenario)
    {
        StopScenario();
        currentScenario = scenario;
        ScenarioStep lastStep = currentScenario.scenarioSteps.Find(s => s.time == scenarioTime);
        scenarioTime = currentScenario.scenarioSteps.Max(s => s.time);
        if (lastStep.stepType == ScenarioStep.StepType.AudioClip)
        {
            scenarioTime += lastStep.clip.length;
        }
        moveTime = true;
    }

    private void PlayStep(ScenarioStep step)
    {
        Debug.Log(step.time+" step");



        step.activated = true;

        if (step.waitCommand)
        {
            moveTime = false;
        }

        if (step.stepType == ScenarioStep.StepType.Animation)
        {
            switch (step.parameterType)
            {
                case AnimatorControllerParameterType.Bool:
                    GetComponent<Animator>().SetBool(step.animatorParameter, step.boolType);
                    break;
                case AnimatorControllerParameterType.Float:
                    GetComponent<Animator>().SetFloat(step.animatorParameter, step.floatType);
                    break;
                case AnimatorControllerParameterType.Int:
                    GetComponent<Animator>().SetInteger(step.animatorParameter, step.intType);
                    break;
                case AnimatorControllerParameterType.Trigger:
                    GetComponent<Animator>().SetTrigger(step.animatorParameter);
                    break;
            }
        }

        if (step.stepType == ScenarioStep.StepType.AudioClip)
        {
            GetComponent<AudioSource>().Stop();
            GetComponent<AudioSource>().PlayOneShot(step.clip);
        }

        if (step.stepType == ScenarioStep.StepType.Object)
        {
            step.go.SetActive(step.objectActive);
        }

        if (step.stepType == ScenarioStep.StepType.Material)
        {
            if (step.materialActive) {
                Debug.Log(step.replacedMaterial);
                FindObjectOfType<MaterialReplacer>().selectionMaterial = step.replacedMaterial;
                FindObjectOfType<MaterialReplacer>().SelectItem(step.go);
            }
            else
            {
                FindObjectOfType<MaterialReplacer>().DeselectItem(step.go);
            }
        }
    }

    internal void StopScenario()
    {
        if (currentScenario==null)
        {
            return;
        }

            foreach (ScenarioStep step in currentScenario.scenarioSteps)
            {
            step.activated = false;

                switch (step.stepType)
                {
                    case ScenarioStep.StepType.Animation:
                        GetComponent<Animator>().Rebind();
                        break;
                    case ScenarioStep.StepType.Material:
                        GetComponent<MaterialReplacer>().DeselectItem(step.go);
                        break;
                    case ScenarioStep.StepType.Object:
                        step.go.SetActive(false);
                        //set object default
                        break;
                    case ScenarioStep.StepType.AudioClip:
                        GetComponent<AudioSource>().Stop();
                        break;
                }
        }

        currentScenario = null;
        moveTime = false;
    }


    public void OnInputClicked(InputClickedEventData eventData)
    {

        moveTime = true;
    }
}
