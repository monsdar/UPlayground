using UnityEngine;
using ProtoBuf;
using EasyErgsocket;
using NetMQ;
using NetMQ.Sockets;
using System.Collections.Generic;
using System.Linq;

public class ErgManager : MonoBehaviour
{
    NetMQContext context = null;
    SubscriberSocket subSocket = null;
    
    //String: Id of the Erg
    //GameObject: The boat
    IDictionary<string, GameObject> boats = new Dictionary<string, GameObject>();
    string playerIndex = "";

    public GameObject boatType;
    public int numLanes = 5;
    public int playerLaneIndex = 2;
    public double laneDistance = 20.0;
    IList<Vector3> freeLanes = new List<Vector3>(); //these are the lanes for bots
    Vector3 playerLane = new Vector3(); //position of the lane where the player is on
    
    void Start ()
    {
        initLanes();

        Debug.Log("Starting up NetMQ interface");
        context = NetMQContext.Create();
        subSocket = context.CreateSubscriberSocket();
        subSocket.Connect("tcp://127.0.0.1:21744");
        subSocket.Subscribe("EasyErgsocket");
    }

    private void initLanes()
    {
        //populate the lane list according to the given values
        double startDistance = 0.0 - (laneDistance * playerLaneIndex);
        for (int index = 0; index < numLanes; index++)
        {
            Vector3 newLanePos = transform.position;
            newLanePos.z = (float)(startDistance + (laneDistance * index));
            if (index == playerLaneIndex)
            {
                playerLane = newLanePos;
            }
            else
            {
                freeLanes.Add(newLanePos);
            }
        }
    }

    void Update()
    {
        //try to receive something from the network... if that succeeds get the distance from the given Erg
        var message = new NetMQMessage();
        if (subSocket.TryReceiveMultipartMessage(System.TimeSpan.Zero, ref message))
        {
            foreach (var frame in message.Skip(1)) //the first frame is always just the envelope/topic... let's ignore it by using Linq
            {
                byte[] rawMessage = frame.Buffer;
                using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream(rawMessage))
                {
                    var givenErg = Serializer.Deserialize<EasyErgsocket.Erg>(memoryStream);
                    UpdateErg(givenErg);
                }
            }
        }

        //update the statsdisplay and all the other important things
        if(boats.ContainsKey(playerIndex))
        {
            Boat playerBoat = boats[playerIndex].GetComponentInChildren<Boat>();
            StatDisplayManager.Instance.UpdatePosition(playerBoat.Distance);

            foreach (var boat in boats)
            {
                //do not update the playerboat with itself
                if(boat.Key == playerIndex)
                {
                    continue;
                }

                Boat currentBoat = boat.Value.GetComponentInChildren<Boat>();
                currentBoat.AttachToBoat(playerBoat.Distance);
            }
        }
    }

    private void UpdateErg(Erg givenErg)
    {
        //if the boat does not exist yet, add it
        if(!boats.ContainsKey(givenErg.ergId))
        {
            CreateBoat(givenErg);
        }
        
        //update the boats distance
        boats[givenErg.ergId].GetComponent<Boat>().Distance = (float)givenErg.distance;

        //update the stats on the track
        if (givenErg.playertype == EasyErgsocket.PlayerType.HUMAN)
        {
            StatDisplayManager.Instance.UpdateStats(givenErg);
        }
    }

    private void CreateBoat(Erg givenErg)
    {
        Vector3 newPos;
        Quaternion newRot = new Quaternion();
        if (givenErg.playertype == EasyErgsocket.PlayerType.HUMAN)
        {
            newPos = playerLane;
            playerIndex = givenErg.ergId;
        }
        else
        {
            newPos = GetFreeBotLane();
        }

        Debug.Log("Created new Boat " + givenErg.ergId);
        boats[givenErg.ergId] = Instantiate(boatType, newPos, newRot) as GameObject;
        
        //if we just created the player boat we need to take care of positioning the camera, creating the track etc
        if (givenErg.playertype == EasyErgsocket.PlayerType.HUMAN)
        {
            CameraManager.Instance.SetParent(boats[givenErg.ergId].transform);
            TrackManager.Instance.SetDistance((float)givenErg.distance);
        }

        //update the boats name etc
        boats[givenErg.ergId].GetComponent<Boat>().BoatName = givenErg.name;
    }

    private Vector3 GetFreeBotLane()
    {
        if(freeLanes.Count >= 1)
        {
            Vector3 retValue = freeLanes[0];
            freeLanes.RemoveAt(0);
            return retValue;
        }

        return new Vector3();
    }

    void OnApplicationQuit()
    {
        Debug.Log("Shutting down...");
        subSocket.Close();
        context.Terminate();
        Debug.Log("Done...");
    }
}
