using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coleccionable : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si choca contra este objeto el jugador
        if (collision.gameObject.tag == "Player")
        {
            Destroy(gameObject);
        }
    }

}
