using System;
using UnityEngine;
using UnityTemplateProjects.Meteoroid;

namespace UnityTemplateProjects
{
    public class MeteoroidTarget : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetKey(KeyCode.W) && transform.position.y < 10)
            {
                transform.position += Vector3.up / 10;
            }
            if (Input.GetKey(KeyCode.S) && transform.position.y > -10)
            {
                transform.position += Vector3.down / 10;
            }
            if (Input.GetKey(KeyCode.A) && transform.position.x > -10)
            {
                transform.position += Vector3.left / 10;
            }
            if (Input.GetKey(KeyCode.D) && transform.position.x < 10)
            {
                transform.position += Vector3.right / 10;
            }
        }


        private void OnCollisionEnter(Collision other)
        {
        }
    }
}