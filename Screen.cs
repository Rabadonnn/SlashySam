namespace SlashySam
{
    public abstract class Screen
    {
        public abstract string ID
        {
            get;
        }

        public abstract void Load();
        public abstract void Update();
        public abstract void Draw();
        public virtual void OnEnter()
        {

        }
        public virtual void OnExit()
        {

        }
    }
}
