using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Scene_Switch_Data
{
    public static bool main_menu = true; //Determines if in main_menu / tutorial
    public static bool main_game = false; //Determines if in main game
    public static int level = 0; //Current player level
    public static List<int> character_number_list = new List<int>{}; //0 = Cardie; 1 = Duncan
    public static int game_type = 0; //0 = single player; 1 = split screen; 2 = online



    /*************** DEVELOPER VARIABLES ***************/


    public static bool DEVELOPER_spawn_last_region = false; //Spawns kid at last region to start
}
