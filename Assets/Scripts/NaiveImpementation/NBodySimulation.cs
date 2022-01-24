using System.Collections.Generic;
using UnityEngine;
using BodyN = NaiveImpementation.Body;

public class NBodySimulation : MonoBehaviour
{
    List<Transform> quadBodies;
    List<BodyN> bodies;

    int gridsSize = 25;

    // Start is called before the first frame update
    void Start()
    {
        quadBodies = new List<Transform>();
        bodies = new List<BodyN>();

        for (int i = 0; i < gridsSize; i++)
        {
            for (int j = 0; j < gridsSize; j++)
            {
                //instantiate bodies
                var newBody = GameObject.CreatePrimitive(PrimitiveType.Quad);
                newBody.transform.parent = this.transform;
                newBody.transform.localScale = Vector3.one * 0.5f;
                quadBodies.Add(newBody.transform);

                Vector3 position = new Vector3(-gridsSize / 2 + i, -gridsSize / 2 + j) * 1.5f;
                bodies.Add(new BodyN(position, (Random.insideUnitSphere + Vector3.Cross(position, Vector3.back).normalized), 1e-1f));
            }
        }
    }

    private void Update()
    {
        for (int i = 0; i < bodies.Count; i++)
        {
            bodies[i].KinematicsUpdate(bodies, Time.deltaTime);
            quadBodies[i].transform.position = bodies[i].position;
        }
    }
}
