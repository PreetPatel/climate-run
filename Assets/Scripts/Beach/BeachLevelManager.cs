﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeachLevelManager : MonoBehaviour
{
    public bool SHOW_COLLIDER = false; //$$

    public static BeachLevelManager Instance {set;get;}

    //level spawning
    private const float DISTANCE_BEFORE_SPAWN = 100.0f;
    private const int INITIAL_SEGMENTS = 10;
    private const int MAX_SEGMENTS_ON_SCREEN = 15;
    private Transform cameraContainer;
    private int amountOfActiveSegments;
    private int continiousSegments;
    private int currentSpawnZ;
    private int currentLevel;
    private int y1, y2, y3;

    //list of pieces
    public List<BeachPiece> ramps = new List<BeachPiece>();
    public List<BeachPiece> longblocks = new List<BeachPiece>();
    public List<BeachPiece> jumps = new List<BeachPiece>();
    public List<BeachPiece> slides = new List<BeachPiece>();
    [HideInInspector]
    public List<BeachPiece> pieces = new List<BeachPiece>();//all the pieces in the pool

    //list of segments
    public List<Segement> availableSegments = new List<Segement>();
    public List<Segement> availableTransitions = new List<Segement>();
    [HideInInspector]
    public List<Segement> segments = new List<Segement>();

    //GamePlay
    private bool isMoving = false;

    private void Awake()
    {
        Instance = this;
        cameraContainer = Camera.main.transform;
        currentSpawnZ = 0;
        currentLevel = 0;
 
        //FindObjectOfType<CameraMotor>().isFollowing = true;
    }

    public Segement GetSegment(int id, bool transition)
    {
        Segement s = null;
        s = segments.Find(x => x.SegId == id && x.transition == transition && !x.gameObject.activeSelf);

        if( s == null) //no segment instantiated
        {
            GameObject go = Instantiate((transition)?availableTransitions[id].gameObject : availableSegments[id].gameObject) as GameObject;
            s = go.GetComponent<Segement>();

            s.SegId = id;
            s.transition = transition;

            segments.Insert(0, s); //always keep the last one first in list

        }
        else
        {
            segments.Remove(s);
            segments.Insert(0, s);
        }

        return s;

    }
    

    /**
     * Returns the correct piece according to the beachpiecetype and the visual Index given
     */
    public BeachPiece GetPiece(BeachPieceType pt, int visualIndex)
    {
        BeachPiece p = pieces.Find( x => x.type == pt && x.visualIndex == visualIndex && !x.gameObject.activeSelf);
        if(p == null)
        {
            GameObject go = null;
            if(pt == BeachPieceType.ramp)
            {
                go = ramps[visualIndex].gameObject;
            }
            else if (pt == BeachPieceType.longblock)
            {
                go = longblocks[visualIndex].gameObject;
            }else if (pt == BeachPieceType.jump)
            {
                go = jumps[visualIndex].gameObject;
            }else if (pt == BeachPieceType.slide)
            {
                go = slides[visualIndex].gameObject;
            }
            go = Instantiate(go);
            p = go.GetComponent<BeachPiece>();
        }

        return p;
    }


    // Start is called before the first frame update\
    void Start()
    {
        for (int i = 0; i < INITIAL_SEGMENTS; i++)
        {
            //generate a segment to fill the list
            GenerateSegment(); 
        }
    }

    private void GenerateSegment()
    {
        SpawnSegment();
        if (Random.RandomRange(0f, 1f) < (continiousSegments * 0.25f))
        {
            //spawn transition seg
            continiousSegments = 0;
            SpawnTransition();
        }
        else
        {
            continiousSegments++;
        }
 
    }

    private void SpawnSegment()
    {
        //find segment in correct line
        List<Segement> possibleSeg = availableSegments.FindAll(x => x.beginY1 == y1 || x.beginY2 == y2 || x.beginY3 == y3);
        int id = Random.Range(0, possibleSeg.Count); //randomly select a segment

        Segement s = GetSegment(id, false);

        y1 = s.endY1;
        y2 = s.endY2;
        y3 = s.endY3;

        s.transform.SetParent(transform);
        s.transform.localPosition = Vector3.forward * currentSpawnZ; //move segment to last

        currentSpawnZ += s.length;
        amountOfActiveSegments++;
        s.Spawn();
    }

    private void SpawnTransition()
    {
        List<Segement> possibleTransition = availableTransitions.FindAll(x => x.beginY1 == y1 || x.beginY2 == y2 || x.beginY3 == y3);
        int id = Random.Range(0, possibleTransition.Count);

        Segement s = GetSegment(id, false);

        y1 = s.endY1;
        y2 = s.endY2;
        y3 = s.endY3;

        s.transform.SetParent(transform);
        s.transform.localPosition = Vector3.forward * currentSpawnZ;

        currentSpawnZ += s.length;
        amountOfActiveSegments++;
        s.Spawn();
    }

    // Update is called once per frame
    void Update()
    {
        if(currentSpawnZ - cameraContainer.position.z < DISTANCE_BEFORE_SPAWN)
        {
            GenerateSegment();
        }

        if(amountOfActiveSegments >= MAX_SEGMENTS_ON_SCREEN) //if max number of segment reached in order to protect memory, we despawn pieces
        {
            segments[amountOfActiveSegments - 1].DeSpawn();
            amountOfActiveSegments--;
        }
    }
}