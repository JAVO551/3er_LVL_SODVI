using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

public class TouchCode : MonoBehaviour
{
    //Dirección
        //Derecha
    public bool dir = false;
    //Componentes
    Rigidbody2D myRigidbody2D;
    Transform myTransform;
    CapsuleCollider2D myCollider2D;
    [SerializeField] GameObject player;
    //Variables inciales
    private float gravedad = 1f;
    [SerializeField] float moveSpeedR = 10f;
    [SerializeField] float moveSpeedL = -10f;
    //Input
    private PlayerInput playerInput;
    private InputAction touchPositionAction;
    private InputAction touchPressAction;
    [SerializeField] Vector2 moveInput;
    private Transform currentLocation;
    //Run
    private bool runStart = true;
    private bool Undamaged = true;
    private float reboteEnemigo = -10f;
    [SerializeField] private float bounceTimeEnemy = 1f;
    private bool coroutineEnemy = false;
    //Jump
    [SerializeField] float jumpHeight = 10f;
    //Slide
    private bool onSlideCoroutineEnemy = false;
    [SerializeField] private float slideTime = 1f;
    void Start()
    {   
        //Dir inicio default 
        dir = false;
        //Obtener instancias
        Debug.Log(dir);
        myRigidbody2D = GetComponent<Rigidbody2D>();
        myCollider2D = GetComponent<CapsuleCollider2D>();
        myTransform = GetComponent<Transform>();
        playerInput = GetComponent<PlayerInput>();
        touchPressAction = playerInput.actions.FindAction("TouchPress");
        touchPositionAction = playerInput.actions.FindAction("TouchPosition");
        currentLocation  = GetComponent<Transform>();
        Debug.Log(myRigidbody2D.velocity);
    }
    // Update is called once per frame
    void Update()
    {
        if (dir == true)
        {
           
            //Correr derecha
            if (coroutineEnemy == false)
            {
                RunRight();
            }
            //Slide derecha
            if (onSlideCoroutineEnemy == false)
            {
                SlideRight();
            }
            //Saltar derecha
            JumpRight();
            Debug.Log(myRigidbody2D.velocity);
        }
        //izquierda
        else if(dir == false)
        {
           
            //Correr izq
            if (coroutineEnemy == false)
            {
                RunLeft();
            }
            //Slide izq
            if (onSlideCoroutineEnemy == false)
            {
                SlideLeft();
            }
            //Saltar izq
            JumpLeft();
            Debug.Log(myRigidbody2D.velocity);
        }
        
    }

 
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Choca contra enemigo
        if (collision.gameObject.tag == "Enemy")
        {
            StartCoroutine(EnemyAttack());
            Destroy(collision.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            Debug.Log("Adios");
            Destroy(collision.gameObject);
        }
    }

    //Coroutines
    //Contra enemigos
    IEnumerator EnemyAttack()
    {
        coroutineEnemy = true;
        Debug.Log("Ouch!!");
        //Si va a la derecha
        if(dir == true)
        {
            myRigidbody2D.AddForce(new Vector2(reboteEnemigo, 0), ForceMode2D.Impulse);
        }
        //Si va a la izquierda
        if (dir == false)
        {
            myRigidbody2D.AddForce(new Vector2(-1 * reboteEnemigo, 0), ForceMode2D.Impulse);
        }
        //Se ejecuta el resto del código en lo que sale, no se queda esperando, solo vuelve a terminar lo que venga después una vez termine el tiempo
        yield return new WaitForSeconds(bounceTimeEnemy);
        coroutineEnemy = false;
    }
        //Slide
    IEnumerator EnemyAvoid()
    {
        onSlideCoroutineEnemy = true;
        Debug.Log("Shuush!!");
        //Quita gravedad y vuelve al collider trigger para que detecte si toca el collider del enemigo
        myRigidbody2D.gravityScale = 0f;
        myCollider2D.isTrigger = true;
        //myCollider2D.enabled = false;
        yield return new WaitForSeconds(slideTime);
        myRigidbody2D.gravityScale = gravedad;
        myCollider2D.isTrigger = false;
        //myCollider2D.enabled = true;
        onSlideCoroutineEnemy = false;
        Debug.Log("Fin corrutina");
    }

    //Movement functions
    void RunRight()
    {
        //Candando, checa solo 1 vez si se ha tocado el lado derecho de la pantalla
        if (runStart)
        {
            //Verifica si se tocó la pantalla
            if (touchPressAction.WasPressedThisFrame())
            {
                //Compara con la posición de la cámara si se tocó el lado derecho
                Vector3 position = Camera.main.ScreenToWorldPoint(touchPositionAction.ReadValue<Vector2>());
                if (position.x > Camera.main.transform.position.x)
                {
                    //Abre candado
                    Debug.Log("Derecha una vez");
                    runStart = false;
                    myRigidbody2D.velocity = new Vector3(moveSpeedR, myRigidbody2D.velocity.y);
                }

            }
        }
        
        else
        //Movimiento constante
        {
            if (Undamaged)
            {
                Debug.Log("Permanente Derecha");
                myRigidbody2D.velocity = new Vector3(moveSpeedR, myRigidbody2D.velocity.y);
            }          
        }   
    }
    //Notas RunRight:
    // Como la variable del if para movimiento permanente es la misma en los dos casos, no tiene que dar click de nuevo al cambiar de modo izquierda a derecha.
    void RunLeft()
    {
        //Candando, checa solo 1 vez si se ha tocado el lado derecho de la pantalla
        if (runStart)
        {
            //Verifica si se tocó la pantalla
            if (touchPressAction.WasPressedThisFrame())
            {
                //Compara con la posición de la cámara si se tocó el lado derecho
                Vector3 position = Camera.main.ScreenToWorldPoint(touchPositionAction.ReadValue<Vector2>());
                if (position.x < Camera.main.transform.position.x)
                {
                    //Abre candado
                    Debug.Log("Izquierda una vez");
                    runStart = false;
                    myRigidbody2D.velocity = new Vector3(moveSpeedL, myRigidbody2D.velocity.y);
                }

            }
        }

        else
        //Movimiento constante
        {
            if (Undamaged)
            {
                Debug.Log("Permanente Izquierda");
                myRigidbody2D.velocity = new Vector3(moveSpeedL, myRigidbody2D.velocity.y);
            }
        }
    }

    void JumpRight()
    {
        if (touchPressAction.WasPressedThisFrame())
        {
            Vector3 position = Camera.main.ScreenToWorldPoint(touchPositionAction.ReadValue<Vector2>());
            if (position.x < Camera.main.transform.position.x && position.y > Camera.main.transform.position.y)
            {
                Debug.Log("JumpR");
                myRigidbody2D.velocity = new Vector3(myRigidbody2D.velocity.x, myRigidbody2D.velocity.y + jumpHeight);
            }
            
        }
    }
    void JumpLeft()
    {
        if (touchPressAction.WasPressedThisFrame())
        {
            Vector3 position = Camera.main.ScreenToWorldPoint(touchPositionAction.ReadValue<Vector2>());
            if (position.x > Camera.main.transform.position.x && position.y > Camera.main.transform.position.y)
            {
                Debug.Log("JumpL");
                myRigidbody2D.velocity = new Vector3(myRigidbody2D.velocity.x, myRigidbody2D.velocity.y + jumpHeight);
            }

        }
    }

    void SlideRight()
    {
        if (touchPressAction.WasPressedThisFrame())
        {
            Vector3 position = Camera.main.ScreenToWorldPoint(touchPositionAction.ReadValue<Vector2>());
            if (position.x < Camera.main.transform.position.x && position.y < Camera.main.transform.position.y)
            {
                StartCoroutine("EnemyAvoid");
                Debug.Log("Slide");
            }

        }
    }

    void SlideLeft()
    {
        if (touchPressAction.WasPressedThisFrame())
        {
            Vector3 position = Camera.main.ScreenToWorldPoint(touchPositionAction.ReadValue<Vector2>());
            if (position.x > Camera.main.transform.position.x && position.y < Camera.main.transform.position.y)
            {
                StartCoroutine("EnemyAvoid");
                Debug.Log("Slide");
            }

        }
    }



}
