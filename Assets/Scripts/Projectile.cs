using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class Projectile : MonoBehaviour
{
    const int LOOKBACK_COUNT = 10;
    static List<Projectile> PROJECTILES = new List<Projectile>();

    [SerializeField]
    private bool _awake = true;
    public bool awake{
        get {return _awake; }
        private set {_awake = value; }
    }

    private Vector3 prevPos;
    private List<float> deltas = new List<float>();
    private Rigidbody rigid;
    private AudioSource audioSource;
    [Header("Audio")]
    public AudioClip whirSound;

    [Header("Skins")]
    public Material defaultMaterial;
    public Material[] skinMaterials; // Array to hold available skin materials
    [SerializeField]
    private Renderer projectileRenderer;
    void Start(){
        rigid = GetComponent<Rigidbody>();
        awake = true;
        prevPos = new Vector3(1000,1000,0);
        deltas.Add(1000);
        PROJECTILES.Add(this);

        audioSource = GetComponent<AudioSource>();
        audioSource.clip = whirSound;
        audioSource.loop = true;
        audioSource.playOnAwake = false;

        if (projectileRenderer == null) {
        projectileRenderer = GetComponent<Renderer>();
        }

        if (projectileRenderer == null) {
            Debug.LogError("Renderer component not found on the projectile GameObject or its children.");
            return;
        }

        // Apply the saved skin from PlayerPrefs
        int selectedSkin = PlayerPrefs.GetInt("SelectedSkin", -1);
        if (selectedSkin >= 0) {
            ApplySkin(selectedSkin);
        } else {
            ApplyDefaultSkin();
        }
    }

    void FixedUpdate(){
        if(rigid.isKinematic || !awake) return;

        Vector3 deltaV3 = transform.position - prevPos;
        deltas.Add(deltaV3.magnitude);
        prevPos = transform.position;

        while(deltas.Count > LOOKBACK_COUNT){
            deltas.RemoveAt(0);
        }

        float maxDelta = 0;
        foreach(float f in deltas){
            if(f > maxDelta) maxDelta = f;
        }

        if(maxDelta <= Physics.sleepThreshold){
            awake = false;
            rigid.Sleep();
            audioSource.Stop();
        }
        if (!audioSource.isPlaying && awake) {
            audioSource.Play();
        }
    }

    public void OnDestroy(){
        PROJECTILES.Remove(this);
        audioSource.Stop();
    }

    static public void DESTROY_PROJECTILES(){
        foreach(Projectile p in PROJECTILES){
            Destroy(p.gameObject);
        }
    }
     public void ApplySkin(int skinIndex) {
        if (skinMaterials != null && skinIndex < skinMaterials.Length) {
            projectileRenderer.material = skinMaterials[skinIndex];
        } else {
            ApplyDefaultSkin();
        }
    }

    // Apply the default material if no skin is selected
    public void ApplyDefaultSkin() {
        if (defaultMaterial != null) {
            projectileRenderer.material = defaultMaterial;
        }
    }
}
