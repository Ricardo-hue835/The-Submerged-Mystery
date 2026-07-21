using UnityEngine;
using UnityEngine.InputSystem; // Obrigatório para o Input System

[RequireComponent(typeof(Rigidbody2D))]
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
    private Vector2 moverInput;
    private bool estaNoChao;

    // Ativa a escuta das ações quando o objeto entra em cena
    private void OnEnable()
    {
        if (acaoMover != null) acaoMover.action.Enable();

        if (acaoPular != null)
        {
            acaoPular.action.Enable();
            // Registra o evento de clique no botão de pulo
            acaoPular.action.performed += AoPular;
        }
    }

    // Desativa a escuta quando o objeto é desativado/destruído (evita vazamento de memória)
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
    }

    private void Update()
    {
        // Lê continuamente a direção do movimento (WASD / Setas / Analógico)
        if (acaoMover != null)
        {
            moverInput = acaoMover.action.ReadValue<Vector2>();
        }
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

    // Função disparada no exato momento em que o botão de pulo é pressionado
    private void AoPular(InputAction.CallbackContext context)
    {
        if (estaNoChao)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, forcaPulo);
        }
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