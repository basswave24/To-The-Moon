using UnityEngine;

//Declaring using public instead of SerializeField, just to showcase both ways

public class AudioLoudness : MonoBehaviour {

    public AudioSource audioSource;

    public Color color;
    public float updateStep = 0.01f;   //this will update it every 0.01 seconds, this seems to be the most smoothest setting
    public int sampleDataLength = 1024;
    MeshRenderer renderer;
    private float currentUpdateTime = 0f;
    public bool wantColour = false; //this is used to change if we want the cube to change colours or not 
    
    public float clipLoudness;

    private float[] clipSampleData;

    public GameObject gameObject;
    public float sizeFactor = 1; //how much the gameobject will increase by


    public float minSize = 1; //minimum size that the game object can exist.
    public float maxSize = 500; //maximum size that the game object can increase by

    private void Awake()
    {
        clipSampleData = new float[sampleDataLength]; 
        renderer = GetComponent<MeshRenderer>(); //used to change colour
    }

    private void Update()
    {
        currentUpdateTime = currentUpdateTime + Time.deltaTime;
        if(currentUpdateTime >= updateStep) 
        {
            currentUpdateTime = 0f;
            audioSource.clip.GetData(clipSampleData,audioSource.timeSamples);
            clipLoudness = 0f;
            foreach(var sample in clipSampleData)
            {
                clipLoudness = clipLoudness + Mathf.Abs(sample);
            }
            clipLoudness = clipLoudness / sampleDataLength;

            clipLoudness = clipLoudness * sizeFactor;
            clipLoudness = Mathf.Clamp(clipLoudness, minSize, maxSize); //similar to map function in java
            gameObject.transform.localScale = new Vector3(clipLoudness,clipLoudness,clipLoudness); //moves object to music
           

            color = Color.HSVToRGB(clipLoudness/10, clipLoudness, clipLoudness); //divided by 10 because ranges from 0 to 1 

            if (wantColour == true)
            {

                renderer.material.color = color; //changes the colour 
            }
           
            
        }
    }

}
