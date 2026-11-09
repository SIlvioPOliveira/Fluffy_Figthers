using UnityEngine;

public sealed class CombatTargetTest : MonoBehaviour
{
    [SerializeField] private PetEquipmentTestDriver driver;
    [SerializeField, Min(0.1f)] private float maxDistance = 20f;
    private string message = "";
    private float messageUntil;
    public EnemyHealth Target { get; private set; }

    private void Update()
    {
        if (Target != null && (!Target.IsAlive || !Target.gameObject.activeInHierarchy || Vector3.Distance(driver.Player.position, Target.transform.position) > maxDistance)) ClearTarget();
        if (driver.InventoryOpen || driver.GachaOpen || driver.BarrierOpen || driver.PauseOpen || driver.VictoryOpen) return;
        if (Input.GetKeyDown(KeyCode.Q)) ClearTarget();
        if (!Input.GetMouseButtonDown(0) || Camera.main == null || PlayerHudPrototype.IsPointerOverInventoryButton()) return;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
        {
            EnemyHealth enemy = hit.collider.GetComponentInParent<EnemyHealth>();
            if (enemy != null) TrySelect(enemy);
        }
    }

    public bool TrySelect(EnemyHealth enemy)
    {
        if (enemy == null || !enemy.IsAlive || !enemy.gameObject.activeInHierarchy) return false;
        if (Vector3.Distance(driver.Player.position, enemy.transform.position) > maxDistance)
        {
            message = "Aproxime-se para atacar!";
            messageUntil = Time.time + 2f;
            return false;
        }
        if (Target == enemy) return true;
        if (Target != null) Target.SetSelected(false);
        Target = enemy;
        Target.SetSelected(true);
        return true;
    }

    public void ClearTarget()
    {
        if (Target != null) Target.SetSelected(false);
        Target = null;
    }

    private void OnGUI()
    {
        if (Time.time < messageUntil && !driver.InventoryOpen && !driver.GachaOpen) GUI.Label(new Rect(15, 75, 400, 25), message);
    }
}
