using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    [SerializeField] protected int _vidaMax;
    [SerializeField] protected int _vida;
    MeshRenderer _meshRenderer;
    Color _color;
    public int Vida
    {
        get { return _vida; }
        set { _vida = value; }
    }
    public virtual void TakeDamage(int Damage)
    {
        _vida -= Damage;
        HealthBar healthBar = GetComponent<HealthBar>();
        if (healthBar != null)
        {
           healthBar.UpdateHPBar(_vida, _vidaMax);
            
        }
        else
        {
            Debug.LogError("HealthBar component not found on the entity.");
        }

        if (_vida < 0)
        {
            Morir();
        }



    }

    public abstract void Morir();
    
   


}