using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MissionController : MonoBehaviour
{
    public List<MissionTargetController> Targets;
    public Text PointsText;

    private int InitialTargetSize;

    void Awake()
    {
        InitialTargetSize = Targets.Count;

        for (int i = 0; i < Targets.Count; i++)
        {
            if(i == 0)
            {
                Targets[i].UpdateState(MissionTargetState.Ready);
            }
            else 
            {
                Targets[i].UpdateState(MissionTargetState.NotVisible);
            }
        }
    }

    public void UpdateCurrentTarget(MissionTargetState state)
    {
        if(Targets.Count == InitialTargetSize)
        {
            Targets[0].UpdateState(state);
            Targets.RemoveAt(0);
            foreach (MissionTargetController target in Targets)
            {
                target.UpdateState(MissionTargetState.Ready);
            }
        }
        else 
        {
            Targets[0].UpdateState(state);
            Targets.RemoveAt(0);
        }
    }

    public void UpdatePoints(int points)
    {
        PointsText.text = "Points: " + points;
    }
}