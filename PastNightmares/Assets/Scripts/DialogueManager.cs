using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class DialogueManager : MonoBehaviour
{
    private Queue<string> sentences;
    //[SerializeField]private GameObject dialogueCanvas;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    private PlayerInput playerInput;
    public Animator dialogueBoxAnimator;
    private PlayerPickupDrop playerPickupDrop;
    public static bool playerIsTalking = false;



    void Awake ()
    {
        playerInput = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();
        playerPickupDrop = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPickupDrop>();
    }


    // Start is called before the first frame update
    void Start()
    {
        sentences = new Queue<string>();
    }


    public void StartDialogue(Dialogue dialogue)
    {
        Debug.Log("Started conversation with " + dialogue.name + ".");
        Cursor.lockState = CursorLockMode.None;
        //dialogueCanvas.SetActive(true);
        playerInput.enabled = false;
        playerPickupDrop.enabled = false;

        playerIsTalking = true;

        dialogueBoxAnimator.SetBool("IsOpen", true);

        nameText.text = dialogue.name;

        sentences.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }


    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }
        Debug.Log("Next Sentence.");
        string sentence = sentences.Dequeue();
        //dialogueText.text = sentence;
        StopCoroutine("TypeSentence"); //if a sentence is still being typed, then stop it and type the next
        StartCoroutine(TypeSentence(sentence));
    }


    private IEnumerator TypeSentence (string sentence)
    {
        dialogueText.text = "";

        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return null; //waiting a single frame
        }
    }


    private void EndDialogue()
    {
        Debug.Log("End of conversation with " + nameText.text + ".");
        //dialogueCanvas.SetActive(false);
        playerInput.enabled = true;
        playerPickupDrop.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;

        playerIsTalking = false;

        dialogueBoxAnimator.SetBool("IsOpen", false);
    }
    
}
