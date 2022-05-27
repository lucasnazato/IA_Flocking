using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{
    // Variavel para o manager e o speed
    public FlockManager myManager;
    float speed;

    private void Start()
    {
        // Definir a speed do peixe baseado nos valores minimos e maximos do manager
        speed = Random.Range(myManager.minSpeed, myManager.maxSpeed);
    }

    private void Update()
    {
        // Movimentar o peixe para frente
        transform.Translate(0, 0, Time.deltaTime * speed);
    }
}
