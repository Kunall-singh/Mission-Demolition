using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slingshot : MonoBehaviour
{
    [Header("Inscribed")]
    public GameObject projectilePrefab;
    public float velocityMult = 10f;
    public GameObject projLinePrefab;
    [Header("Audio Clips")]
    public AudioClip snapSound;
    public AudioClip whirSound;

    [Header("Dynamic")]
    public GameObject launchPoint;
    public Vector3 launchPos;
    public GameObject projectile;
    public bool aimingMode;
    private LineRenderer lineRenderer;
     private AudioSource audioSource;
    void Awake() {
        Transform launchPointTrans = transform.Find("LaunchPoint");
        launchPoint = launchPointTrans.gameObject;
        launchPos = launchPointTrans.position;
        launchPoint.SetActive( false );   
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2; 
        audioSource = gameObject.AddComponent<AudioSource>();
    }
    void OnMouseEnter() {
        launchPoint.SetActive(true);
    }
    void OnMouseExit() {
        launchPoint.SetActive(false); 
    }

    void OnMouseDown(){
        aimingMode = true;
        projectile = Instantiate(projectilePrefab) as GameObject;
        projectile.transform.position = launchPos;
        projectile.GetComponent<Rigidbody>().isKinematic = true;
        lineRenderer.enabled = true;
    }
    void Update() {
        if(!aimingMode) return;

        Vector3 mousePos2D = Input.mousePosition;
        mousePos2D.z = -Camera.main.transform.position.z;
        Vector3 mousePos3D = Camera.main.ScreenToWorldPoint(mousePos2D);

        Vector3 mouseDelta = mousePos3D - launchPos;
        float maxMagnitude = this.GetComponent<SphereCollider>().radius;
        if(mouseDelta.magnitude > maxMagnitude){
            mouseDelta.Normalize();
            mouseDelta *= maxMagnitude;
        }   
        Vector3 projPos = launchPos + mouseDelta;
        projectile.transform.position = projPos;

        lineRenderer.SetPosition(0, launchPos);
        lineRenderer.SetPosition(1, projPos);

        if(Input.GetMouseButtonUp(0)){
            aimingMode = false;
            Rigidbody projRB = projectile.GetComponent<Rigidbody>();
            projRB.isKinematic = false;
            projRB.collisionDetectionMode = CollisionDetectionMode.Continuous;
            projRB.linearVelocity = -mouseDelta*velocityMult;
            FollowCam.SWITCH_VIEW(FollowCam.eView.slingShot);
            FollowCam.POI = projectile;
            Instantiate<GameObject>(projLinePrefab, projectile.transform);
            projectile = null;
            MissionDemolition.SHOT_FIRED();
            lineRenderer.enabled = false;
            if (snapSound != null) {
                audioSource.clip = snapSound; // Assign the clip manually
                audioSource.Play();
            } else {
                Debug.LogWarning("Snap sound not assigned in the Slingshot script.");
            }
        }
    }
}
