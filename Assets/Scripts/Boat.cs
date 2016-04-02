using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Boat : MonoBehaviour {

    public TextMesh nameText = null;
    public TextMesh distanceText = null;

    float currentVelocity = 0.0f;
    float distTimestamp = 0.0f;
    float distance = 0.0f;
    float parentDistance = 0.0f;
    public float Distance
    {
        get
        {
            return getDrDistance();
        }
        set
        {
            float metersSinceLastUpdate = value - distance;
            float timeSinceLastUpdate = Time.time - distTimestamp;
            currentVelocity = metersSinceLastUpdate / timeSinceLastUpdate;
            distTimestamp = Time.time;
            distance = value;

            //update the boat
            var newPos = transform.position;
            newPos.x = distance;
            transform.position = newPos;

            //update the distance text
            if (parentDistance >= 0.0f)
            {
                float relativeDistance = value - parentDistance;
                distanceText.text = relativeDistance.ToString("0") + "m";
            }
            else
            {
                distanceText.text = distance.ToString("0") + "m";
            }
        }
    }

    private float getDrDistance()
    {
        //update the boat, apply dead reckoning
        float timeSinceLastUpdate = Time.time - distTimestamp;
        float drDistance = timeSinceLastUpdate * currentVelocity;
        return distance + drDistance;
    }

    private string boatName = "Boat";
    public string BoatName
    {
        get { return boatName; }
        set
        {
            boatName = value;
            nameText.text = boatName;
        }
    }

    public void AttachToBoat(float distance)
    {
        parentDistance = distance;

        Vector3 newDistPos = distanceText.transform.position;
        newDistPos.x = distance + 10.0f;
        distanceText.transform.position = newDistPos;

        Vector3 newNamePos = nameText.transform.position;
        newNamePos.x = distance - 8.0f;
        nameText.transform.position = newNamePos;
    }

    // Use this for initialization
    void Start () {
        if(nameText == null ||
            distanceText == null)
        {
            Debug.Log("Missing attributes! Please set NameText and DistanceText or this script will not work as expected!");
        }

    }
	
	// Update is called once per frame
	void Update ()
    {
        //update the boat, apply dead reckoning
        var newPos = transform.position;
        newPos.x = getDrDistance();
        transform.position = newPos;
    }
}
