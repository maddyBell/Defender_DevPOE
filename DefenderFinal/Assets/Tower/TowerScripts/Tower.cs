using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tower : MonoBehaviour
{

    public int maxHealth = 100;
    private int health;
    public int attackDamage = 25;


    public GameObject projectilePrefab;
    public Transform firePoint;
    public float fireInterval = 3f;

    public GameObject attackRadiusObject;
    public float attackRadius = 25f;


    public float closeDamageInterval = 5f;
    //public int closeDamageAmount = 2;

    //Getting base damage update meshes
    public Mesh midMesh;
    public Mesh lowMesh;

    //Upgrade 1 meshes

    public Mesh upgrade1BaseMesh, upgrade1MidMesh, upgrade1LowMesh;

    //Upgrade 2 meshes
    public Mesh upgrade2BaseMesh, upgrade2MidMesh, upgrade2LowMesh;

    //Upgrade Double meshes
    public Mesh upgradeDoubleBaseMesh, upgradeDoubleMidMesh, upgradeDoubleLowMesh;


    public int towerUpgradeLevel = 0; // means no upgrades have been applied
    public String towerType;

    public int projectileUpgradeLevel = 0; // to keep track of the change for projectile


    // Colliders
    public SphereCollider attackRadiusCollider;
    public SphereCollider closeRadiusCollider;

    // Tracking enemies
    private List<Transform> enemiesInRange = new List<Transform>();
    private List<Enemy> closeRangeEnemies = new List<Enemy>();

    private Coroutine firingCoroutine;
    private Coroutine closeRangeCoroutine;
    public GameManager gameManager;

    public Image healthBar;
    public Mesh currentMesh;

    public DamageVFX damageVFX;
    public WeatherGeneration weatherGeneration;

    void Start() // setting up the health, making sure the colliders are in and finding the game manager 
    {
        health = maxHealth;

        if (attackRadiusCollider == null || closeRadiusCollider == null)
        {
            Debug.LogError("Please assign both radius colliders in the Inspector!");
        }
        gameManager = FindObjectOfType<GameManager>();
        currentMesh = GetComponent<MeshFilter>().mesh;
        damageVFX = FindObjectOfType<DamageVFX>();
        weatherGeneration = FindObjectOfType<WeatherGeneration>();
        
    }

    void Update()
    {
        GetComponent<MeshFilter>().mesh = currentMesh;
        //attackRadius -= weatherGeneration.GetFogMultiplier();
        attackRadiusObject.GetComponent<SphereCollider>().radius = attackRadius;
    }

    // Called by the child radius trigger scripts
    public void EnemyEnteredAttackRadius(Transform enemy)
    {
        if (!enemiesInRange.Contains(enemy))
            enemiesInRange.Add(enemy); // keeping track of all the enemies within the fireing range 

        if (firingCoroutine == null)
            firingCoroutine = StartCoroutine(FireAtEnemies()); // starts a coroutine to fire at the enemies, easier to structure in a way that feels more realistic and not just spam shoot projectiles 
    }

    public void EnemyExitedAttackRadius(Transform enemy)
    {
        //removes the enemy from the attack radius mainly to prevent the tower from shooting in the directions of enemies that have already died 
        enemiesInRange.Remove(enemy);

        if (enemiesInRange.Count == 0 && firingCoroutine != null)
        {
            StopCoroutine(firingCoroutine);
            firingCoroutine = null;
        }
    }

    public void EnemyEnteredCloseRange(Enemy enemy) // checking if the enemies have entered the close rrange trigger where they can apply damage to the tower 
    {
        if (!closeRangeEnemies.Contains(enemy))
            closeRangeEnemies.Add(enemy);

        if (closeRangeCoroutine == null)
            closeRangeCoroutine = StartCoroutine(ApplyCloseDamage());
    }

    public void EnemyExitedCloseRange(Enemy enemy) // checking if the enemy is still there again to prevent stupid shooting 
    {
        closeRangeEnemies.Remove(enemy);

        if (closeRangeEnemies.Count == 0 && closeRangeCoroutine != null)
        {
            StopCoroutine(closeRangeCoroutine);
            closeRangeCoroutine = null;
        }
    }

    //setting up the projectiles to fire at the enemies direction and taking a small break between firing for a realistic feel 
    private IEnumerator FireAtEnemies()
    {
        while (enemiesInRange.Count > 0)
        {
            Transform target = ClosestEnemy();
            if (target != null)
            {
                GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
                Projectile projScript = projectile.GetComponent<Projectile>();
                if (projScript != null)
                    projScript.Launch(target, attackDamage,projectileUpgradeLevel,gameManager,weatherGeneration);
            }
            yield return new WaitForSeconds(fireInterval);
        }
    }

    //getting the position of the enemy closest to the tower, to set it as the priority target 
    private Transform ClosestEnemy()
    {
        Transform closest = null;
        float minDistance = Mathf.Infinity;

        foreach (Transform enemy in enemiesInRange)
        {
            if (enemy == null) continue;
            float dist = Vector3.Distance(transform.position, enemy.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closest = enemy;
            }
        }

        return closest;
    }

    //taking damage from the enemies that are within the range to inflict damage 
    private IEnumerator ApplyCloseDamage()
    {
        while (closeRangeEnemies.Count > 0)
        {
            foreach (Enemy enemy in closeRangeEnemies)
            {
                if (enemy != null)
                    TakeDamage(enemy.GetTowerDamage()); // enemy damage method
            }
            yield return new WaitForSeconds(closeDamageInterval);
        }
    }

    //actually applying the damage the enemies are giving, updating the health bar and adding a visual destruction effect by changing the mesh filter 
    public void TakeDamage(int damage)
    {
        health -= damage;
        healthBar.fillAmount = (float)health / maxHealth;

        if (health <= 66 && midMesh != null)
        {
            currentMesh = midMesh;
            if (damageVFX != null)
            {
                damageVFX.Hit(0.3f); // playing screen vfx 
            }
        }

        if (health <= 33 && lowMesh != null)
        {
            currentMesh = lowMesh;
            if (damageVFX != null)
            {
                damageVFX.Hit(0.6f); 
            }
        }
        if (health <= 0)
        {
            Die();
        }
    }

    //run when the tower has died, destroying the object and triggering the game over screen 

    private void Die()
    {
        Debug.Log("Tower destroyed");
        gameManager.TowerDead(true);
        Destroy(gameObject);
    }

    //upgrading the tower 

    public void UpgradeTower(string upgradeType) // have game manager call this one 
    {
        if (upgradeType == "Flame" )
        {
            UpgradeLevel1();
            gameManager.SpendBones(30);
            projectileUpgradeLevel = 1;
        }
        else if (upgradeType == "Poison" )
        {
            UpgradeLevel2();
            gameManager.SpendBones(35);
            projectileUpgradeLevel = 2;
        }

    }

    public void UpgradeLevel1()
    {
        health += maxHealth * 25 / 100; // increase health by 25%

        attackDamage += attackDamage * 50 / 100; // increase attack damage by 50%

        if (gameManager.GetBones() < 30)
        {
            Debug.Log("Not enough bones");
            return;
        }

        if (towerUpgradeLevel == 0) // making sure upgrade 1 mesh is applied only when it is the first upgrade 
        {
            // Update mesh based on current health
            if (health >= maxHealth / 3 * 2)
            {
                currentMesh = upgrade1BaseMesh;
                midMesh = upgrade1MidMesh;
                lowMesh = upgrade1LowMesh;
            }
            else if (health < maxHealth / 3 * 2 && health >= maxHealth / 3 )
            {
                currentMesh = upgrade1MidMesh;
                lowMesh = upgrade1LowMesh;
            }
            else
            {
                currentMesh = upgrade1LowMesh;
            }
            towerUpgradeLevel = 1; // one upgrade has been applied
        }
        else if (towerUpgradeLevel == 1)
        {
            UpgradeDouble(); // if the tower is already at upgrade level 1, applying the double upgrade instead
            towerUpgradeLevel = 2; // one upgrade has been applied
        }
        else
        {
            Debug.Log("Tower is already at maximum upgrade level.");
        }


    }
    public void UpgradeLevel2() // more powerful upgrade
    {
        health += maxHealth * 50 / 100; // increase health by 25%

        attackDamage += attackDamage * 75 / 100; // increase attack damage by 50%

         if(gameManager.GetBones() < 35)
        {
            Debug.Log("Not enough bones");
            return;
        }


        if (towerUpgradeLevel == 0) // making sure upgrade 2 mesh is applied only when it is the second upgrade 
        {
            // Update mesh based on current health
            if (health >= maxHealth / 3 * 2)
            {
                currentMesh = upgrade2BaseMesh;
                midMesh = upgrade2MidMesh;
                lowMesh = upgrade2LowMesh;
            }
            else if (health < maxHealth / 3 * 2 && health >= maxHealth / 3)
            {
                currentMesh = upgrade2MidMesh;
                lowMesh = upgrade2LowMesh;
            }
            else
            {
                currentMesh = upgrade2LowMesh;
            }
            towerUpgradeLevel = 1; // one upgrade has been applied

        }
        else if (towerUpgradeLevel == 1)
        {
            UpgradeDouble(); // if the tower is already at upgrade level 2, applying the double upgrade instead
            towerUpgradeLevel = 2; // one upgrade has been applied
        }
        else
        {
            Debug.Log("Tower is already at maximum upgrade level.");
        }


    }

    public void UpgradeDouble()
    {
        if (health >= maxHealth / 3 * 2)
        {
            currentMesh = upgradeDoubleBaseMesh;
            midMesh = upgradeDoubleMidMesh;
            lowMesh = upgradeDoubleLowMesh;
        }
        else if (health < maxHealth / 3 * 2 && health >= maxHealth / 3)
        {
            currentMesh = upgradeDoubleMidMesh;
            lowMesh = upgradeDoubleLowMesh;
        }
        else
        {
            currentMesh = upgradeDoubleLowMesh;
        }
        projectileUpgradeLevel = 3;
    }
}
    

    /* Explanation of the Upgrade Logic: 
        There are two upgrades that the player can apply. 
        The first is the "Poison Attack" this will cost 20 bones and increase the tower's attack damage by 50% and health by 25%.
        The second is the "Flaming Attack" this will cost 30 bones and increase the tower's attack damage by 75% and health by 50%.

        I already have the tower with 3 different meshes that represent the damage that it has taken (base, mid, low)
        This needed to be represented through the upgrades as well.
        This is why Upgrade1 and Upgrade2 check the health and set the correct meshes. 
        The tower can never regain health other than when an upgrade is applied which is why the lower health meshes dont reset the higher level meshes values

        There is also the double upgrade meshes becasue i have allowed the upgrades to be stackable 
        THe upgradeDouble meshes are a combination of the visual changes from both upgrades
        The system allows for the stats to be updated correctly when stacked but lets the meshes be changed to match them

        ***Thank you for coming to me Ted Talk***
    */

    /* BEEP BEEP IDEAAAA 

        cos posion and flame attacks, make a shader to change the color for each and double 
        find out how to make the shader take input...maybe throigh the material using variables ? 
        acts as both visual improvement and fulls need for base changing shader 

    */
        
    