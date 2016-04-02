using UnityEngine;
using System.Collections;

public class CollNotifier : MonoBehaviour {

    void OnCollisionEnter()
    {
        TrackManager.Instance.NotifyCollision(gameObject.GetInstanceID());
    }
}
