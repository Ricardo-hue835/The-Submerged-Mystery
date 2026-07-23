using UnityEngine;
using UnityEngine.InputSystem; // Obrigatório para o Input System

[RequireComponent(typeof(Rigidbody2D))]
public class NadarPlayer : MonoBehaviour
{
    [Header("Mapeamento de Input (Inspector)")]
    [SerializeField] private InputActionReference acaoMovimento;

    [Header("Configurações de Movimento")]
    [SerializeField] private float velocidadeMovimento = 8f;

    [Header("Componentes de Animação")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private string nomeParametroAnimacao = "IsWalking"; // Nome da variável Bool no Animator

    private Rigidbody2D rb2d;
    private Vector2 direcaoMovimento;

    private void OnEnable()
    {
        if (acaoMovimento != null) acaoMovimento.action.Enable();
    }

    private void OnDisable()
    {
        if (acaoMovimento != null) acaoMovimento.action.Disable();
    }

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();

        // Busca automática dos componentes caso não sejam arrastados no Inspector
        if (animator == null) animator = GetComponent<Animator>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        // Lê a direção nos eixos X (Horizontal) e Y (Vertical)
        if (acaoMovimento != null)
        {
            direcaoMovimento = acaoMovimento.action.ReadValue<Vector2>();
        }

        AtualizarAnimacaoEViragem();
    }

    private void FixedUpdate()
    {
        // Aplica a velocidade em ambas as direções simultaneamente
        rb2d.linearVelocity = direcaoMovimento * velocidadeMovimento;
    }

    private void AtualizarAnimacaoEViragem()
    {
        // Checa se há movimento apenas nas laterais (eixo X)
        bool estaAndandoLados = Mathf.Abs(direcaoMovimento.x) > 0.1f;

        // Atualiza a animação usando SetBool
        if (animator != null)
        {
            animator.SetBool(nomeParametroAnimacao, estaAndandoLados);
        }

        // Vira o Sprite dependendo da direção
        if (spriteRenderer != null)
        {
            if (direcaoMovimento.x > 0.1f)
            {
                spriteRenderer.flipX = false; // Olhando para a Direita
            }
            else if (direcaoMovimento.x < -0.1f)
            {
                spriteRenderer.flipX = true; // Olhando para a Esquerda
            }
        }
    }
}