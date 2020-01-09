using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*************** FUNCTIONS FOR CONTROLLING KID ATTRIBUTES & BEHAVIOR *****************/
public class KidUtilities : MonoBehaviour
{
    //Determines if a ghost has captured the kid
    public void check_collisions(){
        GameController gc = GameObject.Find("ScriptObject").GetComponent<GameController>(); //Grabs GameController script data
        GhostUtilities gu = GameObject.Find("ScriptObject").GetComponent<GhostUtilities>();

        for(int k = 0; k < gc.kid_list.Count; k++){ //For each kid
            for(int i = 0; i < gc.ghost_list.Count; i++){ //Calculate Euclidean distance between each ghost and kid
                float euclidean = Mathf.Sqrt(Mathf.Pow((gc.kid_list[k].kid_object.transform.position.x - gc.ghost_list[i].ghost_object.transform.position.x), 2f) + Mathf.Pow((gc.kid_list[k].kid_object.transform.position.z - gc.ghost_list[i].ghost_object.transform.position.z), 2f));

                //If ghost leaves Duncan's aura, then remove the slow effects
                if(gc.kid_list[k].character_number == 1 && euclidean >= 5f && gc.ghost_list[i].speed_multiplier < 1f) {
                    gc.ghost_list[i].speed_multiplier = 1f; //If out of Duncan aura -> reset speed
                    gc.ghost_list[i].slowed = false;
                    gu.destroy_slow_symbol(i); //Destroy slow symbol
                }

                //If ghost within Duncan slow aura, reduce the speed multiplier
                if(gc.kid_list[k].character_number == 1 && euclidean < 5f && gc.ghost_list[i].speed_multiplier >= 1){
                    gc.ghost_list[i].speed_multiplier = 0.83334f - (0.005f * (float)gc.kid_list[k].power_upgrades); //If in Duncan aura -> slow speed

                    if(!gc.ghost_list[i].slowed){ //If slow symbol not there, then add it to Ghost
                        gc.ghost_list[i].slowed = true;

                        //Make slow symbol over ghost and remove unwanted properties
                        GameObject slow_symbol = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        SphereCollider sc = slow_symbol.GetComponent<SphereCollider>();
                        sc.enabled = false;

                        //Define position properties for slow symbol
                        Vector3 pos = gc.ghost_list[i].ghost_object.transform.position;
                        pos.y += 1.5f;
                        Quaternion rot = new Quaternion(); rot.Set(0f, 0f, 0f, 1f);
                        slow_symbol.transform.position = pos;
                        slow_symbol.transform.rotation = rot;

                        //Scale and place material on slow symbol
                        slow_symbol.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
                        slow_symbol.GetComponent<Renderer>().material = gc.slow_symbol_material;

                        //Add the slowed ghost to list
                        gc.slowed_ghost_list.Add((gc.ghost_list[i].ghost_object.name, slow_symbol));
                    }
                }

                if(gc.kid_list[k].region.safe_zone == false && euclidean < 0.715f) respawn(k); //Respawn when hit in non-safe area
            }
        }
    }

    //Respawns kid (kid_list[k]) at last checkpoint
    public void respawn(int k){
        GameController gc = GameObject.Find("ScriptObject").GetComponent<GameController>(); //Grabs GameController script data
        GhostUtilities gu = GameObject.Find("ScriptObject").GetComponent<GhostUtilities>();

        if(gc.kid_list[k].region_index % 2 != 0){ //If not in safe zone, then set the kid_region to the last safe zone
            gc.kid_list[k].region = gc.region_list[gc.kid_list[k].region_index - 1];
            gc.kid_list[k].region_index -= 1;
        }

        //Place kid in middle of last safe zone
        gc.kid_list[k].kid_object.transform.position = new Vector3(gc.kid_list[k].region.boundary_coordinates[1] - (gc.kid_list[k].region.boundary_coordinates[1] - gc.kid_list[k].region.boundary_coordinates[0]) / 2f, 0,  
        gc.kid_list[k].region.boundary_coordinates[3] - ((gc.kid_list[k].region.boundary_coordinates[3] - gc.kid_list[k].region.boundary_coordinates[2]) / 2f) - 2 * k);

        Vector3 rotationVector; //Rotate kid
        if(gc.kid_list[k].region.region_up == 0) rotationVector = new Vector3(0f, 270f, 0f);
        else if(gc.kid_list[k].region.region_up == 1) rotationVector = new Vector3(0f, 90f, 0f);
        else if(gc.kid_list[k].region.region_up == 2) rotationVector = new Vector3(0f, 180f, 0f);
        else rotationVector = new Vector3(0f, 0f, 0f);
        gc.kid_list[k].kid_object.transform.eulerAngles = rotationVector;

        gc.kid_list[k].animator.SetBool("isRunning", false);
        gc.kid_list[k].animator.SetBool("isBackingUp", false);

        gc.kid_list[k].falling = false;
    }

    //Makes the kid for the player to use
    public void make_kid(){
        GameController gc = GameObject.Find("ScriptObject").GetComponent<GameController>(); //Grabs GameController script data

        //Defines initial position
        Vector3 pos;
        if(gc.main_menu_active == true) pos = new Vector3(-5f, 0f, 10f);
        else pos = new Vector3(5f, 0f, 5f);
        
        Quaternion rot = new Quaternion(); //Defines initial Quanternion of kid
        rot.Set(0f, 0f, 0f, 1f);  //Defines initial Quanternion of kid

        /********** TESTING ***********/
        if(Scene_Switch_Data.DEVELOPER_spawn_last_region) pos = new Vector3(50f, 0f, -40f);
        /********** END TESTING ***********/

        //Make the kids!
        for(int i = 0; i < Scene_Switch_Data.character_number_list.Count; i++){
            pos += new Vector3(0f, 0f, 1.5f * i);
            Kid new_kid;

            if(Scene_Switch_Data.character_number_list[i] == 0) { //If current kid is chosen to be Cardie
                new_kid = new Kid(Instantiate(gc.kid_prefab_cardie, pos, rot), 0, i + 1);
                new_kid.sprint_available = true;
            }
            else { //If current kid is chosen to be Duncan
                new_kid = new Kid(Instantiate(gc.kid_prefab_duncan, pos, rot), 1, i + 1);
            }

            gc.kid_list.Add(new_kid);

            new_kid.region = gc.region_list[0]; //Assign region to the kid
            new_kid.region_index = 0;
            new_kid.animator = new_kid.kid_object.GetComponent<Animator> (); //Assigns the animator to the kid

            //Fill region flag list of every kid
            for(int k = 0; k < gc.kid_list.Count; k++){
                for(int j = 0; j < gc.region_list.Count; j++) gc.kid_list[k].region_flags_list.Add(false);
            }

            /********** TESTING ***********/
            
            if(Scene_Switch_Data.DEVELOPER_spawn_last_region){
                new_kid.region = gc.region_list[12];
                new_kid.region_index = 12;
                new_kid.upgrade_points = 15;
                new_kid.speed = 6.0f;
            } 

            /********** END TESTING ***********/
        }

        for(int k = 0; k < gc.kid_list.Count; k++){ //For each kid
            Vector3 rotationVector; //Rotate kid in the correct direction
            if(gc.kid_list[k].region.region_up == 0) rotationVector = new Vector3(0f, 270f, 0f);
            else if(gc.kid_list[k].region.region_up == 1) rotationVector = new Vector3(0f, 90f, 0f);
            else if(gc.kid_list[k].region.region_up == 2) rotationVector = new Vector3(0f, 180f, 0f);
            else rotationVector = new Vector3(0f, 0f, 0f);
            gc.kid_list[k].kid_object.transform.eulerAngles = rotationVector;
        }
    }

    //Determines next kid position
    public void move_kid(){
        GameController gc = GameObject.Find("ScriptObject").GetComponent<GameController>(); //Grabs GameController script data

        if(gc.level_end_timer > 0 && gc.level_complete == true) return;

        for(int k = 0; k < gc.kid_list.Count; k++){ //For each kid

            if(gc.kid_list[k].falling == true) { //If kid is falling, then make him fall
                gc.kid_list[k].kid_object.transform.position -= new Vector3(0f, 0.1f, 0f);
                if(gc.kid_list[k].kid_object.transform.position.y < -3f) respawn(k);
                continue;
            }
            
            //Move the kid forward
            bool isRunningPressed;
            if(Scene_Switch_Data.game_type == 1 && k == 0) isRunningPressed = Input.GetKey(KeyCode.W);
            else if(Scene_Switch_Data.game_type == 1 && k == 1) isRunningPressed = Input.GetKey(KeyCode.UpArrow);
            else isRunningPressed = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);

            //Adjust animation and position
            gc.kid_list[k].animator.SetBool("isRunning", isRunningPressed);
            if(isRunningPressed) gc.kid_list[k].kid_object.transform.position += gc.kid_list[k].kid_object.transform.forward * Time.fixedDeltaTime * gc.kid_list[k].speed;
            

            //Move the kid backward
            bool isBackingUpPressed;
            if(Scene_Switch_Data.game_type == 1 && k == 0) isBackingUpPressed = Input.GetKey(KeyCode.S);
            else if(Scene_Switch_Data.game_type == 1 && k == 1) isBackingUpPressed = Input.GetKey(KeyCode.DownArrow);
            else isBackingUpPressed = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);

            //Adjust animation and position
            gc.kid_list[k].animator.SetBool("isBackingUp", isBackingUpPressed);
            if(isBackingUpPressed && !isRunningPressed) gc.kid_list[k].kid_object.transform.position -= gc.kid_list[k].kid_object.transform.forward * Time.fixedDeltaTime * gc.kid_list[k].speed * 2 / 3;

            //Determines if character is rotating to the right
            bool isRotatingRightPressed;
            if(Scene_Switch_Data.game_type == 1 && k == 0) isRotatingRightPressed = Input.GetKey(KeyCode.D);
            else if(Scene_Switch_Data.game_type == 1 && k == 1) isRotatingRightPressed = Input.GetKey(KeyCode.RightArrow);
            else isRotatingRightPressed = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);

            if(isRotatingRightPressed) gc.kid_list[k].kid_object.transform.Rotate(0, 2 * gc.kid_list[k].speed, 0, Space.World);

            //Determines if character is rotating to the left
            bool isRotatingLeftPressed;
            if(Scene_Switch_Data.game_type == 1 && k == 0) isRotatingLeftPressed = Input.GetKey(KeyCode.A);
            else if(Scene_Switch_Data.game_type == 1 && k == 1) isRotatingLeftPressed = Input.GetKey(KeyCode.LeftArrow);
            else isRotatingLeftPressed = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);

            if(isRotatingLeftPressed) gc.kid_list[k].kid_object.transform.Rotate(0, -2 * gc.kid_list[k].speed, 0, Space.World);

            //Determines if to activate sprint
            bool q_Pressed;
            if(Scene_Switch_Data.game_type == 1 && k == 1) q_Pressed = Input.GetKey(KeyCode.RightControl);
            else q_Pressed = Input.GetKey(KeyCode.Q);

            //Activate Sprint
            if(q_Pressed == true && gc.kid_list[k].sprint_available == true && gc.kid_list[k].sprint_active == false && gc.kid_list[k].sprint_cooldown <= 0f){
                gc.kid_list[k].sprint_bonus = (gc.kid_list[k].speed / 2f) + gc.kid_list[k].speed * gc.kid_list[k].ability_power;
                gc.kid_list[k].speed += gc.kid_list[k].sprint_bonus;
                gc.kid_list[k].sprint_active = true;
                gc.kid_list[k].sprint_timer = 3f;
            }

            //Move corresponding camera to player's character
            if(Scene_Switch_Data.game_type == 1){
                gc.P1_camera.transform.position = new Vector3(gc.kid_list[0].kid_object.transform.position.x, 7f, gc.kid_list[0].kid_object.transform.position.z - 9f);
                gc.P1_camera.transform.rotation = Quaternion.Euler(new Vector3(40f, 0f, 0f));

                gc.P2_camera.transform.position = new Vector3(gc.kid_list[1].kid_object.transform.position.x, 7f, gc.kid_list[1].kid_object.transform.position.z - 9f);
                gc.P2_camera.transform.rotation = Quaternion.Euler(new Vector3(40f, 0f, 0f));
            }
            else{
                gc.main_camera.transform.position = new Vector3(gc.kid_list[0].kid_object.transform.position.x, 7f, gc.kid_list[0].kid_object.transform.position.z - 9f);
                gc.main_camera.transform.rotation = Quaternion.Euler(new Vector3(40f, 0f, 0f));
            }

            //Check if kid transitioning into new region or fell off the platform
            bool cross_min_x = gc.kid_list[k].kid_object.transform.position.x < gc.kid_list[k].region.boundary_coordinates[0];
            bool cross_max_x = gc.kid_list[k].kid_object.transform.position.x > gc.kid_list[k].region.boundary_coordinates[1];
            bool cross_min_z = gc.kid_list[k].kid_object.transform.position.z < gc.kid_list[k].region.boundary_coordinates[2];
            bool cross_max_z = gc.kid_list[k].kid_object.transform.position.z > gc.kid_list[k].region.boundary_coordinates[3];

            //If next region is smaller than current, need to check if kid is within the other boundaries
            //Example: last region of level 1 -> If player crosses min_x, need to check if within z-boundaries of last region
            bool in_bounds_min_x = false;
            bool in_bounds_max_x = false;
            bool in_bounds_min_z = false;
            bool in_bounds_max_z = false;

            if(gc.kid_list[k].region_index != gc.region_list.Count - 1){
                in_bounds_min_x = gc.kid_list[k].kid_object.transform.position.x > gc.region_list[gc.kid_list[k].region_index + 1].boundary_coordinates[0];
                in_bounds_max_x = gc.kid_list[k].kid_object.transform.position.x < gc.region_list[gc.kid_list[k].region_index + 1].boundary_coordinates[1];
                in_bounds_min_z = gc.kid_list[k].kid_object.transform.position.z > gc.region_list[gc.kid_list[k].region_index + 1].boundary_coordinates[2];
                in_bounds_max_z = gc.kid_list[k].kid_object.transform.position.z < gc.region_list[gc.kid_list[k].region_index + 1].boundary_coordinates[3];
            }

            bool x_in_bounds = in_bounds_min_x & in_bounds_max_x;
            bool z_in_bounds = in_bounds_min_z & in_bounds_max_z;

            //If any boundaries were crosses
            if(cross_min_x == true || cross_max_x == true || cross_min_z == true || cross_max_z == true){
                int curr_up = gc.kid_list[k].region.region_up;
                int curr_down = gc.kid_list[k].region.region_down;

                //For each boundary, check if to increase or decreased region number
                //Check min_x
                if(cross_min_x == true && 0 == curr_up && gc.kid_list[k].region_index < gc.region_list.Count - 1 && z_in_bounds){
                    gc.kid_list[k].region = gc.region_list[gc.kid_list[k].region_index + 1];
                    gc.kid_list[k].region_index += 1;
                    //Debug.Log("Region Increased");

                    //If new checkpoint is reached, modify flags
                    if(gc.kid_list[k].region.safe_zone == true && gc.kid_list[k].region_flags_list[gc.kid_list[k].region_index] == false) {
                        gc.kid_list[k].upgrade_points += 1;
                        gc.kid_list[k].region_flags_list[gc.kid_list[k].region_index] = true;
                    }
                }
                else if(cross_min_x == true && 0 == curr_down && gc.kid_list[k].region_index > 0){
                    gc.kid_list[k].region = gc.region_list[gc.kid_list[k].region_index - 1];
                    gc.kid_list[k].region_index -= 1;
                    //Debug.Log("Region Decreased");
                }

                //Check max_x
                else if(cross_max_x == true && 1 == curr_up && gc.kid_list[k].region_index < gc.region_list.Count - 1 && z_in_bounds){
                    gc.kid_list[k].region = gc.region_list[gc.kid_list[k].region_index + 1];
                    gc.kid_list[k].region_index += 1;
                    //Debug.Log("Region Increased");

                    if(gc.kid_list[k].region.safe_zone == true && gc.kid_list[k].region_flags_list[gc.kid_list[k].region_index] == false) {
                        gc.kid_list[k].upgrade_points += 1;
                        gc.kid_list[k].region_flags_list[gc.kid_list[k].region_index] = true;
                    }
                }
                else if(cross_max_x == true && 1 == curr_down && gc.kid_list[k].region_index > 0){
                    gc.kid_list[k].region = gc.region_list[gc.kid_list[k].region_index - 1];
                    gc.kid_list[k].region_index -= 1;
                    //Debug.Log("Region Decreased");
                }

                //Check min_z
                else if(cross_min_z == true && 2 == curr_up && gc.kid_list[k].region_index < gc.region_list.Count - 1 && x_in_bounds){
                    gc.kid_list[k].region = gc.region_list[gc.kid_list[k].region_index + 1];
                    gc.kid_list[k].region_index += 1;
                    //Debug.Log("Region Increased");

                    if(gc.kid_list[k].region.safe_zone == true && gc.kid_list[k].region_flags_list[gc.kid_list[k].region_index] == false) {
                        gc.kid_list[k].upgrade_points += 1;
                        gc.kid_list[k].region_flags_list[gc.kid_list[k].region_index] = true;
                    }
                }
                else if(cross_min_z == true && 2 == curr_down && gc.kid_list[k].region_index > 0){
                    gc.kid_list[k].region = gc.region_list[gc.kid_list[k].region_index - 1];
                    gc.kid_list[k].region_index -= 1;
                    //Debug.Log("Region Decreased");
                }

                //Check max_z
                else if(cross_max_z == true && 3 == curr_up && gc.kid_list[k].region_index < gc.region_list.Count - 1 && x_in_bounds){
                    gc.kid_list[k].region = gc.region_list[gc.kid_list[k].region_index + 1];
                    gc.kid_list[k].region_index += 1;
                    //Debug.Log("Region Increased");

                    if(gc.kid_list[k].region.safe_zone == true && gc.kid_list[k].region_flags_list[gc.kid_list[k].region_index] == false) {
                        gc.kid_list[k].upgrade_points += 1;
                        gc.kid_list[k].region_flags_list[gc.kid_list[k].region_index] = true;
                    }
                }
                else if(cross_max_z == true && 3 == curr_down && gc.kid_list[k].region_index > 0){
                    gc.kid_list[k].region = gc.region_list[gc.kid_list[k].region_index - 1];
                    gc.kid_list[k].region_index -= 1;
                    //Debug.Log("Region Decreased");
                }

                //The player fell off
                else{gc.kid_list[k].falling = true;}
            }
        }
    }
}
