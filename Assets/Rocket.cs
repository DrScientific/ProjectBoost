using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    Rigidbody rigidbody;
    AudioSource audioSource;
    [SerializeField] AudioClip thrustAudio;
    [SerializeField] AudioClip rcsAudio;
    [SerializeField] AudioClip explosionAudio;
    [SerializeField] AudioClip levelCompleteAudio;
    [SerializeField] ParticleSystem thrustParticleSystem;
    [SerializeField] ParticleSystem deathParticleSystem;
    [SerializeField] ParticleSystem levelCompleteParticleSystem;
    [SerializeField] float thrustForce = 10f;
    [SerializeField] float rcsThrust = 10f;
    [SerializeField] float levelLoadDelay = 2f;
    bool isInvincible = false;
    bool isTransitioning = false;

    enum PlayerState { Alive, Dying, Transcending }

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!isTransitioning)
        {
            ProcessInput();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isTransitioning)
        {
            switch (collision.gameObject.tag)
            {
                case "Friendly":
                    break;
                case "Finish":
                    StartLevelCompleteSequence();
                    break;
                default:
                    if (!isInvincible)
                    {
                        StartDeathSequence();
                    }
                    break;
            }
        }
    }

    void ProcessInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            ProcessThrust();
        }
        else if (audioSource.isPlaying)
        {
            thrustParticleSystem.Stop();
            audioSource.Stop();
        }
        if (Input.GetKey(KeyCode.A))
        {
            ProcessRCS(true);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            ProcessRCS(false);
        }
        if (Debug.isDebugBuild)
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                LoadNextLevel();
            }
            else if (Input.GetKeyDown(KeyCode.I))
            {
                isInvincible = !isInvincible;
            }
        }
    }

    void ProcessThrust()
    {
        rigidbody.AddRelativeForce(Vector3.up * thrustForce * Time.deltaTime);
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(thrustAudio);
        }
        if(!thrustParticleSystem.isPlaying)
        {
            thrustParticleSystem.Play();
        }
    }

    void ProcessRCS(bool rotateLeft)
    {
        Vector3 rotationAxis = rotateLeft ? Vector3.forward : Vector3.back;

        rigidbody.angularVelocity = Vector3.zero;

        float rotationThisFrame = rcsThrust * Time.deltaTime;
        transform.Rotate(rotationAxis * rotationThisFrame);
    }

    private void StartLevelCompleteSequence()
    {
        isTransitioning = true;
        levelCompleteParticleSystem.Play();
        audioSource.Stop();
        audioSource.PlayOneShot(levelCompleteAudio);
        Invoke("LoadNextLevel", levelLoadDelay);
    }

    private void StartDeathSequence()
    {
        isTransitioning = true;
        deathParticleSystem.Play();
        audioSource.Stop();
        audioSource.PlayOneShot(explosionAudio);
        Invoke("LoadFirstLevel", levelLoadDelay);
    }

    private void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
    }

    private void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = (currentSceneIndex == SceneManager.sceneCountInBuildSettings - 1 ? 0 : currentSceneIndex + 1);
        SceneManager.LoadScene(nextSceneIndex);
    }
}
