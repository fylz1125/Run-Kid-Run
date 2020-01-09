using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Region
{
    public List<GameObject> character_list = new List<GameObject>(); //Stores all ghost and player objects within region
    public float[] boundary_coordinates = new float[4]; //0 = min x; 1 = max_x; 2 = min_z; 3 = max_z;
    public bool safe_zone; //Determines if region is a safe zone
    public int region_up; //Index of the boundary of the next region
    public int region_down; //Index of the boundary of the previous region
    public int ghost_number; //Number of ghost region spawns

    //Initializes the region with boundary coordinates
    public Region(float[] new_coords, bool sz, int down, int up, int ghost_number){
        boundary_coordinates = new_coords;
        safe_zone = sz;
        region_up = up;
        region_down = down;
        this.ghost_number = ghost_number;
    }

    public Region(float min_x, float max_x, float min_z, float max_z, bool sz, int down, int up, int ghost_number){
        float[] new_coords = new float[4];
        new_coords[0] = min_x; new_coords[1] = max_x; new_coords[2] = min_z; new_coords[3] = max_z;
        boundary_coordinates = new_coords;
        safe_zone = sz;
        region_up = up;
        region_down = down;
        this.ghost_number = ghost_number;
    }
}
