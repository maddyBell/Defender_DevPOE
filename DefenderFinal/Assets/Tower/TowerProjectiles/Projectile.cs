using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    //setting up the projectile that the tower and defenders throw at the enemies 
    public float speed = 10f;
    private Transform target;
    private int damage;
    public Material projMaterial;
    public GameManager gameManager;
    public Vector3 vfxLocalOffset = Vector3.zero;
    public float engulfDuration = 3f;
    public WeatherGeneration weatherGeneration;


    //getting the target and how much damage needs to be done when the projectile is thrown and destroying it accordingly 
    public void Launch(Transform enemyTarget, int damageValue, int upgradeLevel, GameManager gameManager, WeatherGeneration weatherGeneration)
    {
        projMaterial = GetComponent<Renderer>().material;
        projMaterial.SetInt("_UpgradeLevel", upgradeLevel);
        this.gameManager = gameManager;
        target = enemyTarget;
        damage = damageValue;
       this.weatherGeneration = weatherGeneration;
        Destroy(gameObject, 8f); //destroys after 8s if it doesnt hit the enemy
        speed -= weatherGeneration.GetRainMultiplier();
    }

    void Update() // moving the projectile toward the targetted enemy using its position and direction 
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
        }
        if (weatherGeneration == null)
        {
            weatherGeneration = FindObjectOfType<WeatherGeneration>();
        }

        Vector3 dir = (target.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;
        transform.forward = dir;
    }

// applying damage to the enemy if it successfully collides 
    private void OnTriggerEnter(Collider other)
    {
       
        if (other == null) return;

        if (other.TryGetComponent<Enemy>(out Enemy enemy))
        {
            if (gameManager == null)
            {
                gameManager = FindObjectOfType<GameManager>();
            }
             gameManager.flamePrefab.GetComponent<ParticleSystem>().Play();
            
            ParticleSpawner.SpawnParticles(gameManager.flamePrefab, other.transform, vfxLocalOffset);

            enemy.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }


    }
}