using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class FogLayer
{
    public GameObject fogObject; // Reference to the fog object
    public Renderer fogRenderer; // Distance of the fog layer from the camera
    public Color color; // Color of the fog layer
    public float fogTimer; // Scale of the fog layer
}

public class Fog : MonoBehaviour
{
    [Header("Fog Settings")]
    [SerializeField] private Color fogColor = new Color(0,0,0); // Default fog color
    [SerializeField] private float fogDensity = 0.05f; // Default fog density
    [SerializeField] private float fogStartDistance = 0f; // Distance at which fog starts
    [SerializeField] private float fogEndDistance = 100f; // Distance at which fog ends
    [SerializeField][Range(1,100)] private int fogLayers = 5; // layers of fog
    [SerializeField][Range(0,10)] private float lerpSpeed = 0.2f;
    [SerializeField] private AnimationCurve fogOpacityCurve;
    [SerializeField] private Material fogMaterial; // Reference to the fog material
    [SerializeField] private GameObject follow; // Reference to the fog material


    public List<FogLayer> fogObjects; // Reference to the fog object
    private float fogIntervalDistance; // Distance between fog objects

    private void Awake()
    {
        ActivateFog(); // Activate fog if enabled

    }

    private void Update()
    {
        if (fogObjects[0] != null) // Check if fogObject is not null
        {
            UpdateFog(); // Update the fog position and scale
        }
    }

    private void OnValidate()
    {
        //ensure valid values
        fogEndDistance = Mathf.Max(fogEndDistance, fogStartDistance); // Ensure end distance is greater than start distance
                
    }

    public void ActivateFog()
    {
        RenderSettings.fog = true; // Enable fog
        RenderSettings.fogColor = fogColor; // Set the fog color
        RenderSettings.fogDensity = fogDensity; // Set the fog density
        RenderSettings.fogStartDistance = fogEndDistance; // Set the fog start distance
        RenderSettings.fogEndDistance = fogEndDistance; // Set the fog end distance
        CreateFogs();

    }
    public void DeactivateFog()
    {
        RenderSettings.fog = false; // Disable fog
        for (int i = 0; i < fogLayers; i++)
        {
            if (fogObjects[i] != null)
            {
                fogObjects[i].fogObject.SetActive(false);
            }
        }
    }

    private void CreateFogs()
    {

        if (follow == null) // Check if follow object is not assigned
        {
            follow = GameObject.FindWithTag("Player"); // Assign the main camera as the follow object
        }

        //make fog
        if (fogObjects == null || fogObjects.Count == 0)
        {
            fogObjects = new List<FogLayer>(fogLayers); // Initialize the fog object list
        }
        
        fogIntervalDistance = (fogEndDistance - fogStartDistance) / fogLayers; // Calculate the amount of fog objects based on distance and interval

        //make new or daactivate old fogs depending on active fogs
        for (int i = 0; i < Mathf.Max(fogObjects.Count,fogLayers); i++)
        {
            if (i < fogLayers)
            {
                if (fogObjects.Count <= i) // Check if the fog object list is smaller than the number of layers
                {
                    FogLayer layer = new FogLayer(); // Create a new fog layer
                    fogObjects.Add(layer); // Add a null entry to the list
                }
                if (fogObjects[i].fogObject == null)
                {
                    fogObjects[i].fogObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    fogObjects[i].fogObject.name = ("FogLayer" + i); 
                    fogObjects[i].fogObject.transform.SetParent(transform); // Set the parent to this object
                    fogObjects[i].fogObject.transform.position = follow.transform.position; // Set the position of the fog layer
                    fogObjects[i].fogObject.transform.localScale = new Vector3(fogEndDistance,fogEndDistance,fogEndDistance); // Set the position of the fog layer
                    fogObjects[i].fogObject.GetComponent<MeshRenderer>().material = fogMaterial; // Add a MeshRenderer and set the material
                    fogObjects[i].fogObject.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off; // Disable shadow casting
                    fogObjects[i].fogObject.GetComponent<MeshRenderer>().receiveShadows = false; // Disable shadow receiving
                    fogObjects[i].fogObject.GetComponent<SphereCollider>().enabled = false; 
                    fogObjects[i].fogRenderer = fogObjects[i].fogObject.GetComponent<Renderer>();
                    fogObjects[i].color = fogObjects[i].fogRenderer.material.color;

                    
                }
                else if (fogObjects[i].fogObject != null && fogObjects[i].fogObject.activeSelf == false) // Check if the fog object already exists and is inactive
                {
                    fogObjects[i].fogObject.SetActive(true); // Activate the fog object if it already exists
                }
            }
            else if (i > fogLayers) // If the fog object is not needed anymore, deactivate it
            {
                fogObjects[i].fogObject.SetActive(false); // Deactivate the fog object
            }
        }
        UpdateFog();
    }


    private void UpdateFog()
    {
        RenderSettings.fogStartDistance = Mathf.Lerp(fogEndDistance, fogStartDistance,lerpSpeed*Time.deltaTime); // Set the fog start distance
        // Update the fog position based on the follow object
        for (int i = 0; i < fogLayers; i++)
        {
            if (fogObjects[i].fogObject != null)
            {
                float color = 255*fogOpacityCurve.Evaluate((float)i/fogLayers);
                fogObjects[i].fogRenderer.material.color = new Color(fogColor.r, fogColor.g, fogColor.b, color);
                fogObjects[i].fogObject.transform.position = Vector3.Lerp(fogObjects[i].fogObject.transform.position,follow.transform.position,lerpSpeed*Time.deltaTime); // Set the position of the fog layer
                fogObjects[i].fogObject.transform.localScale = Vector3.Lerp( fogObjects[i].fogObject.transform.localScale, new Vector3(fogStartDistance + fogIntervalDistance * i, fogStartDistance + fogIntervalDistance * i, fogStartDistance + fogIntervalDistance * i),lerpSpeed*Time.deltaTime); // Set the scale of the fog layer
            }
        }
    }

}