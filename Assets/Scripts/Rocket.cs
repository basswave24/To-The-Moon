using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{

    [SerializeField] float rcsThrust = 100f; //can be changed in unity to make rocket rotate faster/slower
    [SerializeField] float mainThrust = 100f; //can be changed in unity to make rocket thrust faster/slower
    [SerializeField] float levelLoadDelay = 2f; //won't load next level as fast


    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip DeathSound;
    [SerializeField] AudioClip LoadSound;


    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem DeathParticles;
    [SerializeField] ParticleSystem LoadParticles;

    Rigidbody rigidBody;
    AudioSource audioSource;

    enum State { Alive, Dying, Transcending }
    State state = State.Alive;

    bool collisionsDisabled = false;



    // Use this for initialization
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Alive)
        {
            RespondToThrustInput();
            RespondToRotateInput();
        } //this is used to prevent you from moving the rocket if you're either DEAD or progressing onto next levl

        if (Debug.isDebugBuild)
        {
            RespondToDebugKeys();
        } //this is used to get through the levels faster
    }

    private void RespondToDebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextLevel(); //press L to get next level
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            collisionsDisabled = !collisionsDisabled; //can hit obstacles and not die 
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive || collisionsDisabled)
        {
            return;
        } //if you're in debugging mode

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                break; //do nothing if you're on a friendly platform, in our case only the beginning platform
            case "Finish":
                StartSuccessSequence();
                break;
            default: //if you hit anything else that doesn't have the tag "Finish", you die
                StartDeathSequence();
                break;
        }
    }

    private void StartDeathSequence()
    {
        state = State.Dying;
        audioSource.Stop(); //this stops the thrusting noise
        audioSource.PlayOneShot(DeathSound);
        mainEngineParticles.Stop();
        DeathParticles.Play();
        Invoke("LoadFirstLevel", levelLoadDelay);
    }

    private void StartSuccessSequence()
    {
        state = State.Transcending;
        audioSource.Stop();
        audioSource.PlayOneShot(LoadSound);
        LoadParticles.Play();
        Invoke("LoadNextLevel", levelLoadDelay);
    }

    private void LoadFirstLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex; //gets current level
        SceneManager.LoadScene(currentSceneIndex); //loads current level
    }

    private void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex; //gets current level
        int NextSceneIndex = currentSceneIndex + 1; //gets next scene 
        SceneManager.LoadScene(NextSceneIndex); //loads next scene
    }

    void RespondToRotateInput()
    {

        rigidBody.freezeRotation = true; //take manual control of rotation

        float rotationThisFrame = rcsThrust * Time.deltaTime; //* Time.deltaTime used for making it smooth


        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        } //rotate left 
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }//rotate right

        rigidBody.freezeRotation = false; //resume physics controls

        /*what .freezeRotation does is whenever A and D are being pressed, the rocket won't react to physics anymore, instead
         * it will react to whatever the user presses. The moment A and D are let go, gravity becomes a thing again, and the rocket
         * will react however */
    }

    private void RespondToThrustInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            ApplyThrust();
        }
        else
        {
            mainEngineParticles.Stop(); //stops particles if not thrusting
            audioSource.Stop();//stops thrusting noises if not thrusting
        }
    }

    private void ApplyThrust()
    {
        rigidBody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngine);
        } //plays the engine noise
        mainEngineParticles.Play(); //engine particles 
    }
}
