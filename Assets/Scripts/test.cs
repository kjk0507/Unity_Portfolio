using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    public float speed = 6.0f;
    public float gravity = -9.81f;

    private CharacterController controller;
    private Vector3 velocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        controller.Move(move * speed * Time.deltaTime);

        //if (!controller.isGrounded)
        //{
        //    velocity.y += gravity * Time.deltaTime;
        //}
        //else
        //{
        //    velocity.y = -2f; // 작은 값으로 설정하여 캐릭터가 땅에 붙어 있게 합니다.
        //}

        controller.Move(velocity * Time.deltaTime);
    }
}
