using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;
    
    [Header("时间控制")]
    public float global_time_scale =1f;
    public float slow_down_durian = 3f;
    private float timer_slow_down;
    private bool slow_down_flag = false;

    private Dictionary<GameObject,float> obj_time_scale_dic= new Dictionary<GameObject, float>();
    private void Awake() //确保全局只有一个instance
    {
        if(Instance== null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (slow_down_flag)
        {
            timer_slow_down += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(timer_slow_down / slow_down_durian);

            //通过插值实现时间流逝
            global_time_scale =  Mathf.Lerp(1f, 0f, progress);
            if (progress > 1f)
            {
                slow_down_flag = false;
            }
        }
    }
    public void StartSlowDown()
    {
        slow_down_flag = true;
        timer_slow_down = 0f;
    }
    public void StopSlowDown()
    {
        slow_down_flag = false;
        global_time_scale = 1f;
    }
    public float GetObjTimeScale(GameObject game_obj)
    {
        if (game_obj.CompareTag("Player")) return 1f;

        return obj_time_scale_dic.TryGetValue(game_obj, out float scale) ? scale*global_time_scale : global_time_scale; //获取物体的时间 
    }
    public void RegisterObj(GameObject obj, float initial_scale = 1f)
    {
        if (!obj_time_scale_dic.ContainsKey(obj))
        {
            obj_time_scale_dic.Add(obj, initial_scale);
        }
    }
}
