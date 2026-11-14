using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Player : MonoBehaviour
{
    [System.Serializable]
    public class PlayerStats
    {
        public float Health = 100f;
    }

    public PlayerStats playerStats = new PlayerStats(); //creating an instance of the Player's Stats
    private Animator animator;
    private GameObject player;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Animator healthSliderAnimator;
    //public static int fallBoundary = -20;



    void Awake()
    {
        //normalHeight = playerHeight.height;
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        animator = player.GetComponent<Animator>();
        GameMaster.gm.respawnPoint = transform.position; //The initial position of the player will be his first respawn point
    }


    void Update()
    {
        //Damage Player excessively if he falls under the minimum height (y axis)
        //if (transform.position.y <= fallBoundary)
        //{
        //    DamagePlayer(999999);
        //}

        //The player needs to hold down the LCtrl button to stay crouched
        //if (Input.GetKeyDown(KeyCode.LeftControl))
        //{
        //    animator.Play("CrouchingIdle");
        //    playerHeight.height = crouchHeight;
        //}
        //if (Input.GetKeyUp(KeyCode.LeftControl))
        //{
        //    playerHeight.height = normalHeight;
        //}

        if (playerStats.Health < 100)
        {
            healthSliderAnimator.SetBool("ShowHealthBar", true); //show health slider if below max
            healthSlider.value = playerStats.Health;
            playerStats.Health += 0.03f; //automatic health regeneration if below max
        }
        else
        {
            healthSliderAnimator.SetBool("ShowHealthBar", false);
        }
            
    }


    //The Damage Player function
    public void DamagePlayer(float damage)
    {
        playerStats.Health -= damage;

        if (playerStats.Health <= 0)
        {
            //Kill Player
            GameMaster.KillPlayer(this, 2); //Play the fatal blow animation if health drops below zero (for now at least, may change it later)
        }
    }


    //This method will run whenever the Player collides with another object that has a trigger
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FallDetector"))
        {
            GameMaster.KillPlayer(this, 1);
        }
        else if (other.CompareTag("Checkpoint"))
        {
            Debug.Log("Hit a checkpoint! (at pos: " + transform.position +")");
            GameMaster.gm.respawnPoint = transform.position;
        }
        else if (other.CompareTag("PlayerKiller") && !GameMaster.gm.playerIsDead)
        {
           //DamagePlayer(999999);
           GameMaster.KillPlayer(this, 2);
        }
    }


}
