using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

public class LocationTrackingScript : MonoBehaviour {
    
    public UnityEngine.UI.Text txtPlayerLocation;

    public LocationInfo currentGPSLocation { private set; get; }
    public Vector3 currentWorldLocation { private set; get;  }

    private bool wasInitiated = false;

	// Use this for initialization
	void Start () {
		// turn on location services, if available 
        //transform.localPosition = new Vector3(0f, 0f, 6378137f);
		Input.location.Start();
	}
	
	// Update is called once per frame
	void Update () {
        
        if(!wasInitiated) {
            txtPlayerLocation.text = "Player\nlocation not available yet";
			if (Input.location.isEnabledByUser && Input.location.status == LocationServiceStatus.Running)
			{
				wasInitiated = true;
			}
        } else {
            //transform.rotation = Quaternion.Euler(0, -Input.compass.magneticHeading, 0);
            currentGPSLocation = Input.location.lastData;
            float latitude = currentGPSLocation.latitude;
			float longitude = currentGPSLocation.longitude;
			float alt = currentGPSLocation.altitude;
            currentWorldLocation = Quaternion.AngleAxis(longitude, -Vector3.up) * Quaternion.AngleAxis(latitude, -Vector3.right) * new Vector3(0, 0, 6378137f);

            //txtPlayerLocation.text = "Player\nlat: " + latitude + ", lon: " + longitude 
            //+ "\nx: " + transform.position.x + ", z: " + transform.position.z + ", y: " + transform.position.y;
            txtPlayerLocation.text = "Player\nlat: " + latitude + ", lon: " + longitude
                + "\nx: " + Camera.main.transform.position.x.ToString("F4")
                + ", y: " + Camera.main.transform.position.y.ToString("F4")
                + ", z: " + Camera.main.transform.position.z.ToString("F4");
		}   
	}

	private float GetDistanceInMeters(float lat1, float lon1, float lat2, float lon2)
	{
		var R = 6378137; // Radius of earth in KM
		var dLat = lat2 * Mathf.PI / 180 - lat1 * Mathf.PI / 180;
		var dLon = lon2 * Mathf.PI / 180 - lon1 * Mathf.PI / 180;
		float a = Mathf.Sin(dLat / 2) * Mathf.Sin(dLat / 2) +
		Mathf.Cos(lat1 * Mathf.PI / 180) * Mathf.Cos(lat2 * Mathf.PI / 180) *
		Mathf.Sin(dLon / 2) * Mathf.Sin(dLon / 2);
		var c = 2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1 - a));
		var d = R * c;
        return d;
	}
}
