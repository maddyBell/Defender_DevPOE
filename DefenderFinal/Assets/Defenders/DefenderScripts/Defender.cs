using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class Defender : MonoBehaviour
{
    //public variable to get the needed stats and the details 
    public int health, startHealth = 100, damage = 15;
    public float attackSpeed = 1f;
    public Transform firePoint;
    public GameObject projectilePrefab;
    public Image healthBar;
    public int defenderType; // to identify the type of defender for upgrades, specifically to single out the turrets

    //All Meshes needed 
    public Mesh upgrade1Mesh;
    public Mesh upgrade2Mesh;
    public Mesh upgradeDoubleMesh;
    public Mesh[] TurretMeshes; // array to hold the turret meshes for different tower types

    public Material upgrade1Material;
    public Material upgrade2Material;
    public Material upgradeDoubleMaterial;


    //private variables getting the animtor to play animations, set an attack timer and get the enemy targetted
    private Animator animator;
    private float nextAttackTime = 0f;
    private Enemy currentEnemyTarget;
    private int currentLevel = 0; // keeps track of the upgrade level of the defender to run the correct upgrades and update the mesh accordingly 
    public int projectileUpgradeLevel =0 ;
    public Mesh currentMesh;
    private Tower tower; // getting the tower to ensure that the tower type and turret upgrades match
    public String towerType; // to identify the tower type for matching with turret upgrades
    GameManager gameManager;


    public Material defenderMaterial;
    void Awake()
    {
        animator = GetComponent<Animator>();
        health = startHealth;
        currentMesh = GetComponent<MeshFilter>().mesh;
        tower = FindObjectOfType<Tower>();
        gameManager = FindObjectOfType<GameManager>();
        defenderMaterial = GetComponentInChildren<Renderer>().material;

    }

    //making the defender take damage, updating the health bar and checking if its dead
    public void TakeDamage(int amount)
    {
        health -= amount;
        healthBar.fillAmount = (float)health / startHealth;

        if (health <= 0)
        {
            Die();
        }
    }

    //run when the defender is dead, playing the animation and destroying the object after a delay
    private void Die()
    {
        if (animator != null)
            animator.SetTrigger("Die");

        Destroy(gameObject, 2f); // delay for death anim
    }

    //running the update method, checking if there is an emeny in the defenders range and running the attack funstions 
    private void Update()
    {
        if (currentEnemyTarget != null)
        {
            this.transform.LookAt(currentEnemyTarget.transform);

            // Attack cooldown
            if (Time.time >= nextAttackTime)
            {
                nextAttackTime = Time.time + attackSpeed; // attackSpeed = 5 seconds for example

                // getting the animator to play the attack animation ** some issue with attack, idle may be too long so is messing uop
                if (animator != null)
                    animator.SetBool("IsAttacking", true);

                // spawning a projectile that targets the enemy 
                GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
                Projectile projScript = projectile.GetComponent<Projectile>();
                if (projScript != null)
                    projScript.Launch(currentEnemyTarget.transform, damage, projectileUpgradeLevel, gameManager, tower.weatherGeneration);
            }
        }
        else
        { // revert back to idle animation if no enemies to attack 
            if (animator != null)
                animator.SetBool("IsAttacking", false);
        }
        if (tower == null)
        {
            tower = FindAnyObjectByType<Tower>();
        }
        GetComponent<MeshFilter>().mesh = currentMesh;
    }

    private void OnTriggerEnter(Collider other)
    {
        // ignoring the space radius specifically cos was casuing issues with detecting enemies 
        if (other.gameObject == gameObject || other.transform.IsChildOf(transform))
            return;

        //getting the enemy off the trigger, double chefcking cos some enemy's werent picking up so backup with the parent stuff 
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy == null)
            enemy = other.GetComponentInParent<Enemy>();

        if (enemy != null)
        {
            currentEnemyTarget = enemy;
        }
    }
    // tracks the exit so diesnt get stuck on attacking out of range enemies 
    private void OnTriggerExit(Collider other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null && enemy == currentEnemyTarget)
        {
            currentEnemyTarget = null;
        }
    }

    public void GetTowerType()
    {
        if (tower != null)
        {
            towerType = tower.towerType;
        }
    }

    public void TurretTowerMeshes()
    { // meshes are made to match the visuals of the Tower, with 3 different towers that can be spawned in the game
        GetTowerType();
        switch (towerType)
        {
            case "Big":
                upgrade1Mesh = TurretMeshes[0];
                upgrade2Mesh = TurretMeshes[1];
                upgradeDoubleMesh = TurretMeshes[2];
                break;
            case "Evil":
                upgrade1Mesh = TurretMeshes[3];
                upgrade2Mesh = TurretMeshes[4];
                upgradeDoubleMesh = TurretMeshes[5];
                break;
            case "Hex":
                upgrade1Mesh = TurretMeshes[6];
                upgrade2Mesh = TurretMeshes[7];
                upgradeDoubleMesh = TurretMeshes[8];
                break;
        }
    }

    public void UpgradingDefender(string upgradeType)
    {
        if (defenderType == 3) // checking if turret defender type to load the correct meshes 
        {
            TurretTowerMeshes();
        }

        if (upgradeType == "Poison" )
        {
            Upgrade1();
            


        }
        else if (upgradeType == "Flame" )
        {
            Upgrade2();
            
        }

    }

    public void Upgrade1()
    {
        health += startHealth * 50 / 100; // increase health by 50%
        damage += damage * 25 / 100; // increase damage by 25%


            if (currentLevel == 0)
            {
                currentMesh = upgrade1Mesh;
                defenderMaterial = upgrade1Material;
                this.GetComponentInChildren<Renderer>().material = defenderMaterial;
                currentLevel = 1;
            projectileUpgradeLevel = 1;
            Debug.Log("Upgrade Poison>");
            }

            else if (currentLevel == 1)
            {
                currentMesh = upgradeDoubleMesh;
                defenderMaterial = upgradeDoubleMaterial;
                this.GetComponentInChildren<Renderer>().material = defenderMaterial;
                currentLevel = 2;
                projectileUpgradeLevel = 3;

            }
            else if (currentLevel == 2)
            {
                Debug.Log("Defender already at max upgrade level.");
            }
    }
    public void Upgrade2()
    {
        health += startHealth * 50 / 100; // increase health by 50%
        damage += damage * 50 / 100; // increase damage by 50%

         if (currentLevel == 0)
            {
                currentMesh = upgrade2Mesh;
                defenderMaterial = upgrade2Material;
                this.GetComponentInChildren<Renderer>().material = defenderMaterial;
                currentLevel = 1;
                projectileUpgradeLevel = 2;
                Debug.Log("upgrade defenders");
            }
            else if (currentLevel == 1)
            {
                currentMesh = upgradeDoubleMesh;
                defenderMaterial = upgradeDoubleMaterial;
                this.GetComponentInChildren<Renderer>().material = defenderMaterial;
                currentLevel = 2;
                projectileUpgradeLevel = 3;
                
            }
            else if (currentLevel == 2)
            {
                Debug.Log("Defender already at max upgrade level.");
            }
        
    }

}//end class

/* BEEP BEEP IDEAAAA

    have a method in the defender placement class that uses raycasts when the player clicks to upgrade and let them choose which defender to upgrade 
    then get that defender and call this script attached to it, calling the upgrade method and passing in the upgrade type based on what the player selected
*/