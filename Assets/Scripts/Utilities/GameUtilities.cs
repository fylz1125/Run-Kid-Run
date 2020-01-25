using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


/*************** FUNCTIONS FOR CONTROLLING GENERAL GAMEPLAY *****************/
public class GameUtilities : MonoBehaviour
{
    //Sets game UI based on settings menu values
    public void adjustPlayerUI(){
        GameController gc = GameObject.Find("ScriptObject").GetComponent<GameController>(); //Grabs GameController script data

        //Adjust minimap size
        Game_Settings.minimap_size_multiplier = gc.minimap_size_slider.value * 2f;
        if(gc.minimap_object != null){
            if(gc.minimap_anchors_set < 2){
                StartCoroutine(setAnchors());
                return;
            }
            gc.minimap_object.transform.localScale = new Vector3(Game_Settings.minimap_size_multiplier, Game_Settings.minimap_size_multiplier, 1);

            float new_position = 100f * Game_Settings.minimap_size_multiplier;
            //gc.minimap_object.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(1f,0f);

            gc.minimap_object.transform.localPosition = new Vector3(-1f * new_position + Screen.width / 2 + 95, new_position - Screen.height / 2 - 48, 0);

            Debug.Log(gc.minimap_object.transform.localPosition.x);
            //Debug.Log(Screen.width);
            //Debug.Log(Screen.height);
            //Debug.Log(new_position);
            //Debug.Log("Game Settings: " + Game_Settings.minimap_center_anchor_y);
            //Debug.Log("Position: " + gc.minimap_object.transform.position.x);
            //Debug.Log("Game Settings: " + Game_Settings.minimap_center_anchor_x);
            //Debug.Log("Y Assignment: " + new_position);
            //Debug.Log("Multiplier: " + Game_Settings.minimap_size_multiplier + "; X: " + gc.minimap_object.transform.localPosition.x + "; Y: " + gc.minimap_object.transform.localPosition.y);
            //Debug.Log("Multiplier: " + Game_Settings.minimap_size_multiplier + "; X: " + gc.minimap_object.transform.position.x + "; Y: " + gc.minimap_object.transform.position.y);
        }
        
    }

    IEnumerator setAnchors(){
        GameController gc = GameObject.Find("ScriptObject").GetComponent<GameController>(); //Grabs GameController script data
        yield return new WaitForSecondsRealtime(0.04f);
        Game_Settings.minimap_center_anchor_x = gc.minimap_object.transform.position.x;
        Game_Settings.minimap_center_anchor_y = gc.minimap_object.transform.position.y;
        Debug.Log("anchor set: " + gc.minimap_anchors_set);
        gc.minimap_anchors_set += 1;
    }

    //Pauses the game by stopping all movement
    public void pause_game(){
        GameController gc = GameObject.Find("ScriptObject").GetComponent<GameController>(); //Grabs GameController script data
        bool isPausePressed = Input.GetKeyDown(KeyCode.Escape);

        if(isPausePressed == true){ //If player (un)pauses
            gc.game_paused = !gc.game_paused;
            if(gc.game_paused == true){ //Pause game
                Time.timeScale = 0;
                gc.pause_panel.SetActive(true);
            }
            else{ //Unpause game
                Time.timeScale = 1;
                gc.pause_panel.SetActive(false);
            }
        }
    }

    //Unpauses game by button click
    public void onResumePressed(){
        GameController gc = GameObject.Find("ScriptObject").GetComponent<GameController>(); //Grabs GameController script data
        Time.timeScale = 1;
        gc.pause_panel.SetActive(false);
        gc.pause_timeout = -1f;
        gc.game_paused = false;
    }

    //Updates text boxes
    public void updateUIText(){
        GameController gc = GameObject.Find("ScriptObject").GetComponent<GameController>(); //Grabs GameController script data

        if(gc.kid_list.Count > 0){
            gc.upgrade_text.text = gc.kid_list[0].upgrade_points.ToString();
            gc.speed_text.text = "Speed [" + gc.kid_list[0].speed_upgrades.ToString() + "]";
            gc.power_text.text = "Power [" + gc.kid_list[0].power_upgrades.ToString() + "]";

            if(gc.kid_list.Count > 1){
                gc.upgrade_text_P2.text = gc.kid_list[1].upgrade_points.ToString();
                gc.speed_text_P2.text = "Speed [" + gc.kid_list[1].speed_upgrades.ToString() + "]";
                gc.power_text_P2.text = "Power [" + gc.kid_list[1].power_upgrades.ToString() + "]";

            }
        }
    }



    /**********************  LEVEL FUNCTIONS *********************/
    //transitions from initial text into the game 
    public void level_begin(){
        GameController gc = GameObject.Find("ScriptObject").GetComponent<GameController>(); //Grabs GameController script data

        //Transition camera to new position
        gc.main_camera.transform.eulerAngles = gc.main_camera.transform.eulerAngles + new Vector3(0.5f, 0f, 0f); //Transition camera to new position
        gc.plot_song.volume -= 0.0075f;

        if(gc.main_camera.transform.eulerAngles.x >= 40 && gc.main_camera.transform.eulerAngles.x < 45) {
            gc.game_begun = true;

            setup_level_enemies();
            gc.kid_utilities.make_kid();

            //Activate split screen cameras if playing split screen mode
            if(Scene_Switch_Data.game_type == 1) EnableSplitScreen();

            for(int k = 0; k < gc.kid_list.Count; k++){
              if(gc.kid_list[k].character_number == 0){
                  gc.kid_list[k].sprint_available = true; //Enable sprint if current player chose Cardie  
                  if(k == 0){
                    gc.sprint_image.gameObject.SetActive(true);
                    gc.q_text.gameObject.SetActive(true);
                  }
                  else{
                    gc.sprint_image_P2.gameObject.SetActive(true);
                    gc.q_text_P2.gameObject.SetActive(true);
                  }
              }
            }

            gc.plot_song.Stop();

            //Enable Upgrade UI
            gc.upgrade_button_on.gameObject.SetActive(true);
            gc.upgrade_text.gameObject.SetActive(true);
            gc.minimap_object.SetActive(true);

            //Play voiceline
            if(gc.level == 1){
                gc.vl_start_1_object.GetComponent<AudioSource>().Play();
                gc.vl_start_1_text.gameObject.SetActive(true);
            }
        }
    }

    //Cleans up level one once completed
    public void end_level(){
        if(Scene_Switch_Data.level == 0) return; //Do nothing if in tutorial mode

        GameController gc = GameObject.Find("ScriptObject").GetComponent<GameController>(); //Grabs GameController script data
        GhostUtilities gu = GameObject.Find("ScriptObject").GetComponent<GhostUtilities>();
        KidUtilities ku = GameObject.Find("ScriptObject").GetComponent<KidUtilities>();

        if(gc.level + 1 >= 4) onMainMenuClick(); //Sends player back to main screen when current levels completed

        //Delete all ghost from ghost_list and and destroy the objects of every ghost in all regions
        for(int i = 0; i < gc.ghost_list.Count; i++){
            gu.destroy_slow_symbol(i);
            Destroy(gc.ghost_list[i].ghost_object);
        }
        gc.ghost_list.Clear();
        gc.region_list.Clear();

        gc.level += 1;
        setup_level_enemies();

        for(int k = 0; k < gc.kid_list.Count; k++){ //For each kid in kid_list
            gc.kid_list[k].region_index = 0;
            gc.kid_list[k].region = gc.region_list[0];
            gc.kid_list[k].region_flags_list.Clear();

            //Fill region flag list of every kid
            for(int j = 0; j < gc.region_list.Count; j++) gc.kid_list[k].region_flags_list.Add(false);

            ku.respawn(k);
        }
        gc.level_end_timer = 2.0f;
        gc.level_complete = false;
    }

    //Spawns and/or initiates spawning of enemies for the given level
    void setup_level_enemies(){
        GameController gc = GameObject.Find("ScriptObject").GetComponent<GameController>(); //Grabs GameController script data
        gc.region_utilities.initialize_region_list(gc.level);

        if(gc.level == 1) gc.spawn_green_ghost = true;
        else if(gc.level == 2){
            gc.spawn_green_ghost = false; 
            gc.region_utilities.fill_regions();
        }
        else if(gc.level == 3){
            gc.region_utilities.fill_regions();
            gc.spawn_green_ghost = true;
        }
    }

    //Enter main game
    public void EnterGame(){
        GameController gc = GameObject.Find("ScriptObject").GetComponent<GameController>(); //Grabs GameController script data
        Scene_Switch_Data.main_menu = false;
        Scene_Switch_Data.main_game = true;
        Scene_Switch_Data.level = 1;
        SceneManager.LoadScene(1);
    }

    //Turns on all of the necessary split screen elements
    public void EnableSplitScreen(){
        GameController gc = GameObject.Find("ScriptObject").GetComponent<GameController>(); //Grabs GameController script data

        //Set cameras
        gc.P1_camera.gameObject.SetActive(true);
        gc.P2_camera.gameObject.SetActive(true);
        gc.main_camera.gameObject.SetActive(false);

        //Set UI
        gc.upgrade_button_on_P2.gameObject.SetActive(true);
        gc.upgrade_text_P2.gameObject.SetActive(true);
        gc.minimap_object_2.SetActive(true);
    }




    /**********************  BUTTON CLICK FUNCTIONS *********************/



     //Starts the main menu
    public void main_menu_begin(){
        RegionUtilities region_utilities = GameObject.Find("ScriptObject").GetComponent<RegionUtilities>(); //Grabs RegionUtility script data
        region_utilities.initialize_region_list(0);
        region_utilities.fill_regions();
    }

    //Starts tutorial for player
    public void onTutorialClick(){
        GameController gc = GameObject.Find("ScriptObject").GetComponent<GameController>(); //Grabs GameController script data
        KidUtilities kid_utilities = GameObject.Find("ScriptObject").GetComponent<KidUtilities>(); //Grabs KidUtility script data

        //Make a cardie for player
        List<int> new_list = Scene_Switch_Data.character_number_list;
        new_list.Add(0);
        Scene_Switch_Data.character_number_list = new_list;
        kid_utilities.make_kid();

        //Turn on in-game mechanics
        gc.upgrade_button_on.gameObject.SetActive(true);
        gc.upgrade_text.gameObject.SetActive(true);
        gc.sprint_image.gameObject.SetActive(true);
        gc.q_text.gameObject.SetActive(true);

        //Turn on Cardie abilities and give player some points to play with
        gc.kid_list[0].sprint_available = true;
        gc.kid_list[0].upgrade_points = 10;
    }

    //Sets the screen back to the main_menu screen
    public void onEndTutorialClick(){
        GameController gc = GameObject.Find("ScriptObject").GetComponent<GameController>(); //Grabs GameController script data

        //Disable in-game UI elements
        gc.upgrade_button_on.gameObject.SetActive(false);
        gc.upgrade_text.gameObject.SetActive(false);
        gc.sprint_image.gameObject.SetActive(false);
        gc.q_text.gameObject.SetActive(false);

        //Destroy kid elements
        gc.kid_list[0].sprint_available = false;
        Destroy(gc.kid_list[0].kid_object);
        gc.kid_list.Clear();
        Scene_Switch_Data.character_number_list = new List<int>();

        //Reset to main menu
        gc.main_menu_panel.SetActive(true);
        gc.main_camera.transform.position = new Vector3(10f, 1f, -2f);
        gc.main_camera.transform.eulerAngles = new Vector3(-15f, 0f, 0f);
    }

    //If the player chooses the Cardie character
    public void onCardieClick(){
        List<int> new_list = Scene_Switch_Data.character_number_list;
        new_list.Add(0);
        Scene_Switch_Data.character_number_list = new_list;
        if(Scene_Switch_Data.game_type == 0 || Scene_Switch_Data.game_type == 1 && Scene_Switch_Data.character_number_list.Count == 2) EnterGame();
    }

    //If the player chooses the Duncan character
    public void onDuncanClick(){
        List<int> new_list = Scene_Switch_Data.character_number_list;
        new_list.Add(1);
        Scene_Switch_Data.character_number_list = new_list;
        if(Scene_Switch_Data.game_type == 0 || Scene_Switch_Data.game_type == 1 && Scene_Switch_Data.character_number_list.Count == 2) EnterGame();
    }

    //Enter main menu
    public void onMainMenuClick(){
        GameController gc = GameObject.Find("ScriptObject").GetComponent<GameController>(); //Grabs GameController script data

        //Clear in-game and pause information
        Time.timeScale = 1;
        gc.pause_panel.SetActive(false);
        gc.pause_timeout = -1f;
        gc.game_paused = false;
        gc.kid_list.Clear();

        //Update scene_switch variables
        Scene_Switch_Data.main_menu = true;
        Scene_Switch_Data.main_game = false;
        Scene_Switch_Data.level = 0;
        Scene_Switch_Data.character_number_list = new List<int>{};
        SceneManager.LoadScene(0);
    }

    //When Play button is clicked
    public void onPlayButtonClick(){
        Scene_Switch_Data.character_number_list = new List<int>{};
    }

    //Sets the boolean to start level one when start button clicked
    public void start_level_one(){
        GameController gc = GameObject.Find("ScriptObject").GetComponent<GameController>(); //Grabs GameController script data
        gc.end_level_1_text = true;
    }
}
