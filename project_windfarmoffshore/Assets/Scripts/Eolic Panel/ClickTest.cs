using UnityEngine;

public class ClickTest : MonoBehaviour
{
    void Update()
    {
        // Verifica se o usu�rio clicou com o bot�o esquerdo do mouse
        if (Input.GetMouseButtonDown(0)) // 0 = clique esquerdo
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // Cria um raio a partir da posi��o do mouse na tela

            if (Physics.Raycast(ray, out hit)) // Verifica se o raio atingiu algo
            {
                // Se o objeto atingido for este objeto (o que tem esse script), imprime uma mensagem
                if (hit.collider.gameObject == gameObject)
                {
                    Debug.Log("Objeto clicado: " + gameObject.name); // Imprime o nome do objeto no console
                }
            }
        }
    }
}
