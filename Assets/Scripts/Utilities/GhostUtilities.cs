using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*************** FUNCTIONS FOR CONTROLLING GHOST ATTRIBUTES & BEHAVIOR *****************/

public class GhostUtilities : MonoBehaviour
{
    private float green_ghost_rotation_vector;
    private float green_ghost_timer;
    private int green_ghost_name_count;

    void Start(){
        green_ghost_rotation_vector = 0f;
        green_ghost_timer = 0f;
        green_ghost_name_count = 0;
    }
    void FixedUpdate(){
        GameController gc = GameObject.Find("ScriptObject").GetComponent<GameController>(); //Grabs GameController script data

        for(int i = 0; i < gc.ghost_list.Count; i++){ //Loop through all the ghost

            //Move slow symbol with slowed ghost
            if(gc.ghost_list[i].slowed){
                move_slow_symbol(i);
            }

            //Destroy green ghost after fixed amount of time
            if(gc.ghost_list[i].type == 1){
                if(gc.ghost_list[i].death_timer >= 15f){
                    if(gc.ghost_list[i].slowed) destroy_slow_symbol(i); //Destroy slow symbol if ghost is slowed

                    Destroy(gc.ghost_list[i].ghost_object);
                    gc.ghost_list.RemoveAt(i);
                }
                else gc.ghost_list[i].death_timer = gc.ghost_list[i].death_timer + Time.fixedDeltaTime; //Increments ghost timer
            }
        }

        //Spawns green ghost
        if(gc.game_begun == true && gc.spawn_green_ghost == true){
            green_ghost_timer += Time.fixedDeltaTime;
            spawn_green_ghost();
        }
    }

    //Makes a new orange ghost
    public void make_ghost_oj(Vector3 pos, Vector3 rotationVector, Region region){
        GameController gc = GameObject.Find("ScriptObject").GetComponent<GameController>(); //Grabs GameController script data

        //Initialize Ghost
        Quaternion rot = new Quaternion(); //Defines initial Quanternion of ghost
        rot.Set(0f, 0f, 0f, 1f);  //Defines initial Quanternion of ghost
        GameObject new_ghost_object = Instantiate(gc.Toon_Ghost_Orange_Prefab, pos, rot);  //Create the ghost!

        string name = "ghost_oj_" + (gc.ghost_list.Count).ToString();
        Ghost new_ghost = new Ghost(pos, rotationVector, name, region, new_ghost_object, 0);
        gc.ghost_list.Add(new_ghost);

        //Add ghost to Region
        List<GameObject> new_character_list = region.character_list;
        new_character_list.Add(new_ghost.ghost_object);
        region.character_list = new_character_list;
    }

    //Makes a new green ghost
    public void make_ghost_green(Vector3 pos, Vector3 rotation_vector, Region region){
        GameController gc = GameObject.Find("ScriptObject").GetComponent<GameController>(); //Grabs GameController script data

        //Initialize Ghost
        Quaternion rot = new Quaternion(); //Defines initial Quanternion of ghost
        rot.Set(0f, 0f, 0f, 1f);  //Defines initial Quanternion of ghost
        GameObject new_ghost_object = Instantiate(gc.Toon_Ghost_Green_Prefab, pos, rot);  //Create the ghost!

        string name = "ghost_green_" + (green_ghost_name_count).ToString();
        Ghost new_ghost = new Ghost(pos, rotation_vector, name, region, new_ghost_object, 1);
        gc.ghost_list.Add(new_ghost);

        green_ghost_name_count = (green_ghost_name_count + 1) % 125;
    }

    //Moves all ghost in ghost_list such that they are within their region boundaries
    public void move_ghost(){
        GameController gc = GameObject.Find("ScriptObject").GetComponent<GameController>(); //Grabs GameController script data
        for(int i = 0; i < gc.ghost_list.Count; i++){
            GameObject ghost = gc.ghost_list[i].ghost_object;
            Region region = gc.ghost_list[i].region;
            float ghost_rot_y = gc.ghost_list[i].rotation_vector.y;

            //If green ghost, then move forward and continue
            if(gc.ghost_list[i].type == 1){
                ghost.transform.position += ghost.transform.forward * Time.fixedDeltaTime * gc.ghost_speed * gc.ghost_list[i].speed_multiplier * 1.5f;
                continue;
            }

            //Ghost hits left boundary (min_x)
            if(ghost.transform.position.x <= region.boundary_coordinates[0] && gc.ghost_list[i].boundary_flags[0] == true){
                ghost_rot_y = gc.ghost_list[i].rotation_vector.y;
                if(ghost_rot_y < 0) ghost_rot_y += 360;

                if(ghost_rot_y >= 270) gc.ghost_list[i].rotation_vector = new Vector3(0, 90f - (360f - ghost_rot_y), 0); //Ghost comes from above of normal
                else gc.ghost_list[i].rotation_vector =(new Vector3(0, 180 - (ghost_rot_y - 180), 0)); //Ghost comes from below of normal
                gc.ghost_list[i].boundary_flags[0] = false;
            }
            else{
                if(!(ghost.transform.position.x <= region.boundary_coordinates[0])) gc.ghost_list[i].boundary_flags[0] = true; //Set boundary flag if they have returned to inside the boundary
            }

            //Ghost hits right boundary (max_x)
            if(ghost.transform.position.x >= region.boundary_coordinates[1] && gc.ghost_list[i].boundary_flags[1] == true){
                ghost_rot_y = gc.ghost_list[i].rotation_vector.y;
                if(ghost_rot_y < 0) ghost_rot_y += 360;

                if(ghost_rot_y >= 90) gc.ghost_list[i].rotation_vector = new Vector3(0, 180 + (180f - ghost_rot_y), 0); //Ghost comes from above of normal
                else gc.ghost_list[i].rotation_vector = new Vector3(0, 360 - ghost_rot_y, 0); //Ghost comes from below of normal
                gc.ghost_list[i].boundary_flags[1] = false;
            }
            else{
                if(!(ghost.transform.position.x >= region.boundary_coordinates[1])) gc.ghost_list[i].boundary_flags[1] = true; //Set boundary flag if they have returned to inside the boundary
            }

            //Ghost hits lower boundary (min_z)
            if(ghost.transform.position.z <= region.boundary_coordinates[2] && gc.ghost_list[i].boundary_flags[2] == true){
                ghost_rot_y = gc.ghost_list[i].rotation_vector.y;
                if(ghost_rot_y < 0) ghost_rot_y += 360;

                if(ghost_rot_y >= 180) gc.ghost_list[i].rotation_vector = new Vector3(0, 360f - (ghost_rot_y - 180f), 0); //Ghost comes from right side of normal
                else gc.ghost_list[i].rotation_vector = new Vector3(0, (180f - ghost_rot_y), 0); //Ghost comes from left side of normal
                gc.ghost_list[i].boundary_flags[2] = false;
            }
            else{
                if(!(ghost.transform.position.z <= region.boundary_coordinates[2])) gc.ghost_list[i].boundary_flags[2] = true; //Set boundary flag if they have returned to inside the boundary
            }

            //Ghost hits upper boundary (max_z)
            if(ghost.transform.position.z >= region.boundary_coordinates[3] && gc.ghost_list[i].boundary_flags[3] == true){
                ghost_rot_y = gc.ghost_list[i].rotation_vector.y;
                if(ghost_rot_y < 0) ghost_rot_y += 360;

                if(ghost_rot_y >= 270) gc.ghost_list[i].rotation_vector = new Vector3(0, 180 + (360f - ghost_rot_y), 0); //Ghost comes from right side of normal
                else gc.ghost_list[i].rotation_vector = new Vector3(0, 180 - ghost_rot_y, 0); //Ghost comes from left side of normal
                gc.ghost_list[i].boundary_flags[3] = false;
            }
            else{
                if(!(ghost.transform.position.z >= region.boundary_coordinates[3])) gc.ghost_list[i].boundary_flags[3] = true; //Set boundary flag if they have returned to inside the boundary
            }

            //Implement rotation and movement of orange ghost
            ghost.transform.rotation = Quaternion.Euler(gc.ghost_list[i].rotation_vector);
            ghost.transform.position += ghost.transform.forward * Time.fixedDeltaTime * gc.ghost_list[i].speed_multiplier * gc.ghost_speed;
        }
    }

    //Make a green ghost in middle of map rotated in a specific direction
    void spawn_green_ghost(){
        GameController gc = GameObject.Find("ScriptObject").GetComponent<GameController>(); //Grabs GameController script data

        if(green_ghost_timer > 0.1f){ //Spawns the ghost based on this timer
            make_ghost_green(new Vector3(35f, 0f, -40f), new Vector3(0f, green_ghost_rotation_vector, 0f), gc.region_list[0]);
            green_ghost_timer = 0f;
            green_ghost_rotation_vector = (green_ghost_rotation_vector + Random.Range(15, 25)) % 360;
        }
    }

    //Destroys the slow symbol for a given ghost index
    public void destroy_slow_symbol(int i){
        GameController gc = GameObject.Find("ScriptObject").GetComponent<GameController>(); //Grabs GameController script data

        for(int j = 0; j < gc.slowed_ghost_list.Count; j++){
            if(gc.slowed_ghost_list[j].Item1 == gc.ghost_list[i].ghost_object.name){ //Destroy slow symbol
                Destroy(gc.slowed_ghost_list[j].Item2);
                gc.slowed_ghost_list.RemoveAt(j);
            }
        }
    }

    //Moves the slow symbol for a given ghost index
    void move_slow_symbol(int i){
        GameController gc = GameObject.Find("ScriptObject").GetComponent<GameController>(); //Grabs GameController script data

        for(int j = 0; j < gc.slowed_ghost_list.Count; j++){
            if(gc.slowed_ghost_list[j].Item1 == gc.ghost_list[i].ghost_object.name){ //Move the symbol to over
                gc.slowed_ghost_list[j].Item2.transform.position =  new Vector3(gc.ghost_list[i].ghost_object.transform.position.x, 1.5f, gc.ghost_list[i].ghost_object.transform.position.z);
            }
        }
    }
}
