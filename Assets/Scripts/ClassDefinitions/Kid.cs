using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kid
{  
    //Kid description attributes
    public GameObject kid_object; //GameObject representing the kid
    public int character_number; //0 = Cardie; 1 = Duncan
    public int player_number; //Who owns this kid
    public Animator animator; //Determines what animation is currently playing
    public Region region; //Stores the Region the kid is currently in
    public int region_index; //Stores the indices of the kid_region
    public float speed = 3.0f; //Determines kid velocity
    public bool falling = false; //Determines if kid is falling
    public List<bool> region_flags_list = new List<bool>(); //Stores all of the Regions the player has reached
    
    //Upgrade attributes
    public int upgrade_points = 0; //Amount of points player has to upgrade
    public int speed_upgrades = 0; //Amount of points invested in speed
    public int power_upgrades = 0; //Amount of points invested in ability power

    //Ability Attributes
    public float sprint_cooldown = -1f; //Amount of time before sprint can be used again
    public float sprint_timer = -1f; //Amount of time sprint remains active
    public bool sprint_active = false; //Determmines if sprint is active
    public float sprint_bonus; //Amount of speed sprint increases kid_speed by
    public float ability_power = 0; //multiplier that determines ability strength
    public bool sprint_available = false; //Determines if sprint cooldown is complete

    public Kid(GameObject kid_object, int character_number, int player_number){
        this.character_number = character_number;
        this.kid_object = kid_object;
        this.player_number = player_number;
    }
}