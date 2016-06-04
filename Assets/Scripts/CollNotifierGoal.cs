using UnityEngine;
using System.Collections;

public class CollNotifierGoal : MonoBehaviour {

    bool hasBeenReached = false;

    void OnCollisionEnter(Collision collision)
    {
        if(!hasBeenReached)
        {
            var goalManager = gameObject.GetComponentInParent<GoalManager>();
            goalManager.OnCollision(collision.gameObject.transform.position.z);

            hasBeenReached = true;
        }
    }
}
