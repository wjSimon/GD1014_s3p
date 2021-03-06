﻿using UnityEngine;
using System.Collections;

public class PlayerController_Old : Attributes
{
    /*
    //combat
    public GameObject target;

    //camera
    float cameraRotationH;
    float cameraRotationSpeedH;
    float cameraRotationV;
    float cameraRotationSpeedV;

    public float cameraSpeed;

    public float cameraDis;
    float currentCameraDis;

    public float cameraSmoothPos = 10;
    public float cameraSmoothRot = 10;

    public float InverseCameraX = 1;
    public float InverseCameraY = 1;

    float h;
    float v;

    Quaternion targetRotation;
    public Transform cameraTarget;
    public Transform cameraPos;
    public LayerMask cameraCollision;

    public bool lockOn = false;

    //movement
    bool collisionAHead;
    Vector3 movement;
    public float walkSpeed = 4;
    public float runSpeed = 6;
    public Transform inputDir;
    public Transform inputDirTarget;
    Vector3 animDir;

    [HideInInspector]
    public bool inRoll;
    public float rollDuration;
    public float rollCancel;
    float rollStorage;
    public float rollDelay;
    public float rollAccel;

    bool triggerPressed;
    //bool animTriggerBlock;

    Vector3 rollDir;
    float gravity = 20;
    public float speed = 4;
    Vector3 moveDir;


    //reference
    CharacterController cc;
    public GameObject playerModel;
    public GameObject meleeWeapon;
    //animation
    public Animator ani;
    AnimatorStateInfo animStateLayer1;
    AnimatorStateInfo animStateLayer2;
    AnimatorTransitionInfo animTransition1;
    AnimatorTransitionInfo animTransition2;
    AnimatorStateInfo animInfo;
    AnimatorStateInfo animInfoL1;
    AnimatorTransitionInfo animInfoTrans;
    public bool block;

    public Transform handPos;

    bool potionRefill;

    // Use this for initialization
    void Start()
    {
        Init();
    }

    void Init()
    {
        cc = GetComponent<CharacterController>();

        currentHealth = maxHealth;
        currentStamina = maxStamina;

        rollDelay = -rollDelay;
        rollStorage = rollDuration;
        rollDuration = rollDelay;

        RefillPotion();
    }
    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        animStateLayer1 = ani.GetCurrentAnimatorStateInfo(0);
        animStateLayer2 = ani.GetCurrentAnimatorStateInfo(1);
        animTransition1 = ani.GetAnimatorTransitionInfo(0);
        animTransition2 = ani.GetAnimatorTransitionInfo(1);

        inAttack = animStateLayer1.IsTag("Attack") || animTransition1.IsUserName("Attack"); // || necessary?
        inBlock = animStateLayer2.IsTag("Block") || animTransition2.IsUserName("Block");
        ani.SetBool("Block", block);

        //CameraUpdate();
        MovementUpdate();
        CombatUpdate();
        InterActionUpdate();
    }

    void CombatUpdate()
    {
        //Debug.LogWarning(triggerPressed);

        ani.SetBool("Roll", inRoll);

        if (inAttack || inRoll || inBlock)
        {
            if(inRun)
            {
                inRun = false;
                //Find better solution maybe? Can probably bug big time
                staminaTick *= -1;
            }
        }

        if (!inRoll || inRoll && rollDuration >= (Mathf.Clamp01(rollCancel) * rollStorage))
        {
            if (Input.GetButtonDown("LightAttack"))
            {
                if (!inAttack)
                {
                    if (StaminaCost(gameObject, "LightAttack"))
                    {
                        ani.SetTrigger("Attack");
                        block = false;

                        if (inRoll)
                        {
                            inRoll = false;
                            rollDuration = rollStorage;
                        }
                    }
                }
            }

            if (Input.GetAxis("HeavyAttack") > 0 && triggerPressed == false)
            {
                triggerPressed = true;
                if (!inAttack)
                {
                    if (StaminaCost(gameObject, "HeavyAttack"))
                    {
                        ani.SetTrigger("HeavyAttack");

                        if (inRoll)
                        {
                            inRoll = false;
                            rollDuration = rollStorage;
                        }
                    }
                }
            }

            if (Input.GetButtonDown("Block"))
            {
                if (!inAttack && !inRoll && currentStamina > 0)
                {
                    ani.SetBool("Block", true);
                    block = true;
                }
            }
            if (Input.GetButtonUp("Block"))
            {
                ani.SetBool("Block", false);
                block = false;
            }

            if (inAttack)
            {
                if (animStateLayer1.IsName("LightAttack1") == true)
                {
                    float start = AnimationLibrary.Get().SearchByName("LightAttack1").start;
                    float end = AnimationLibrary.Get().SearchByName("LightAttack1").end;

                    if (animStateLayer1.normalizedTime >= start && animStateLayer1.normalizedTime <= end)
                    {
                        meleeWeapon.GetComponent<BoxCollider>().enabled = true;
                    }
                    else
                    {
                        meleeWeapon.GetComponent<BoxCollider>().enabled = false;
                    }
                }
                if (animStateLayer1.IsName("HeavyAttack1") == true)
                {
                    float start = AnimationLibrary.Get().SearchByName("HeavyAttack1").start;
                    float end = AnimationLibrary.Get().SearchByName("HeavyAttack1").end;

                    if (animStateLayer1.normalizedTime >= start && animStateLayer1.normalizedTime <= end)
                    {
                        meleeWeapon.GetComponent<BoxCollider>().enabled = true;
                    }
                    else
                    {
                        meleeWeapon.GetComponent<BoxCollider>().enabled = false;
                    }
                }
            }

            if (Input.GetButtonDown("Roll"))
            {
                if ((rollDuration <= rollDelay) && (!inAttack || inAttack && animStateLayer1.normalizedTime <= AnimationLibrary.Get().SearchByName("LightAttack1").cancel))
                {
                    if (StaminaCost(gameObject, "Roll"))
                    {
                        inRoll = true;
                        rollDuration = rollStorage;
                        h = Input.GetAxisRaw("Horizontal");
                        v = Input.GetAxisRaw("Vertical");
                    }
                }
                //.Move((Mathf.Sign(animator.GetFloat("X")) * transform.right) * Time.deltaTime * agent.speed / 3);
            }
        }

        //Rolling animation stuff - Decoupled
        if (inRoll)
        {
            Debug.LogWarning((transform.TransformDirection(new Vector3(h, 0, v)) * speed));
            if (lockOn)
            {
                GetComponent<CharacterController>().Move((transform.TransformDirection(new Vector3(h, 0, v)) * speed) * Time.deltaTime * speed * rollAccel);
            }
            else
            {
                GetComponent<CharacterController>().Move(moveDir * Time.deltaTime * speed * rollAccel);
            }

            if (rollDuration <= 0)
            {
                inRoll = false;
            }
        }

        if (Input.GetAxis("HeavyAttack") <= 0 && !inAttack)
        {
            triggerPressed = false;
        }

        rollDuration -= Time.deltaTime;
    }

    
    void CameraUpdate()
    {
        Ray cameraRay = new Ray(cameraTarget.position, cameraPos.position - cameraTarget.position);
        RaycastHit cameraRayInfo;

        if (Physics.Raycast(cameraRay, out cameraRayInfo, cameraDis, cameraCollision))
        {
            currentCameraDis = Vector3.Distance(cameraTarget.position, cameraRayInfo.point) - Vector3.Distance(cameraTarget.position, cameraTarget.position + (cameraPos.position - cameraTarget.position).normalized);
            currentCameraDis = Mathf.Clamp(currentCameraDis, 0.64f, cameraDis);
        }

        else
        {
            currentCameraDis = cameraDis;
        }

        //Debug.DrawRay(cameraTarget.position, cameraPos.position-cameraTarget.position);
        float ch = Input.GetAxis("CameraH");
        float cV = Input.GetAxis("CameraV");

        //float ch = Input.GetAxis("Mouse X");
        //float cV = Input.GetAxis("Mouse Y");

        if (lockOn == true && target != null)
        {
            //cameraRotationV += Time.deltaTime * cV * cameraSpeed ;  
            //cameraRotationV = Mathf.Clamp(cameraRotationV, -20, 0);    
            cameraRotationV = cameraTarget.eulerAngles.x;
            cameraRotationH = cameraTarget.eulerAngles.y;
            //cameraTarget.eulerAngles = new Vector3 (0, playerModel.transform.eulerAngles.y, 0);  
            cameraTarget.eulerAngles = new Vector3(0, Quaternion.LookRotation((target.transform.position + Vector3.up) - Camera.main.transform.position).eulerAngles.y, 0);
            cameraPos.localPosition = new Vector3(0, 1, -currentCameraDis * 1.4f);
            cameraPos.LookAt(cameraTarget.position);
            Camera.main.transform.position = cameraPos.position;
            //Camera.main.transform.rotation = Quaternion.RotateTowards(Camera.main.transform.rotation, Quaternion.LookRotation((target.transform.position + Vector3.up) - Camera.main.transform.position), 150 * Time.deltaTime);
            //Camera.main.transform.LookAt(target.position+Vector3.up);
            Camera.main.transform.rotation = Quaternion.LookRotation((target.transform.position + Vector3.up) - Camera.main.transform.position);
        }
        else
        {
            //cameraRotationH += Time.deltaTime*ch*cameraSpeed*InverseCameraX ;                 
            //cameraRotationV += Time.deltaTime*cV*cameraSpeed*InverseCameraY ; 
            cameraRotationSpeedH = Mathf.Lerp(cameraRotationSpeedH, Time.deltaTime * ch * cameraSpeed * InverseCameraX, Time.deltaTime * cameraSpeed / 2);
            cameraRotationSpeedV = Mathf.Lerp(cameraRotationSpeedV, Time.deltaTime * cV * cameraSpeed * InverseCameraY, Time.deltaTime * cameraSpeed / 2);

            cameraRotationH += cameraRotationSpeedH;
            cameraRotationV += cameraRotationSpeedV;

            cameraRotationV = Mathf.Clamp(cameraRotationV, -40, 25);
            cameraTarget.eulerAngles = new Vector3(cameraRotationV, cameraRotationH, 0);
            cameraPos.localPosition = new Vector3(0, currentCameraDis / 2, -currentCameraDis);
            cameraPos.LookAt(cameraTarget.position);
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, cameraPos.position, cameraSmoothPos * Time.deltaTime);
            Camera.main.transform.forward = Vector3.Lerp(Camera.main.transform.forward, cameraPos.forward, cameraSmoothRot * Time.deltaTime);
        }
    }

    public void CameraRotateToTarget()
    {
        cameraTarget.eulerAngles = new Vector3(0, Quaternion.LookRotation((target.transform.position + Vector3.up) - Camera.main.transform.position).eulerAngles.y, 0);
        cameraPos.localPosition = new Vector3(0, 1, -currentCameraDis * 1.4f);
        cameraPos.LookAt(cameraTarget.position);
        Camera.main.transform.position = cameraPos.position;
        Camera.main.transform.rotation = Quaternion.LookRotation((target.transform.position + Vector3.up) - Camera.main.transform.position);

    }
    void MovementUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        moveDir = new Vector3(h, 0, v);
        moveDir = transform.TransformDirection(moveDir);
        moveDir *= speed;

        if (inAttack || inRoll)
            return;

        Vector3 move=cameraTarget.forward * v + cameraTarget.right * h;
        move.y = 0;

        if (h != 0 || v != 0)
            inputDirTarget.localPosition = new Vector3(h, 0, v);
        inputDir.eulerAngles = new Vector3(0, cameraTarget.eulerAngles.y, 0);

        if (Input.GetAxis("Sprint") > 0)
        {
            if (!inRun && currentStamina > 0)
            {
                speed = runSpeed;
                inRun = true;
                staminaTick *= -1;
            }
        }

        else if (Input.GetAxis("Sprint") <= 0)
        {
            if (inRun)
            {
                speed = walkSpeed;
                inRun = false;
                staminaTick *= -1;
            }
        }

        if (cc.isGrounded)
        {
            if (lockOn == true)
            {
                transform.LookAt(target.transform);
                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

                //transform.TransformDirection(new Vector3(h, 0, v))*speed
                ani.SetFloat("X", Input.GetAxis("Horizontal"));
                ani.SetFloat("Y", Input.GetAxis("Vertical"));
            }

            else
            {
                moveDir = transform.forward * Vector3.Distance(Vector3.zero, new Vector3(h, 0, v));
                moveDir *= speed;
                if (!inRun)
                {
                    ani.SetFloat("X", 0);
                    ani.SetFloat("Y", Vector3.Distance(Vector3.zero, new Vector3(h, 0, v)) * 2);
                }
                if (inRun)
                {
                    ani.SetFloat("X", 0);
                    ani.SetFloat("Y", 2);
                }


                transform.forward = new Vector3(-inputDir.position.x + inputDirTarget.position.x, 0, -inputDir.position.z + inputDirTarget.position.z);
            }

        }

        moveDir.y -= gravity * Time.deltaTime;
        cc.Move(moveDir * Time.deltaTime);
    }

    void InterActionUpdate()
    {
        if (Input.GetButtonDown("Interact"))
        {
            if (potionRefill)
            {
                RefillPotion();
            }

            //Item, Doors,
        }

        if (Input.GetButtonDown("Heal"))
        {
            if (heals > 0 && !(inAttack || inRoll))
            {
                ani.SetTrigger("Heal");
            }
        }
    }

    void OnTriggerEnter( Collider other )
    {
        if (other.name == "PotionRefill")
        {
            //RefillPotion();
            potionRefill = true;
            //Show Screen Prompt for Potion Refilling, enable key (bool?)
        }
    }

    void OnTriggerExit( Collider other )
    {
        if (other.name == "PotionRefill")
        {
            //Hide Prompt, disable key
            potionRefill = false;
        }
    }

    public void RefillPotion()
    {
        //stuff here
        heals = maxHeals;
        //Debug.LogWarning("Potions refilled");
    }

    public void UseHeal()
    {
        heals -= 1;
        currentHealth += healAmount;
    }
     */
}
