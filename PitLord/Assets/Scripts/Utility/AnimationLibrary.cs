﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// Stores custom AnimationWrappers and allows them to be searched by name, allows user to have all Animation Start/End Times for normalized time in this library,
/// so tweaks are more easily distributed if scripts utilize the Library references instead of hard values.
/// 
/// Adjust Animation normalized times via Library to globally implement changes to normalized times.
/// </summary>
public class AnimationLibrary
{
    private static AnimationLibrary instance;
    public Dictionary<string, AnimationWrapper> animations;

    private AnimationLibrary()
    {
        Init();
    }

    //Name, colStart, colEnd, cancel, duration
    private void Init()
    {
        animations = new Dictionary<string, AnimationWrapper>();

        //PlaceHolder
        AddAnimation(new AnimationWrapper("default", 0.0f, 0.0f, 0.0f, 0.0f));
        AddAnimation(new AnimationWrapper("LightAttack1", 0.5f, 2.0f, 1.5f, 2.9f).RomoLength(0.465f));
        AddAnimation(new AnimationWrapper("LightAttack2", 0.5f, 2.0f, 1.5f, 2.4f).RomoLength(0.247f));
        AddAnimation(new AnimationWrapper("HeavyAttack1", 1.2f, 2.1f, 2.6f, 3.0f).RomoLength(0.499f).Knockback(20f, 10f));
        AddAnimation(new AnimationWrapper("LightAttack1_enemyplaceholder", 0.2f, 0.6f, 2.5f, 2.5f));
    }


    public static AnimationLibrary Get()
    {
        if (instance == null)
        {
            instance = new AnimationLibrary();
        }

        return instance;
    }

    private void AddAnimation( AnimationWrapper wrapper )
    {
        animations[wrapper.name] = wrapper;
    }

    public AnimationWrapper SearchByName( string name )
    {
        return animations[name];
    }
}
