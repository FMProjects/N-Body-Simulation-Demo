using System.Collections.Generic;
using UnityEngine;

namespace NaiveImpementation
{
    class Body
    {
        public Vector3 position { get; private set; }
        Vector3 velocity;
        Vector3 acceleration;
        float mass;

        float distanceThreshold = 0.1f;

        public Body(Vector3 position, Vector3 velocity, float mass)
        {
            this.position = position;
            this.velocity = velocity;
            this.acceleration = Vector3.zero;
            this.mass = mass;
        }

        public void KinematicsUpdate(ICollection<Body> bodies, float timeElapsed)
        {
            UpdateAcceleration(bodies);
            UpdatePosition(timeElapsed);
        }

        private void UpdateAcceleration(ICollection<Body> bodies)
        {
            Vector3 totalForce = Vector3.zero;

            foreach (var b in bodies)
            {
                if (b != this)//we dont compute gravity on self, this will result in division by zero
                {
                    //calculate magnitude and direction of force
                    Vector3 directionVector = b.position - this.position;
                    float distance = directionVector.magnitude;

                    if (distance < distanceThreshold) continue;
                    totalForce += b.mass * this.mass * directionVector.normalized / (distance * distance);
                }
            }

            acceleration = totalForce / mass;
        }

        private void UpdatePosition(float timeElapsed)
        {
            position += timeElapsed * velocity + 0.5f * timeElapsed * timeElapsed * acceleration;
            velocity += timeElapsed * acceleration;
        }
    }
}