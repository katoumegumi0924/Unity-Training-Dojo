public interface IDamageable 
{
    //当前血量，只读
    int currentHealth{ get; }

    //受伤方法
    void TakeDamage(int damage);

    //死亡方法
    void Die();
}
