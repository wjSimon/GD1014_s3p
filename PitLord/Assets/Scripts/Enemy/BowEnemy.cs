﻿using UnityEngine;
using System.Collections;

public class BowEnemy : Enemy
{
    public Transform projectileSource;
    public Transform rayTarget;
    public LayerMask projectileLayer;

    bool canAttack;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();

        if (deactivate)
        {
            SwitchNavMesh(false);
            return;
        }

        if(combatRange == 0)
        {
            combatRange = 20;
        }

        ChangeState(0);
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (isDead()) { return; }
        base.Update();

        if (deactivate)
        {
            return;
        }

        BehaviourSwitch();
        Tracking();
    }

    protected override void Tracking()
    {
        base.Tracking();

        if (inAttack())
        {
            if (attackingInv <= (AnimationLibrary.Get().SearchByName(attackName).duration*0.8f))
            //if (attackingInv <= 0.8f)
            {
                LookAtTarget();
            }
        }
    }

    void BehaviourSwitch()
    {
        //ChangeState(4);

        //Idle until Detection of Player
        behavCooldown -= Time.deltaTime;

        //Not working?? - Slides around during attack animation a lot
        if (inAttack() || !alerted)
        {
            return;
        }

        //Retreats rapidly to spawnPoint when character is out of leashing Range
        /*
        if (Vector3.Distance(spawnPoint, transform.position) > leashingRange)
        {
            ChangeState(State.RETREAT); //Retreat
            return;
        }
        /**/

        //Within detection Range, enemy approaches the player
        if (Vector3.Distance(target.position, transform.position) > combatRange && (behavCooldown <= 0) && !inAttack())
        {
                ChangeState(State.APPROACH);
                behavCooldown = Random.Range(0, 4) + 1;
        }

        //Within Combat Range, the enemy decides to attack, backoff or strafe around player
        if (Vector3.Distance(target.position, transform.position) <= combatRange && !inAttack())
        {
            //Attack, BackOff, Strafe for Combat
            BehaviourRandomize = (Random.Range(0,3));

            if(BehaviourRandomize == 0)
            {
                ChangeState(State.ATTACK);
            }
            if (BehaviourRandomize == 1)
            {
                ChangeState(State.STRAFE);
                behavCooldown = Random.Range(0, 4) + 1;
            }
            if (BehaviourRandomize == 2)
            {
                ChangeState(State.BACKOFF);
                behavCooldown = Random.Range(0, 4) + 1;
            }
        }
        /**/

        Behaviour(currentState);
    }

    protected override void CombatUpdate()
    {
        if (!alerted) { return; }

        SetNavPosition(target.position);

        if (inAttack())
        {
            attacking -= Time.deltaTime;
            attackingInv += Time.deltaTime;
            //transform.Rotate(new Vector3(0, 10, 0));
            if (attackingInv >= AnimationLibrary.Get().SearchByName(attackName).colStart)
            {
                SwitchNavMesh(false);
                LaunchProjectile();
            }

            if (attackingInv >= AnimationLibrary.Get().SearchByName(attackName).colStart && attackingInv <= AnimationLibrary.Get().SearchByName(attackName).colEnd)
            {
                weapon.GetComponent<BoxCollider>().enabled = true;
            }
            else
            {
                weapon.GetComponent<BoxCollider>().enabled = false;
            }
        }
    }

    protected override void Blocking()
    {
        //Bow Enemy can't Block!
        blocking = false;
    }

    public override void LaunchProjectile()
    {
        if (!canAttack) { return; }

        ProjectileScript projectile = projectiles[0];
        Transform projectileSource = gameObject.transform.FindChild("RayCastTarget");

        projectile.source = this;
        Quaternion rotation = Quaternion.LookRotation(rayTarget.transform.position - projectileSource.transform.position);
        GameObject.Instantiate(projectile, projectileSource.position, rotation);

        if (inAttack()) { canAttack = false; }
    }

    protected override void Attack()
    {
        base.Attack();

        if (inAttack())
        {
            return;
        }

        RaycastHit rayInfo;
        Vector3 rayCastTarget = (rayTarget.transform.position - projectileSource.transform.position);

        Debug.DrawRay(projectileSource.transform.position, rayCastTarget, Color.red, 20, false);
        if (Physics.Raycast(projectileSource.transform.position, rayCastTarget, out rayInfo, 100, ~projectileLayer)) //transform.position needs to be projectileSource
        {
            Debug.Log(rayInfo.collider.gameObject.name + " " + rayInfo.collider.gameObject.tag + " " + rayInfo.collider.gameObject.layer);
            if (rayInfo.collider.gameObject.tag == "Player")
            {
                StartAttack("E_BowLight01");
                behavCooldown = AnimationLibrary.Get().SearchByName(attackName).duration;
                canAttack = true;
                animator.SetTrigger("Attack");

                Debug.Log("ATTACK");
            }
        }
    }

    /*
    protected override void Attack()
    {
        //base.Attack();
        //Base.Attack portion, needed to modify

        if (inAttack())
        {
            return;
        }

        RaycastHit rayInfo;
        Vector3 rayCastTarget = (rayTarget.transform.position - projectileSource.transform.position).normalized;

        Debug.DrawRay(projectileSource.transform.position, rayCastTarget * 20, Color.red, 20, false);
        if (Physics.Raycast(projectileSource.transform.position, rayCastTarget, out rayInfo, 100)) //transform.position needs to be projectileSource
        {
            //Debug.Log(rayInfo.collider.gameObject.name + " " + rayInfo.collider.gameObject.tag);
            if(rayInfo.collider.gameObject.tag == "Player")
            {
                animator.SetTrigger("Attack");
                attackName = "LightAttack1";
                attacking = AnimationLibrary.Get().SearchByName(attackName).duration;
                attackingInv = 0;

                LaunchProjectile();
            }
        }
    }
    /**/
}
