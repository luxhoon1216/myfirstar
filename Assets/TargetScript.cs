using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetScript : MonoBehaviour {

    public Transform player;
    public UnityEngine.UI.Text txtTargetLocation;
    public UnityEngine.UI.Text txtDistanceFromPlayer;

    private long id { set; get; }
    // x: longitude, y: altitude, z: latitude
    private Vector3 gpsLocation { set; get; }
    private Transform location { set; get; }

	// Use this for initialization
	void Start () {
        //transform.position = new Vector3(6378137f, 0f, 0f);
	}
	
	// Update is called once per frame
	void Update () {

        gpsLocation = getGPSLocation();

        var playerWorldLocation = player.GetComponent<LocationTrackingScript>().currentWorldLocation;

        var latitude = gpsLocation.y;
        var longitude = gpsLocation.x;
        transform.position = Quaternion.AngleAxis(longitude, -Vector3.up) * Quaternion.AngleAxis(latitude, -Vector3.right) * new Vector3(0, 0, 6378137f) - playerWorldLocation;
        txtTargetLocation.text = "Target\nlat: " + latitude + ", lon: " + longitude
            + "\nx: " + transform.position.x.ToString("F4")
            + ", z: " + transform.position.z.ToString("F4")
            + ", y: " + transform.position.y;
        txtDistanceFromPlayer.text = "Distance: " + Vector3.Distance(Camera.main.transform.position, transform.position);
	}

    private Vector3 getGPSLocation() {
        return new Vector3(-93.418356f, 44.851446f, 1f);
    }
}
