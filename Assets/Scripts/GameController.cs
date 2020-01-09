using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    //GameObject References:
    /********** PREFABS/MATERIALS **********/
    public GameObject Toon_Ghost_Orange_Prefab;
    public GameObject Toon_Ghost_Green_Prefab;
    public GameObject kid_prefab_cardie;
    public GameObject kid_prefab_duncan;
    public Material slow_symbol_material;

    /********** UI **********/
    public Camera main_camera;
    public GameObject minimap_object; //Minimap in lower right
    public GameObject minimap_object_2; //Second minimap for split screen

    //PLAYER 1 UI
    public Camera P1_camera; //Controls player one camera when in split screen mode
    public Text upgrade_text; //Text for upgrade button in shop menu
    public Text speed_text;
    public Text power_text;
    public Text q_text; //Text for q ability
    public Button power_button; //Button for upgrading ability power in shop menu
    public Button ability_button;
    public Text ability_text;
    public Image sprint_image;
    public Button upgrade_button_on; //Menu button in top left
    public Button upgrade_button_off;
    //PLAYER 2 UI
    public Camera P2_camera;
    public Text upgrade_text_P2;
    public Text speed_text_P2;
    public Text power_text_P2;
    public Text q_text_P2;
    public Button power_button_P2;
    public Button ability_button_P2;
    public Text ability_text_P2;
    public Image sprint_image_P2;
    public Button upgrade_button_on_P2;
    public Button upgrade_button_off_P2;


    /********** AUDIO **********/
    public GameObject plot_text_music_object;
    public GameObject level_1_song_object;
    public GameObject pause_panel;
    public GameObject main_menu_panel;
    public GameObject vl_start_1_object; //Voice line at start of game 
    public Text vl_start_1_text; //Text that displays when voiceline starts at beginning of first level
    public GameObject vl_end_1_object; //Voice line at end of game 
    public Text vl_end_1_text; //Text that displays when voiceline starts at end of first level


    //Local variables
    [HideInInspector] public AudioSource level_1_song; //Song played during level 1
    [HideInInspector] public AudioSource plot_song; //Song played during plot_text scenes
    [HideInInspector] public List<Ghost> ghost_list = new List<Ghost>(); //Stores all of the ghost game objects
    [HideInInspector] public List<Region> region_list = new List<Region>(); //Stores all of the Regions
    [HideInInspector] public float ghost_speed; //Determines ghost velocity
    [HideInInspector] public bool game_paused; //Determines if game is paused
    [HideInInspector] public float pause_timeout; //Small amount of time to keep (un)pausing from accidently occuring more than once
    [HideInInspector] public bool game_begun; //Determines when the player actually start the dodging portion of the game
    [HideInInspector] public bool end_level_1_text; //Determines if game the player has finished reading the level_1_text
    [HideInInspector] public bool main_menu_active; //Determines if the player is on the main_menu screen
    [HideInInspector] public AudioSource main_menu_song; //Song played during the main menu
    [HideInInspector] public float vl_1_timer; //Counts down when to increase music volume of game
    [HideInInspector] public bool level_complete; //Determines if player reached the end of the level
    [HideInInspector] public int level; //The current level
    [HideInInspector] public float level_end_timer; //Counts down to when the level ends
    [HideInInspector] public GameUtilities game_utilities; //Script containing basic functions like pausing and text updating
    [HideInInspector] public KidUtilities kid_utilities; //Script containing functions to modify kid attributes
    [HideInInspector] public RegionUtilities region_utilities; //Script containing functions to modify region attributes
    [HideInInspector] public GhostUtilities ghost_utilities; //Script containing functions to modify ghost attributes
    [HideInInspector] public AbilityUtilities ability_utilities; //Script containing functions to modify ability attributes
    [HideInInspector] public bool spawn_green_ghost; //Determines if green ghost should be spawning
    [HideInInspector] public List<(string, GameObject)> slowed_ghost_list = new List<(string, GameObject)>(); //ITEM1: List of ghost (marked by ghost names) that are slowed by Duncan's aura; ITEM2: the corresponding slow symbol
    [HideInInspector] public List<Kid> kid_list = new List<Kid>(); //Stores a Kid for each player
    [HideInInspector] public List<Region> kid_region_list = new List<Region>(); //Stores the region of each kid



    // Start is called before the first frame update
    void Start()
    {
        //Import utility functions
        game_utilities = GameObject.Find("ScriptObject").GetComponent<GameUtilities>();
        kid_utilities = GameObject.Find("ScriptObject").GetComponent<KidUtilities>();
        region_utilities = GameObject.Find("ScriptObject").GetComponent<RegionUtilities>();
        ghost_utilities = GameObject.Find("ScriptObject").GetComponent<GhostUtilities>();
        ability_utilities = GameObject.Find("ScriptObject").GetComponent<AbilityUtilities>();

        //Initialize variables
        main_menu_active = Scene_Switch_Data.main_menu;
        level = 1;
        ghost_speed = 3.0f;
        game_paused = false;
        game_begun = false;
        if(vl_start_1_object != null) vl_start_1_object.GetComponent<AudioSource>().Stop();
        if(vl_end_1_object != null) vl_end_1_object.GetComponent<AudioSource>().Stop();
        vl_1_timer = 3.0f;
        level_complete = false;
        level_end_timer = 2.0f;
        spawn_green_ghost = false;

        if(Scene_Switch_Data.main_menu == false){ //If level 1 start
            level_1_song = level_1_song_object.GetComponent<AudioSource>();
            level_1_song.loop = true;
            level_1_song.Stop();
            end_level_1_text = false;
            plot_song = plot_text_music_object.GetComponent<AudioSource>();
            plot_song.Play();
            plot_song.loop = true;
        }
        else{ //If tutorial start
            main_menu_song = GetComponent<AudioSource>();
            main_menu_song.loop = true;
            main_menu_song.Play();
            game_utilities.main_menu_begin();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(main_menu_active == false) game_utilities.pause_game(); //Pauses Game 
        game_utilities.updateUIText(); //Updates UI text
    }

    //Updates with respect to a fixed deltaTime of 0.02 -> used for time-based actions && positioning changes
    void FixedUpdate()
    {
        //Check if every kid is in the last region
        bool not_all_in_final_zone = false;
        for(int k = 0; k < kid_list.Count; k++){
            if(kid_list[k].region_index != region_list.Count - 1) not_all_in_final_zone = true;
        }

        //If the player reached the end
        if(kid_list.Count > 0 && not_all_in_final_zone == false && Scene_Switch_Data.level > 0){
            if(level_end_timer > 0){
                //Place each kid in middle of region
                for(int k = 0; k < kid_list.Count; k++) {
                    kid_list[k].kid_object.transform.position = new Vector3(kid_list[k].region.boundary_coordinates[1] - (kid_list[k].region.boundary_coordinates[1] - 
                        kid_list[k].region.boundary_coordinates[0]) / 2f,0, 0.5f * k + kid_list[k].region.boundary_coordinates[3] - (kid_list[k].region.boundary_coordinates[3] - kid_list[k].region.boundary_coordinates[2]) / 2f);

                    main_camera.transform.position = new Vector3(kid_list[k].kid_object.transform.position.x, 7f, kid_list[k].kid_object.transform.position.z - 9f);
                }

                level_complete = true;

                //Play ending voiceline
                if(vl_end_1_object.GetComponent<AudioSource>().isPlaying == false) vl_end_1_object.GetComponent<AudioSource>().Play();
                vl_end_1_text.gameObject.SetActive(true);
            }
            else{
                //Stop end voiceline audio and move to next level
                vl_end_1_object.GetComponent<AudioSource>().Stop();
                vl_end_1_text.gameObject.SetActive(false);
                game_utilities.end_level();
            }
        }
        //When the player is actively in the game
        else if(game_begun == true && end_level_1_text == true){

            if(level == 1){ //Initialize level 1
                if(level_1_song.isPlaying == false && vl_1_timer <= 0){
                    level_1_song.Play();
                    vl_start_1_text.gameObject.SetActive(false);
                }
            }
            if(game_paused == false){ //Perform standard functions
                game_utilities.updateUIText();
            } 
        }
        
        if(game_begun == true && end_level_1_text == true && level == 1 && vl_1_timer > 0f) vl_1_timer -= Time.fixedDeltaTime; //Take time to finish voice line before starting music
        if(game_begun == false && end_level_1_text == true) game_utilities.level_begin(); //Take the time to pan the camera down
        if(kid_list.Count > 0 && kid_list[0].region_index == region_list.Count - 1 && level_end_timer > 0) level_end_timer -= Time.fixedDeltaTime; //If player reached end, run the end_level_timer
        if(game_paused == false){
            ghost_utilities.move_ghost();

            //Perform player control functions
            if(kid_list.Count > 0){
                kid_utilities.move_kid();
                kid_utilities.check_collisions();
                ability_utilities.check_abilities();
            }
        }
    }
}
