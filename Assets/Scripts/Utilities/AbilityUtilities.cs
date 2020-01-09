using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityUtilities : MonoBehaviour
{
    //Increases player speed when clicked
    public void onSpeedClick(int kid_index){
        GameController gc = GameObject.Find("ScriptObject").GetComponent<GameController>(); //Grabs GameController script data

        if(gc.kid_list[kid_index].upgrade_points > 0){
            gc.kid_list[kid_index].upgrade_points -= 1;
            gc.kid_list[kid_index].speed += 0.1f;
            gc.kid_list[kid_index].speed_upgrades += 1;
        }
    }

    //Increases ability power when clicked
    public void onPowerClick(int kid_index){
        GameController gc = GameObject.Find("ScriptObject").GetComponent<GameController>(); //Grabs GameController script data

        if(gc.kid_list[kid_index].upgrade_points > 0){
            gc.kid_list[kid_index].upgrade_points -= 1;
            gc.kid_list[kid_index].ability_power += 0.075f;
            gc.kid_list[kid_index].power_upgrades += 1;
        }
    }

    /*
    //Purchases the sprint ability when clicked
    public void onAbilityClick(){
        GameController gc = GameObject.Find("ScriptObject").GetComponent<GameController>(); //Grabs GameController script data

        if(gc.kid_list[0].upgrade_points > 4){
            gc.kid_list[0].upgrade_points -= 5;

            gc.sprint_image.gameObject.SetActive(true);
            gc.q_text.gameObject.SetActive(true);
            gc.power_text.gameObject.SetActive(true);
            gc.power_button.gameObject.SetActive(true);

            gc.ability_text.gameObject.SetActive(false);
            gc.ability_button.gameObject.SetActive(false);

            gc.sprint_available = true;
        }
    }
    */

    //Updates ability timers and effects
    public void check_abilities(){
        GameController gc = GameObject.Find("ScriptObject").GetComponent<GameController>(); //Grabs GameController script data

        for(int k = 0; k < gc.kid_list.Count; k++){
            //Check Sprint
            if(gc.kid_list[k].sprint_cooldown > 0f) { //Continues cooldown timer and displays to player
                gc.kid_list[k].sprint_cooldown -= Time.deltaTime;

                if(k == 1){
                    gc.q_text_P2.text = gc.kid_list[k].sprint_cooldown.ToString("0.00");
                    gc.q_text_P2.color = Color.red;
                }
                else{
                    gc.q_text.text = gc.kid_list[k].sprint_cooldown.ToString("0.00");
                    gc.q_text.color = Color.red;
                }

                //Ends cooldown timer and allows for use of sprint
                if(gc.kid_list[k].sprint_cooldown <= 0f && k == 1){
                    gc.q_text_P2.text = "Q";
                    gc.q_text_P2.color = Color.green;
                }
                else if(gc.kid_list[k].sprint_cooldown <= 0f && k != 1){ //Ends cooldown timer and allows for use of sprint
                    gc.q_text.text = "Q";
                    gc.q_text.color = Color.green;
                }
            }
            if(gc.kid_list[k].sprint_active == true){ //Continues sprint timer and displays to player
                if(k == 1){
                    gc.q_text_P2.text = gc.kid_list[k].sprint_timer.ToString("0.00");
                    gc.q_text_P2.color = Color.green;
                }
                else{
                    gc.q_text.text = gc.kid_list[k].sprint_timer.ToString("0.00");
                    gc.q_text.color = Color.green;
                }

                gc.kid_list[k].sprint_timer -= Time.deltaTime;

                if(gc.kid_list[k].sprint_timer <= 0f){ //Begins cooldown timer and displays to player
                    gc.kid_list[k].sprint_cooldown = 8f;

                    if(k == 1){
                        gc.q_text_P2.text = gc.kid_list[k].sprint_cooldown.ToString("0.00");
                        gc.q_text_P2.color = Color.red;
                    }
                    else{
                        gc.q_text.text = gc.kid_list[k].sprint_cooldown.ToString("0.00");
                        gc.q_text.color = Color.red;
                    }

                    gc.kid_list[k].sprint_active = false;
                    gc.kid_list[k].speed -= gc.kid_list[k].sprint_bonus;
                }
            }
        }
    }
}
