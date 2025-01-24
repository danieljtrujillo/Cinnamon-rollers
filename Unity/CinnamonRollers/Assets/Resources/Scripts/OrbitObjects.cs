using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OrbitObjects : MonoBehaviour
{
    [System.Serializable]
    public class OrbitingObject
    {
        public GameObject target;
        public float speed = 20f;       // Speed of the orbit
        public float radius = 5f;       // Distance from the center object
        public Vector3 orbitDirection = Vector3.up; // Default orbit direction is up (i.e., around the Y-axis)

        [HideInInspector]
        public float currentAngle = 0f; // To keep track of the object's position in its orbit
    }

    public GameObject centerObject;  // The object around which the others will orbit
    public OrbitingObject[] orbiters; // List of objects to orbit the center

    private void Update()
    {
        foreach (OrbitingObject orbiter in orbiters)
        {
            // Calculate the new angle based on speed and time
            orbiter.currentAngle += orbiter.speed * Time.deltaTime;

            // Determine the new position based on the center object's position, the radius, and the current angle
            Vector3 offset = new Vector3(Mathf.Sin(orbiter.currentAngle) * orbiter.radius, 0, Mathf.Cos(orbiter.currentAngle) * orbiter.radius);
            orbiter.target.transform.position = centerObject.transform.position + Quaternion.Euler(orbiter.orbitDirection) * offset;
        }
    }
}
