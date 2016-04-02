using UnityEngine;
using System.Collections.Generic;

public class TrackManager : MonoBehaviour
{
    public GameObject partType;
    List<GameObject> trackparts = new List<GameObject>();
    List<int> usedIdentifiers = new List<int>();

    public int maxParts = 40;
    int partCounter = 0;

    //make this a Singleton to be called by other GameObjects
    static public TrackManager Instance { get; private set; }
    void Awake()
    {
        Instance = this;
    }

    //This is called whenever a collision with a track part has happened
    public void NotifyCollision(int senderId)
    {
        //often we get collision-calls from the same trackparts multiple times... this is why we 
        //use this ignore list - collision-calls will only be handled once per part.
        if(usedIdentifiers.Contains(senderId))
        {
            return;
        }

        //put a new part to the back of the track
        CreateBackPart();
        CleanParts(maxParts);

        usedIdentifiers.Add(senderId);
    }
    
    public void SetDistance(float distance)
    {
        Vector3 trackPos = transform.position;
        trackPos.x = distance - (distance % 100.0f);
        transform.position = trackPos;

        //this cleans any previously created tracks
        CleanParts(0);

        //create some track parts on front of the player and some in the back... this will be the track we're working with
        for (int index = 0; index < maxParts / 2; index++)
        {
            CreateBackPart();
        }
        for (int index = 0; index < maxParts / 2; index++)
        {
            CreateFrontPart();
        }
        CleanParts(maxParts);
    }

    private void CleanParts(int numMaxParts)
    {
        while (trackparts.Count > numMaxParts)
        {
            GameObject firstPart = trackparts[0];
            trackparts.RemoveAt(0);
            Destroy(firstPart, 1.0f);
        }
    }

    private void CreateBackPart()
    {
        //start from the object that we're attached on if there's no track yet
        Transform dockingPoint = transform;

        //if there already are some track parts we need to attach to them
        if (trackparts.Count > 0)
        {
            string dockName = "BackDockingPoint";
            GameObject lastPart = trackparts[trackparts.Count - 1];
            dockingPoint = lastPart.GetComponentInChildren<Transform>().Find(dockName);
        }
        trackparts.Add(Instantiate(partType, dockingPoint.position, dockingPoint.rotation) as GameObject);
        partCounter++;
    }

    private void CreateFrontPart()
    {
        //start from the object that we're attached on if there's no track yet
        Transform dockingPoint = transform;

        //if there already are some track parts we need to attach to them
        string frontDockName = "FrontDockingPoint";
        string backDockName = "BackDockingPoint";

        GameObject firstPart = trackparts[0];
        Transform frontDockingPoint = firstPart.GetComponentInChildren<Transform>().Find(frontDockName);
        Transform backDockingPoint = firstPart.GetComponentInChildren<Transform>().Find(backDockName);
        dockingPoint.position = frontDockingPoint.position - (backDockingPoint.position - frontDockingPoint.position);
        Debug.Log(dockingPoint.position.ToString());

        trackparts.Insert(0, Instantiate(partType, dockingPoint.position, dockingPoint.rotation) as GameObject);
    }

    void Start()
    {}
}
