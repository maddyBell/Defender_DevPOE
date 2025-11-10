using UnityEngine;

public class ParticleSpawner : MonoBehaviour
{
    public static void SpawnParticles(GameObject prefab, Transform parent, Vector3 localOffset)
    {
        if (prefab == null)
            return;

        // Instantiate the particle prefab
        GameObject obj = Instantiate(prefab, parent.position + parent.TransformVector(localOffset), Quaternion.identity, parent);
        obj.transform.localPosition = localOffset;

        // Set the particle system colors
        var particleSystems = obj.GetComponentsInChildren<ParticleSystem>();
        foreach (var ps in particleSystems)
        {
            ps.Play(); // Start the particle system immediately
        }
    }
}