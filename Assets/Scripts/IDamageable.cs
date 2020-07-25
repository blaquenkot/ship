public interface IDamageable : IEnemyDetector 
{
   bool TakeDamage(float damageTaken); // Returns if the object was destroyed
}