using UnityEngine;

public class DisableOnEntityDeath : MonoBehaviour
{
    [SerializeField] private Entity entity;
    [SerializeField] private Mode mode = Mode.Disable;

    private enum Mode
    {
        Disable,
        Destroy
    }

    private void Awake()
    {
        switch (mode)
        {
            case Mode.Disable:
                entity.OnDeath += () => this.gameObject.SetActive(false);
                break;
            case Mode.Destroy:
                entity.OnDeath += () => Destroy(this.gameObject);
                break;
        }
    }
}
