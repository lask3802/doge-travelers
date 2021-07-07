using System;
using UnityEngine;
using UnityTemplateProjects.Meteoroid;

namespace UnityTemplateProjects
{
    public class MeteoroidTarget : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetKey(KeyCode.W) && transform.position.y < 20)
            {
                transform.position += Vector3.up / 5;
            }
            if (Input.GetKey(KeyCode.S) && transform.position.y > -20)
            {
                transform.position += Vector3.down / 5;
            }
            if (Input.GetKey(KeyCode.A) && transform.position.x > -20)
            {
                transform.position += Vector3.left / 5;
            }
            if (Input.GetKey(KeyCode.D) && transform.position.x < 20)
            {
                transform.position += Vector3.right / 5;
            }
        }


        private void OnCollisionEnter(Collision other)
        {
        }
    }
}