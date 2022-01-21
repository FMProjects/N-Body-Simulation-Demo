using UnityEngine;

public class CameraLookAround : MonoBehaviour
{
    [SerializeField] Vector3 pointToCircle;
    [SerializeField] float heightOffset;
    [SerializeField] float distance;
    [SerializeField] float timeScale;

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(distance * Mathf.Cos(Time.unscaledTime* timeScale), heightOffset, distance * Mathf.Sin(Time.unscaledTime* timeScale))+ pointToCircle;
        transform.LookAt(pointToCircle);
    }
}
