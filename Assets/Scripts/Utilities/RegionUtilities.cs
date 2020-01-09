using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*************** FUNCTIONS FOR CONTROLLING REGION ATTRIBUTES & BEHAVIOR *****************/

public class RegionUtilities : MonoBehaviour
{
    //Populates region_list with all of the regions
    public void initialize_region_list(int level){
        GameController gc = GameObject.Find("ScriptObject").GetComponent<GameController>(); //Grabs GameController script data

        if(level == 0){ //Main Screen
            Region r0 = new Region(-10f, 0f, 0f, 20f, true, -1, 1, 0);
            gc.region_list.Add(r0);
            Region r1 = new Region(0f, 20f, 0f, 20f, false, 0, 1, 15);
            gc.region_list.Add(r1);
            Region r2 = new Region(20f, 30f, 0f, 20f, true, 0, -1, 0);
            gc.region_list.Add(r2);
        }
        
        else if(level == 1){ //Level one init
            Region r0 = new Region(0f, 10f, 0f, 10f, true, -1, 1, 0);
            gc.region_list.Add(r0);
            Region r1 = new Region(10f, 70f, 0f, 10f, false, 0, 1, 0);
            gc.region_list.Add(r1);
            Region r2 = new Region(70f, 80f, 0f, 10f, true, 0, 2, 0);
            gc.region_list.Add(r2);
            Region r3 = new Region(70f, 80f, -60f, 0f, false, 3, 2, 0);
            gc.region_list.Add(r3);
            Region r4 = new Region(70f, 80f, -70f, -60f, true, 3, 0, 0);
            gc.region_list.Add(r4);
            Region r5 = new Region(10f, 70f, -70f, -60f, false, 1, 0, 0);
            gc.region_list.Add(r5);
            Region r6 = new Region(0f, 10f, -70f, -60f, true, 1, 3, 0);
            gc.region_list.Add(r6);
            Region r7 = new Region(0f, 10f, -60f, -20f, false, 2, 3, 0);
            gc.region_list.Add(r7);
            Region r8 = new Region(0f, 10f, -20f, -10f, true, 2, 1, 0);
            gc.region_list.Add(r8);
            Region r9 = new Region(10f, 50f, -20f, -10f, false, 0, 1, 0);
            gc.region_list.Add(r9);
            Region r10 = new Region(50f, 60f, -20f, -10f, true, 0, 2, 0);
            gc.region_list.Add(r10);
            Region r11 = new Region(50f, 60f, -30f, -20f, false, 3, 2, 0);
            gc.region_list.Add(r11);
            Region r12 = new Region(50f, 60f, -50f, -30f, true, 3, 0, 0);
            gc.region_list.Add(r12);
            Region r13 = new Region(20f, 50f, -50f, -30f, false, 1, 0, 0);
            gc.region_list.Add(r13);
            Region r14 = new Region(15f, 20f, -42.5f, -37.5f, true, 1, -1, 0);
            gc.region_list.Add(r14);
        }
        else if(level == 2){ //Level two init
            Region r0 = new Region(0f, 10f, 0f, 10f, true, -1, 1, 0);
            gc.region_list.Add(r0);
            Region r1 = new Region(10f, 70f, 0f, 10f, false, 0, 1, 25);
            gc.region_list.Add(r1);
            Region r2 = new Region(70f, 80f, 0f, 10f, true, 0, 2, 0);
            gc.region_list.Add(r2);
            Region r3 = new Region(70f, 80f, -60f, 0f, false, 3, 2, 25);
            gc.region_list.Add(r3);
            Region r4 = new Region(70f, 80f, -70f, -60f, true, 3, 0, 0);
            gc.region_list.Add(r4);
            Region r5 = new Region(10f, 70f, -70f, -60f, false, 1, 0, 30);
            gc.region_list.Add(r5);
            Region r6 = new Region(0f, 10f, -70f, -60f, true, 1, 3, 0);
            gc.region_list.Add(r6);
            Region r7 = new Region(0f, 10f, -60f, -20f, false, 2, 3, 25);
            gc.region_list.Add(r7);
            Region r8 = new Region(0f, 10f, -20f, -10f, true, 2, 1, 0);
            gc.region_list.Add(r8);
            Region r9 = new Region(10f, 50f, -20f, -10f, false, 0, 1, 25);
            gc.region_list.Add(r9);
            Region r10 = new Region(50f, 60f, -20f, -10f, true, 0, 2, 0);
            gc.region_list.Add(r10);
            Region r11 = new Region(50f, 60f, -30f, -20f, false, 3, 2, 10);
            gc.region_list.Add(r11);
            Region r12 = new Region(50f, 60f, -50f, -30f, true, 3, 0, 0);
            gc.region_list.Add(r12);
            Region r13 = new Region(20f, 50f, -50f, -30f, false, 1, 0, 50);
            gc.region_list.Add(r13);
            Region r14 = new Region(15f, 20f, -42.5f, -37.5f, true, 1, -1, 0);
            gc.region_list.Add(r14);
        }
        else if(level == 3){ //Level three init
            Region r0 = new Region(0f, 10f, 0f, 10f, true, -1, 1, 0);
            gc.region_list.Add(r0);
            Region r1 = new Region(10f, 70f, 0f, 10f, false, 0, 1, 35);
            gc.region_list.Add(r1);
            Region r2 = new Region(70f, 80f, 0f, 10f, true, 0, 2, 0);
            gc.region_list.Add(r2);
            Region r3 = new Region(70f, 80f, -60f, 0f, false, 3, 2, 35);
            gc.region_list.Add(r3);
            Region r4 = new Region(70f, 80f, -70f, -60f, true, 3, 0, 0);
            gc.region_list.Add(r4);
            Region r5 = new Region(10f, 70f, -70f, -60f, false, 1, 0, 40);
            gc.region_list.Add(r5);
            Region r6 = new Region(0f, 10f, -70f, -60f, true, 1, 3, 0);
            gc.region_list.Add(r6);
            Region r7 = new Region(0f, 10f, -60f, -20f, false, 2, 3, 35);
            gc.region_list.Add(r7);
            Region r8 = new Region(0f, 10f, -20f, -10f, true, 2, 1, 0);
            gc.region_list.Add(r8);
            Region r9 = new Region(10f, 50f, -20f, -10f, false, 0, 1, 35);
            gc.region_list.Add(r9);
            Region r10 = new Region(50f, 60f, -20f, -10f, true, 0, 2, 0);
            gc.region_list.Add(r10);
            Region r11 = new Region(50f, 60f, -30f, -20f, false, 3, 2, 10);
            gc.region_list.Add(r11);
            Region r12 = new Region(50f, 60f, -50f, -30f, true, 3, 0, 0);
            gc.region_list.Add(r12);
            Region r13 = new Region(20f, 50f, -50f, -30f, false, 1, 0, 30);
            gc.region_list.Add(r13);
            Region r14 = new Region(15f, 20f, -42.5f, -37.5f, true, 1, -1, 0);
            gc.region_list.Add(r14);
        }
    }

    //Populates Regions with orange ghost
    public void fill_regions(){
        GameController gc = GameObject.Find("ScriptObject").GetComponent<GameController>(); //Grabs GameController script data
        GhostUtilities gu = GameObject.Find("ScriptObject").GetComponent<GhostUtilities>(); //Grabs GameUtilities script data

        for(int i = 0; i < gc.region_list.Count; i++){
            if(gc.region_list[i].safe_zone == false){
                for(int j = 0; j < gc.region_list[i].ghost_number; j++){ //Creates j number of ghost
                float[] boundaries = gc.region_list[i].boundary_coordinates;

                Vector3 pos = new Vector3(Random.Range(boundaries[0] + 0.1f, boundaries[1] - 0.1f), 0f, Random.Range(boundaries[2] + 0.1f, boundaries[3] - 0.1f)); //Defines initial position of ghost by picking random point in the region boundary (with a small error)
                Vector3 rotationVector = new Vector3(0f, Random.Range(0, 360), 0f); //Rotate ghost randomly

                gu.make_ghost_oj(pos, rotationVector, gc.region_list[i]); //Make new ghost at specified location
                }
            }
        }
    }
}
