using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

//classe � uma defini��o de um objeto do nosso jogo
public class Player : MonoBehaviour
{
    [Header("Attributes")]
    public float speed; //velocidade do player
    public float jumpForce; //for�a do pulo
    public int life; //vida
    public int apple; //qtd de ma�as

    [Header("Components")]
    public Rigidbody2D rig; //referencia ao rigidbody2d (componente de fisica)
    public Animator anim; //referencia ao animator
    public SpriteRenderer sprite; //referencia ao spriteRenderer

    [Header("UI")]
    public TextMeshProUGUI appleText; //referencia ao texto da qtd de ma�as
    public TextMeshProUGUI lifeText; //texto qtd de vidas
    public GameObject gameOver; //objeto do game over

    private Vector2 direction; //armazenamos o input do teclado/gamepad
    private bool isGrounded; //se estiver no chao, fica verdadeiro
    private bool recovery; //se estiver piscando nao pode tomar outro dano

    public static Player instance; //geramos um acessador estatico para o player

    private void Awake()
    {
        //o trecho abaixo verifica se j� existe um objeto do player na cena. Caso sim, destroi a copia.
        //caso nao, chama o dondestroyonload para o objeto ser mantido na cena
        //isso evitara duplicacoes do objeto do player
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(instance.gameObject);
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        lifeText.text = life.ToString(); //ao iniciar, o texto da vida recebe o total de vida inicial
        Time.timeScale = 1; //resetamos o tempo do jogo para 1
    }

    // Update is called once per frame
    void Update()
    {
        direction = new Vector2(Input.GetAxisRaw("Horizontal"),Input.GetAxisRaw("Vertical")); //captura input

        Jump(); //chama metodo jump a cada frame
        PlayAnim(); //chama metodo animacao a cada frame
    }

    //� usado para f�sica
    void FixedUpdate()
    {
        Movement();
    }

    //andar
    void Movement()
    {
        rig.velocity = new Vector2(direction.x * speed, rig.velocity.y);
    }

    //pular
    void Jump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded == true)
        {
            anim.SetInteger("transition", 2);
            rig.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            isGrounded = false;
        }
        
    }

    //morrer
    void Death()
    {
        if(life <= 0)
        {
            gameOver.SetActive(true);
            Time.timeScale = 0;
        }
    }

    //animacoes
    void PlayAnim()
    {
        if(direction.x > 0)
        {
            if(isGrounded == true)
            {
                anim.SetInteger("transition", 1);
            }
            
            transform.eulerAngles = Vector2.zero;
        }

        if(direction.x < 0)
        {
            if(isGrounded == true)
            {
                anim.SetInteger("transition", 1);
            }
           
            transform.eulerAngles = new Vector2(0, 180);
        }

        if(direction.x == 0)
        {
            if(isGrounded == true)
            {
                anim.SetInteger("transition", 0);
            }
            
        }
    }

    //toma hit
    public void Hit()
    {       
        if(recovery == false)
        {
            StartCoroutine(Flick());
        }     
    }

    //faz piscar
    IEnumerator Flick()
    {
        recovery = true;
        life--;
        Death();
        lifeText.text = life.ToString();
        sprite.color = new Color(1, 1, 1, 0);
        yield return new WaitForSeconds(0.2f);
        sprite.color = new Color(1, 1, 1, 1);
        yield return new WaitForSeconds(0.2f);
        sprite.color = new Color(1, 1, 1, 0);
        yield return new WaitForSeconds(0.2f);
        sprite.color = new Color(1, 1, 1, 1);        
        recovery = false;
    }

    //aumenta score
    public void IncreaseScore()
    {
        apple++;
        appleText.text = apple.ToString();
    }

    //reinicia game
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    //checa colisao do player com outros objetos
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //se colidir com o chao
        if(collision.gameObject.layer == 6)
        {
            isGrounded = true;
        }
    }
}
