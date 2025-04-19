using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Rigidbody))]
public class PlayerBattle : MonoBehaviourPun
{
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody rb;
    private float inputX;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!photonView.IsMine)
            return;

        inputX = Input.GetAxis("Horizontal");
    }

    void FixedUpdate()
    {
        if (!photonView.IsMine)
            return;

        Vector3 move = new Vector3(inputX, 0, 0) * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + move);
    }
}
