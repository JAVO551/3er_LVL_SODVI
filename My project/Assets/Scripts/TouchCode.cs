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
    [SerializeField] float moveSpeedR = 10f;
    [SerializeField] float moveSpeedL = -10f;
    //Input
    private PlayerInput playerInput;
    private InputAction touchPositionAction;
    private InputAction touchPressAction;
    //[SerializeField] Vector2 moveInput;
    //Run
    private bool runStart = true;
    private bool Undamaged = true;
    [SerializeField] float reboteEnemigo = -10f;
    [SerializeField] private float bounceTimeEnemy = 1f;
    public bool coroutineEnemy = false;
    //Jump
    [SerializeField] float jumpHeight = 10f;
    //Slide
    [SerializeField] float slideCooldown = 2f;
    private float lastSlide;
    private float currentSlideTime;
    private bool onSlideCoroutineEnemy = false;
    [SerializeField] private float slideTime = 1f;
    //Animación
    [SerializeField] Animator animator;
    void Start()
    {   
        //Dir inicio default 
        dir = true;
        lastSlide = Time.time;

        //Obtener instancias
        Debug.Log(dir);
        myRigidbody2D = GetComponent<Rigidbody2D>();
        myCollider2D = GetComponent<CapsuleCollider2D>();
        myTransform = GetComponent<Transform>();
        playerInput = GetComponent<PlayerInput>();
        touchPressAction = playerInput.actions.FindAction("TouchPress");
        touchPositionAction = playerInput.actions.FindAction("TouchPosition");
        //Debug.Log(myRigidbody2D.velocity);
    }
    // Update is called once per frame
    void Update()
    {

        //Espera a que el jugador elija dirección para empezar 
        //if jugador toco pantalla izq escojer dir = 0, else lo otro
        //Ade´más prender bander
        //If bandera prendida, empezar movimiento
        //Inicia las condiciones de movimiento
        //Prende el animator que esta por dault apagado
        empezarMov();



    }

    void empezarMov()
    {
        //dir decide la dirección-> true: derecha, false: izquierda
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
            //Saltar derecha solo cuando este tocando el piso
            if (myCollider2D.IsTouchingLayers(LayerMask.GetMask("Ground")))
            {
                JumpRight();
            }

        }
        //izquierda
        else if (dir == false)
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
            //Saltar izquierda solo cuando este tocando el piso
            if (myCollider2D.IsTouchingLayers(LayerMask.GetMask("Ground")))
            {
                JumpLeft();
            }
            //Debug.Log(myRigidbody2D.velocity);
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
                //Debug.Log("Permanente Derecha");
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
                //Debug.Log("Permanente Izquierda");
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
                //Iniciamos animación de salto

                Debug.Log("JumpR");
                myRigidbody2D.velocity = new Vector3(myRigidbody2D.velocity.x, myRigidbody2D.velocity.y + jumpHeight);
                //Iniciamos animación de caida
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


    IEnumerator EnemyAvoid()
    {
        float Area = 50f;
        //Evita que se vuelva a entrar a la rutina mientras este en ejecución
        onSlideCoroutineEnemy = true;
        Debug.Log("Shuush!!");

        //Obtiene un arreglo con los colliders2D dentro un area
        Collider2D[] colliders = Physics2D.OverlapCircleAll(myTransform.position,Area);

        //Recorremos el arreglo hasta encontrar el que este asignado a un objeto con la etiqueta Enemy
        //y desactiva su componente collider2D
        foreach (Collider2D objeto in colliders)
        {          
            if (objeto.CompareTag("Enemy"))
            {
                Debug.Log("Enemigo encontrado");
                objeto.GetComponent<Collider2D>().enabled = false;
            }
            
        }

        Debug.Log("Inicio Invulnerabilidad");
        //En este tiempo puede regresa a la invocación de la rutina, y ejecuta lo que seguía hasta que termine el tiepo
        //Cuando termine el tiempo, retoma justo después del yield
        yield return new WaitForSeconds(slideTime);
        //Recorremos el arreglo anterior hasta encontrar el que este asignado a un objeto con la etiqueta Enemy
        //y lo volvemos le activamos su compoenente collider2D
        foreach (Collider2D objeto in colliders)
        {         
            if (objeto.CompareTag("Enemy"))
            {
                objeto.GetComponent<Collider2D>().enabled = true;
            }
        }
        onSlideCoroutineEnemy = false;
        Debug.Log("Fin Invulnerabilidad");
    }

    void SlideRight()
    {
        if (touchPressAction.WasPressedThisFrame())
        {
            // Validar que la posición que presionó de la patalla corresponde al del Slide (Abajo a la izquierda)
            Vector3 position = Camera.main.ScreenToWorldPoint(touchPositionAction.ReadValue<Vector2>());
            if (position.x < Camera.main.transform.position.x && position.y < Camera.main.transform.position.y)
            {
                //Checamos el cooldown de la abilidad, si aún falta tiempo, no hace nada
                if (Time.time - lastSlide < slideCooldown)
                {
                    Debug.Log("abilidad en cooldown"+ (Time.time - lastSlide));
                    return;
                }
                //Inicia rutina              
                lastSlide = Time.time;
                Debug.Log("inicia rutina Slide derecha");
                StartCoroutine("EnemyAvoid");
                
              
            }

        }
    }

    void SlideLeft()
    {
        // Validar que la posición que presionó de la patalla corresponde al del Slide (Abajo a la derecha)
        if (touchPressAction.WasPressedThisFrame())
        {
            Vector3 position = Camera.main.ScreenToWorldPoint(touchPositionAction.ReadValue<Vector2>());
            if (position.x > Camera.main.transform.position.x && position.y < Camera.main.transform.position.y)
            {
                //Checamos el cooldown de la abilidad, si aún falta tiempo, no hace nada
                if (Time.time - lastSlide < slideCooldown)
                {
                    Debug.Log("abilidad en cooldown" + (Time.time - lastSlide));
                    return;
                }
                //Inicia rutina              
                lastSlide = Time.time;
                Debug.Log("inicia rutina Slide derecha");
                StartCoroutine("EnemyAvoid");
            }

        }
    }

    



}
