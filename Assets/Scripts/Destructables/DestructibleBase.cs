namespace GGJ.Destructibles
{
    public class DestructibleBase : HealthBase
    {

        protected override void Kill()
        {
            Destroy(gameObject);
            
            //TODO Add SFX & VFX upon destroying
        }

    }
}
