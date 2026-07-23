using UnityEngine;
using UnityEngine.InputSystem; // Obrigatório para o Input System

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))] // Garante que o Animator está no objeto
public class PlayerController2D : MonoBehaviour
{
    [Header("Mapeamento de Input (Inspector)")]
    [SerializeField] private InputActionReference acaoMover;
    [SerializeField] private InputActionReference acaoPular;

    [Header("Configurações de Movimento")]
    [SerializeField] private float velocidade = 8f;
    [SerializeField] private float forcaPulo = 12f;

    [Header("Verificação de Chão")]
    [SerializeField] private Transform checadorDeChao;
    [SerializeField] private float raioChecagem = 0.2f;
    [SerializeField] private LayerMask camadaChao;

    private Rigidbody2D rb;
    private Animator anim;
    private Vector2 moverInput;
    private bool estaNoChao;

    // Variável para controlar a direção atual do personagem
    private bool olhandoParaDireita = true;

    private void OnEnable()
    {
        if (acaoMover != null) acaoMover.action.Enable();

        if (acaoPular != null)
        {
            acaoPular.action.Enable();
            acaoPular.action.performed += AoPular;
        }
    }

    private void OnDisable()
    {
        if (acaoMover != null) acaoMover.action.Disable();

        if (acaoPular != null)
        {
            acaoPular.action.performed -= AoPular;
            acaoPular.action.Disable();
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        // Lê continuamente a direção do movimento (WASD / Setas / Analógico)
        if (acaoMover != null)
        {
            moverInput = acaoMover.action.ReadValue<Vector2>();
        }

        // Verifica a direção do movimento e vira o personagem se necessário
        ChecarDirecaoEVirar();

        // Atualiza as animações no Animator
        AtualizarAnimacoes();
    }

    private void FixedUpdate()
    {
        // Aplica o movimento na física
        rb.linearVelocity = new Vector2(moverInput.x * velocidade, rb.linearVelocity.y);

        // Verifica se está no chão
        if (checadorDeChao != null)
        {
            estaNoChao = Physics2D.OverlapCircle(checadorDeChao.position, raioChecagem, camadaChao);
        }
    }

    private void AoPular(InputAction.CallbackContext context)
    {
        if (estaNoChao)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, forcaPulo);

            // Dispara o Trigger usando a string do nome
            if (anim != null)
            {
                anim.SetTrigger("pular");
            }
        }
    }

    private void ChecarDirecaoEVirar()
    {
        // Se estiver movendo para a direita e olhando para a esquerda -> vira
        if (moverInput.x > 0 && !olhandoParaDireita)
        {
            Virar();
        }
        // Se estiver movendo para a esquerda e olhando para a direita -> vira
        else if (moverInput.x < 0 && olhandoParaDireita)
        {
            Virar();
        }
    }

    private void Virar()
    {
        // Inverte a flag
        olhandoParaDireita = !olhandoParaDireita;

        // Inverte a escala no eixo X
        Vector3 escala = transform.localScale;
        escala.x *= -1;
        transform.localScale = escala;
    }

    private void AtualizarAnimacoes()
    {
        if (anim == null) return;

        // Define a Bool "estaAndando" como verdadeira se houver input horizontal
        bool andando = Mathf.Abs(moverInput.x) > 0.01f;
        anim.SetBool("estaAndando", andando);

        // Define a Bool "estaNoChao" conforme a checagem de física
        anim.SetBool("estaNoChao", estaNoChao);
    }

    private void OnDrawGizmosSelected()
    {
        if (checadorDeChao != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(checadorDeChao.position, raioChecagem);
        }
    }
}