using UnityEngine;

public class WitchRoute : MonoBehaviour
{
    [SerializeField] private Transform[] routePoints;

    public Vector3 GetRandomPointInsideCollider()
    {
        // If this method is keeped, then need to transform all the points within routePoints to WorldPosition.
        // Also need to come up with an idea of how to transform them cheaply, if there'll be random points within unit sphere.
        //return routePoints[UnityEngine.Random.Range(0, routePoints.Length - 1)].localToWorldMatrix.GetPosition();

        return routePoints[UnityEngine.Random.Range(0, routePoints.Length - 1)].position;
    }
}
