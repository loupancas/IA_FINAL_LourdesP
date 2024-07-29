using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    [SerializeField] protected int _vidaMax;
    [SerializeField] protected int _vida;
    public virtual void TakeDamage(int Damage)
    {
        _vida -= Damage;
        HealthBar healthBar = GetComponent<HealthBar>();
        healthBar.UpdateHPBar(_vida);

        if (_vida < 0.15*_vidaMax)
        {
            Huir();

            if (_vida < 0)
            {
                Morir();
            }
        }

       
    }

    public abstract void Morir();

    public abstract void Huir();


}