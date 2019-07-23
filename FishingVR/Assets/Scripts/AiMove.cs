﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiMove : MonoBehaviour
{
    private AiSpawner m_AiManager;

    private bool m_hasTarget = false;
    private bool m_isTurning;

    private Vector3 m_wayPoint;
    private Vector3 m_lastWaypoint = new Vector3(0f, 0f, 0f);

    private Animator m_animator;
    private float m_speed;

    private Collider m_collider;

    public string[] animals = new string[]
    {
        "anchovy",
        "catfish",
        "eel",
        "frog"
    };

    // Start is called before the first frame update
    void Start()
    {
        m_AiManager = transform.parent.GetComponentInParent<AiSpawner>();
        m_animator = GetComponent<Animator>();

        SetUpNPC();
    }

    void SetUpNPC()
    {
        float m_scale = Random.Range(0f, 4f);
        transform.localScale += new Vector3(m_scale * 1.5f, m_scale, m_scale);

        if(transform.GetComponent<Collider>() != null && transform.GetComponent<Collider>().enabled == true)
        {
            m_collider = transform.GetComponent<Collider>();
        }
        else if(transform.GetComponentInChildren<Collider>() != null && transform.GetComponentInChildren<Collider>().enabled == true)
        {
            m_collider = transform.GetComponentInChildren<Collider>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!m_hasTarget)
        {
            m_hasTarget = CanFindTarget();
        } else
        {
            RotateNPC(m_wayPoint, m_speed);
            transform.position = Vector3.MoveTowards(transform.position, m_wayPoint, m_speed * Time.deltaTime);

            ColliderNPC();
        }

        if(transform.position == m_wayPoint)
        {
            m_hasTarget = false;
        }
    }

    void ColliderNPC()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, transform.localScale.z))
        {
            if(hit.collider == m_collider | hit.collider.tag == "waypoint")
            {
                return;
            }

            int randomNum = Random.Range(1, 100);
            if(randomNum > 40)
            {
                m_hasTarget = false;
            }

            Debug.Log(hit.collider.transform.parent.name + " " + hit.collider.transform.parent.position);
        }
    }

    Vector3 GetWaypoint(bool isRandom)
    {
        if (isRandom)
        {
            return m_AiManager.RandomPosition();
        }
        else
        {
            return m_AiManager.RandomWaypoint();
        }
    }

    bool CanFindTarget(float start = 1f, float end = 7f)
    {
        m_wayPoint = m_AiManager.RandomWaypoint();
        if(m_lastWaypoint == m_wayPoint)
        {
            m_wayPoint = GetWaypoint(true);
            return false;
        } else
        {
            m_lastWaypoint = m_wayPoint;
            m_speed = Random.Range(start, end);
            m_animator.speed = m_speed;
            return true;
        }
    }

    void RotateNPC(Vector3 waypoint, float currentSpeed)
    {
        float TurnSpeed = currentSpeed * Random.Range(1f, 3f);

        Vector3 LookAt = waypoint - this.transform.position;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(LookAt), TurnSpeed * Time.deltaTime);
    }
}
