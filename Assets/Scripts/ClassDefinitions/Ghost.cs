using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost
{   
    public Region region;
    public Vector3 rotation_vector;
    public GameObject ghost_object;
    public bool[] boundary_flags = new bool[4]; //Keeps animation bugs from occuring; 0 = min x; 1 = max_x; 2 = min_z; 3 = max_z;
    public int type; //0 = Orange Ghost; 1 = Green Ghost
    public float death_timer; //Used to remove green ghost after x amount of time
    public float speed_multiplier; //Slows ghost speed when in Duncan's aura
    public bool slowed; //Determines if ghost is being slowed or not

    public Ghost(Vector3 pos, Vector3 rotation_vector, string name, Region region, GameObject new_ghost, int type){
        this.rotation_vector = rotation_vector;
        this.region = region;
        this.type = type;

        ghost_object = new_ghost;
        new_ghost.transform.rotation = Quaternion.Euler(rotation_vector); //Rotate ghost by rotation vector
        new_ghost.name = name;

        death_timer = 0f;
        speed_multiplier = 1f;
        slowed = false;
    }
}