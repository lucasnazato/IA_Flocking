using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{
    // Variavel para o manager e o speed
    public FlockManager myManager;
    float speed;
    bool turning = false;

    private void Start()
    {
        // Definir a speed do peixe baseado nos valores minimos e maximos do manager
        speed = Random.Range(myManager.minSpeed, myManager.maxSpeed);
    }

    private void Update()
    {
        // Criar bounds para delimitar uma area
        Bounds b = new Bounds(myManager.transform.position, myManager.swinLimits * 2);

        // Criar um raycast e um vetor de direção do manager para o peixe
        RaycastHit hit = new RaycastHit();
        Vector3 direction = myManager.transform.position - transform.position;

        // Se o peixe não estiver dentro da area dos bounds
        if (!b.Contains(transform.position))
        {
            // Definir o turning como verdade e o vetor direction como a direção do manager para o peixe
            turning = true;
            direction = myManager.transform.position - transform.position;
        }
        // Se o peixe detectar um hit na sua frente
        else if (Physics.Raycast(transform.position, this.transform.forward * 50, out hit))
        {
            // Definir turning como verdadeiro e a direção como a inversa ao normal do hit
            turning = true;
            direction = Vector3.Reflect(this.transform.forward, hit.normal);
        }
        // 
        else
        {
            turning = false;
        }

        // Se truning for verdade definir a rotação como Quaternion.Slerp na direção da direction
        if (turning)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), myManager.rotationSpeed * Time.deltaTime);
        }
        // Se for falso, deixar a velocidade aleatoria e chamar o apply rules
        else
        {
            if (Random.Range(0, 100) < 10)
                speed = Random.Range(myManager.minSpeed, myManager.maxSpeed);
            if (Random.Range(0, 100) < 20)
                ApplyRules();
        }

        // Movimentar o peixe para frente
        transform.Translate(0, 0, Time.deltaTime * speed);
    }

    // 
    private void ApplyRules()
    {
        // Array de todos os peixes
        GameObject[] gos;
        gos = myManager.allFish;

        // Variaveis para calcular a rotação do peixe
        Vector3 vcentre = Vector3.zero;
        Vector3 avoid = Vector3.zero;
        float gSpeed = 0.01f;
        float nDistance;
        int groupSize = 0;

        // Para cada peixe presente no array...
        foreach (GameObject go in gos)
        {
            // Se o item atual do foreach não for este peixe
            if (go != this.gameObject)
            {
                // Calcular a distancia do item atual do for each para este peixe
                nDistance = Vector3.Distance(go.transform.position, this.transform.position);

                // Se a distancia entre eles for menor que a distancia definida no manager
                if (nDistance <= myManager.neighbourDistance)
                {
                    // Somar a posição do item ao valor do centro
                    vcentre += go.transform.position;
                    groupSize++;

                    // Se a distancia for menor que 1, calcular a distancia para evitar
                    if (nDistance < 1.0f)
                    {
                        avoid = avoid + (this.transform.position - go.transform.position);
                    }

                    // Pegar o componente flock para calcular a velocidade do grupo
                    Flock anotherFlock = go.GetComponent<Flock>();
                    gSpeed = gSpeed + anotherFlock.speed;
                }
            }
        }

        // Se o tamanho do grupo for maior que 0
        if (groupSize > 0)
        {
            // Calcular a posicao do valor do centro e a velocidade (Dar clamp na velocidade)
            vcentre = vcentre / groupSize + (myManager.goalPos - this.transform.position);
            speed = gSpeed / groupSize;
            speed = Mathf.Clamp(speed, myManager.minSpeed, myManager.maxSpeed);

            // Calcular a direção em que o peixe vai rotacionar
            Vector3 direction = (vcentre + avoid) - transform.position;
            
            // Se a direção não for igual a zero, definir a rotação do peixe
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), myManager.rotationSpeed * Time.deltaTime);
            }
        }
    }
}
