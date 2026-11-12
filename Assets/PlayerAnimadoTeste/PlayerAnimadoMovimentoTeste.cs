using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public sealed class PlayerAnimadoMovimentoTeste : MonoBehaviour
{
    [SerializeField] private Transform cameraBase;
    [SerializeField] private Animator animator;
    [SerializeField] private float velocidade = 2.2f;
    [SerializeField] private float gravidade = -20f;
    private CharacterController controller;
    private float velocidadeVertical;
    private string estadoAtual;

    private void Awake() { controller = GetComponent<CharacterController>(); }

    private void Update()
    {
        Vector2 input = Vector2.ClampMagnitude(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")), 1f);
        if (cameraBase != null)
        {
            Vector3 frente = Vector3.ProjectOnPlane(cameraBase.forward, Vector3.up).normalized;
            if (frente.sqrMagnitude > 0.001f) transform.rotation = Quaternion.LookRotation(frente);
        }
        Vector3 direcao = transform.right * input.x + transform.forward * input.y;
        if (controller.isGrounded && velocidadeVertical < 0) velocidadeVertical = -2f;
        velocidadeVertical += gravidade * Time.deltaTime;
        controller.Move((direcao * velocidade + Vector3.up * velocidadeVertical) * Time.deltaTime);
        string estado = "Idle";
        if (input.sqrMagnitude > 0.01f)
            estado = Mathf.Abs(input.y) >= Mathf.Abs(input.x) ? (input.y > 0 ? "Frente" : "Tras") : (input.x > 0 ? "Direita" : "Esquerda");
        if (animator != null && estado != estadoAtual)
        {
            animator.CrossFadeInFixedTime(estado, 0.12f);
            estadoAtual = estado;
        }
    }
}
