using System;
using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class GameMaster : MonoBehaviour
{
    //!!!WARNING!!!This class will be destroyed if we change scenes (ex. going back to the main menu) and will be re-instanced when we load the scene it exists in again
    public static GameMaster gm;  //For a singleton GameMaster class (only one per scene)

    public bool introHasPlayed = false;  //doesn't work if the whole scene reloads!
    private Animator playerAnimator;
    //public GameObject loadingScreen;
    //public Slider loadingProgressSlider;
    //public Text progressText;
    //public Transform playerPrefab;
    public GameObject playerObject;
    public Transform fallDetectorPrefab;
    //public Transform spawnPoint;
    public int spawnDelay = 5;
    public Vector3 respawnPoint;
    public Image fadeToBlackAfterDeath;
    private Animator fadeToBlackAfterDeathAnimator;
    [SerializeField] private AudioClip playerDeathSound;
    [SerializeField] private AudioClip playerWakeUpGasp;
    public bool ambientMusicTriggered = false;
    public bool playerIsDead = false;
    //private AudioListener mainCameraListener;
    //[SerializeField] private Transform mainCamera;
    //[SerializeField] private Transform playerFollowCamera;
    [SerializeField] private GameObject pauseCanvas;
    [SerializeField] private Transform playerCameraRoot;
    private PlayerPickupDrop playerPickupDrop;
    private Player player;
    private ThirdPersonController playerThirdPersonController;
    private CharacterController playerCharacterController;
    private PlayerInput playerInput;
    private bool tmpSprintSpeed;
    



    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)] //resolve gc handle fix
    static void Init()
    {
        gm = null;
    }


    void Awake()
    {
        if (gm == null)
        {
            gm = this;
            //DontDestroyOnLoad(gameObject); //this instance will preserve even if we change scenes (WE DON'T ALWAYS WANT THIS)
        }
        else
        {
            Destroy(gameObject); //if an instance already exists, destroy it
        }

        
        //playerObject = GameObject.FindGameObjectWithTag("Player");
        playerAnimator = playerObject.GetComponent<Animator>();
        //playerCameraRoot = GameObject.FindGameObjectWithTag("CinemachineTarget").transform;

        //pauseCanvas = GameObject.FindGameObjectWithTag("PauseMenu");

        playerPickupDrop = playerObject.GetComponent<PlayerPickupDrop>();

        player = playerObject.GetComponent<Player>();
        playerThirdPersonController = playerObject.GetComponent<ThirdPersonController>();
        playerCharacterController = playerObject.GetComponent<CharacterController>();
        playerInput = playerObject.GetComponent<PlayerInput>();
        
    }


    void Start()
    {
        //mainCameraListener = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioListener>();
        
    }


    void Update()
    {

        //If the player is dead, then whatever sound can be heard through the camera's audioListener
        /*if (playerIsDead)
        {
            mainCameraListener.enabled = true;
        }
        else
        {
            mainCameraListener.enabled = false;
        }*/

        
    }


    public IEnumerator RespawnPlayer()
    {
        //Fade to black after player's death and before his respawn
        fadeToBlackAfterDeathAnimator = fadeToBlackAfterDeath.GetComponent<Animator>();
        fadeToBlackAfterDeathAnimator.SetTrigger("FadeToBlack");
        yield return new WaitForSeconds(spawnDelay);

        
        playerCharacterController.enabled = false;  //because it does not allow player repositioning..
        
        //StartCoroutine(ReloadCurrentSceneAsynchronously()); //Reload the level (force the player to replay it all after death)
        //Instantiate(playerPrefab, respawnPoint.position, respawnPoint.rotation);

        playerObject.transform.position = respawnPoint; //don't destroy the player; instead, just place him on the respawn point
        Debug.Log("Respawned player at pos: " + playerObject.transform.position);
        playerObject.transform.rotation = Quaternion.Euler(0, 90, 0);
        playerCameraRoot.rotation = Quaternion.Euler(0, 0, 0); //reset the camera looking position
        player.playerStats.Health = 100f;
        playerCharacterController.enabled = true;


        fadeToBlackAfterDeathAnimator.SetTrigger("FadeOutFromBlack");
        playerAnimator.SetTrigger("WakeUp");
        SoundManager.instance.PlaySFXSound(playerWakeUpGasp);
        yield return new WaitForSeconds(5);
        SoundManager.instance.ResumeMusic();  //continue whatever music was playing in the background

        
        playerInput.enabled = true;
        //SOLVED! --> //pauseCanvas.GetComponent<PauseMenu>().enabled = true; //allow the player to pause the game AFTER the character has respawned and is in idle animation

        playerIsDead = false;
    }


    /*IEnumerator ReloadCurrentSceneAsynchronously()
    {
        int scene = SceneManager.GetActiveScene().buildIndex;  //get current scene index
        AsyncOperation operation = SceneManager.LoadSceneAsync(scene);  //store the status of the loading operation

        //loadingScreen.SetActive(true);

        while (!operation.isDone)  //wait till its done
        {
            //float progress = Mathf.Clamp01(operation.progress/0.9f);

            //loadingProgressSlider.value = progress;
            //progressText.text = progress*100f + "%";

            yield return null;
        }

        //loadingScreen.SetActive(false);
    }*/


    //This is static to make sure that we can kill the player from everywhere
    public static void KillPlayer(Player player, int deathAnimation)
    {
        //player.playerStats.Health = 0f;

        gm.playerObject.GetComponent<ThirdPersonController>().Grounded = true; //so that the player doesn't take fall dmg when dying of fall

        if (deathAnimation == 1) //player died of fall
        {
            SoundManager.instance.PauseMusic();  //stop the background music
            SoundManager.instance.PlaySFXSound(gm.playerDeathSound);
        }
        else if (deathAnimation == 2) //player died from a fatal blow
        {
            SoundManager.instance.PauseMusic();  //stop the background music
            SoundManager.instance.PlaySFXSound(gm.playerDeathSound);
            gm.playerAnimator.SetTrigger("Dying");
        }

        gm.playerInput.enabled = false;
        gm.playerThirdPersonController._input.sprint = false; //stop player's sprint (if he was)
        //SOLVED! --> gm.pauseCanvas.GetComponent<PauseMenu>().enabled = false; //don't allow the player to pause the game while character is dying (cause it causes bugs)

        //Destroy(player.gameObject);  //Kill the player
        gm.playerIsDead = true;

        gm.StartCoroutine(gm.RespawnPlayer());  //Respawn the player
       
    }

}
