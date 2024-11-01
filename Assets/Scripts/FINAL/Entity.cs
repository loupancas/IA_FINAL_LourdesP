using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    [SerializeField] protected int _vidaMax;
    [SerializeField] protected int _vida;

    public int Vida
    {
        get { return _vida; }
        set { _vida = value; }
    }
    public virtual void TakeDamage(int Damage)
    {
        _vida -= Damage;
        HealthBar healthBar = GetComponent<HealthBar>();
        healthBar.UpdateHPBar(_vida);

      
            if (_vida < 0)
            {
                Morir();
            }
        

       
    }

    public abstract void Morir();
    
   


}