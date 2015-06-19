namespace SampleGame
{
    public abstract class GameObject
    {
        public string Name { get; set; }

        public override string ToString()
        {
            return Name ?? base.ToString();
        }
    }

    public class PlayerGameObject : GameObject 
    {
        public int Health { get; set; }
    }

    public class EnemyGameObject : GameObject 
    {
        public int Damage { get; set; }
    }
}
