using UnityEngine;
using System.Collections;

/// <summary>
/// Component class, attached to triggers.
/// This class will indentify when a goal happens, and tell the game controller.
/// </summary>
public class GoalTrigger : MonoBehaviour 
{
    public void OnTriggerEnter(Collider other)
    {
        if (Network.isServer && other.gameObject.tag == "Ball")
        {
            // red player's net, blue player's goal
            if(gameObject.name.Contains("Red"))
            {
                GameController.instance.ScoreBlueGoal();
            }
            // blue player's net, red player's goal
            else if (gameObject.name.Contains("Blue"))
            {
                GameController.instance.ScoreRedGoal();
            }
        }
    }
}
