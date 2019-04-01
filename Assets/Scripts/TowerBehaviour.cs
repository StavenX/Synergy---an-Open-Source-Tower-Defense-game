﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBehaviour : MonoBehaviour
{
    public GameObject bulletPrefab;
    private GameObject bullet;
    private int counter;
    private LinkedList<GameObject> enemies;


    // The target marker.
    public Transform target;

    // Angular speed in radians per sec.
    float speed;
    float rotationAmount;

    int towerWaitingPeriod;
    float towerRange;

    // Start is called before the first frame update
    void Start()
    {
        counter = 0;

        //how long the tower has to wait between each shot
        towerWaitingPeriod = 30;

        //how far the tower can shoot
        towerRange = 5.0f;


        enemies = new LinkedList<GameObject>();

                
        //high speed means towers instantly turns to target, lower speed means they turn slowly
        //(if using Time.deltaTime * speed)
        //speed = 200.0f;

        //amount a tower rotates towards target every frame
        //1.0 = 100%, 0.5 = 50% etc.
        rotationAmount = 1.0f;

        //sets rotation target to a an enemy
        target = getNewTarget();
    }
    
    // Update is called once per frame
    void Update()
    {
        this.enemies = getEnemies();

        counter++;

        //reduces amount of work per update to reduce strain on computer
        if (counter % towerWaitingPeriod * Time.deltaTime == 0)
        {
            //rotates towers 90 degrees
            //transform.Rotate(Vector3.forward * -90);

            if (target != null)
            { 
                //gets new target and rotates if not in range
                if (!isInRange(target))
                {
                    target = getNewTarget();
                    rotateToTarget();
                }
                //rotates and shoots if in range of target
                else
                {
                    rotateToTarget();
                    shootBullet();
                }               
            } else
            {
                target = getNewTarget();
            }
        }
    }    

    private bool isInRange(Transform t)
    {
        return Vector3.Distance(transform.position, t.position) <= towerRange;
    }

    /**
     * Returns the enemy that is closest to their endgame
     * Returns null if no targets are available for current tower
     */
    private Transform getNewTarget()
    {
        try
        {
            //temp values, will always be overwritten unless enemies is empty
            //in which case function ends up returning null
            int furthestWaypoint = -1;
            Transform priorityEnemy = null;

            foreach (GameObject enemy in enemies)
            {
                if (isInRange(enemy.transform))
                {
                    int enemyWaypoint = enemy.GetComponent<EnemyBehaviour>().CurrentWaypoint;

                    //sets new priorityenemy if they have reached a waypoint further along the map
                    if (enemyWaypoint > furthestWaypoint)
                    {
                        priorityEnemy = enemy.transform;
                        furthestWaypoint = enemyWaypoint;
                    }
                }                
            }
            return priorityEnemy;
        }
        catch (System.NullReferenceException)
        {
            return null;
        }
        catch (System.Exception)
        {
            return null;
        }
    }

    private void rotateToTarget()
    {
        if (target != null)
        {
            Vector3 vectorToTarget = target.position - transform.position;
            float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg - 180;
            Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);

            //Slerp or RotateTowards methods  
            transform.rotation = Quaternion.Slerp(transform.rotation, q, rotationAmount);
        }
    }

    /*
     * Fires a bullet when a tower is clicked
     * Debug use mostly
     * */
    private void OnMouseUp()
    {
        shootBullet();
        
    }

    /**
     * Tower shoots a bullet
     * */
    void shootBullet()
    {
        //use v as second constructor parameter for bullet to place bullet at v
        //probably remove this
        //Vector3 v = new Vector3(transform.position.x, transform.position.y);

        //new bullet spawns on top of tower
        bullet = (GameObject)
                Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        bullet.GetComponent<BulletBehaviour>().target = this.target;
    }


    private LinkedList<GameObject> getEnemies()
    {
        LinkedList<GameObject> list = new LinkedList<GameObject>();
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            list.AddLast(enemy);
        }
        return list;
    }

}
