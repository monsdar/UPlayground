using UnityEngine;
using System.Collections;

public class CollNotifierTrackpart : MonoBehaviour {

    bool hasCollided = false;
    void OnCollisionEnter()
    {
        if(!hasCollided)
        {
            TrackManager.Instance.NotifyCollision(gameObject.GetInstanceID());
            hasCollided = true;
        }
    }
}
