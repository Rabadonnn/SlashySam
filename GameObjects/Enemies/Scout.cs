using Microsoft.Xna.Framework;
using Spritesheet;

namespace SlashySam.GameObjects.Enemies
{
    public class Scout : Enemy
    {
        Animation walkRight;
        Animation walkLeft;
        Animation idleRight;
        Animation idleLeft;
        Animation currentAnimation;

        public int Range { get; set; } = 400;
        public bool Guarding { get; private set; } = true;
        Vector2 guardingPosition;

        public ScoutSword Sword { get; private set; }

        public Scout(Vector2 position)
        {
            Type = EnemyType.Scout;
            guardingPosition = position;
            Position = position;
            Scale = new Vector2(2.6f, 3f);
            sourceRectangle = new Rectangle(0, 0, 13, 16);
            var sheet = new Spritesheet.Spritesheet(Game.Assets.Enemies.Scout).WithGrid((13, 16));
            walkRight = sheet.CreateAnimation((0, 1), (1, 1));
            idleRight = sheet.CreateAnimation((0, 0), (1, 0), (2, 0));
            walkLeft = walkRight.FlipX();
            idleLeft = idleRight.FlipX();
            currentAnimation = walkRight;
            walkLeft.Start(Repeat.Mode.Loop);
            walkRight.Start(Repeat.Mode.Loop);
            idleLeft.Start(Repeat.Mode.Loop);
            idleRight.Start(Repeat.Mode.Loop);
            currentAnimation.Start(Repeat.Mode.Loop);

            Sword = new ScoutSword(this);

            Initialize();
        }

        int animDirection = 1;
        void UpdateAnimations()
        {
            if (direction.X > 0)
                animDirection = 1;
            else if (direction.X < 0)
                animDirection = -1;


            if (!idle)
            {
                if (animDirection == 1 && currentAnimation != walkRight)
                    currentAnimation = walkRight;
                else if (animDirection == -1 && currentAnimation != walkLeft)
                    currentAnimation = walkLeft;

                if (direction.Y != 0 && (currentAnimation != walkLeft || currentAnimation != walkRight))
                    if (animDirection == 1)
                        currentAnimation = walkRight;
                    else if (animDirection == -1)
                        currentAnimation = walkLeft;
            }
            else
            {
                if (animDirection == 1 && currentAnimation != idleRight)
                    currentAnimation = idleRight;
                else if (animDirection == -1 && currentAnimation != idleLeft)
                    currentAnimation = idleLeft;
            }

            currentAnimation.Update(Game.GameTime);
        }

        Vector2 direction;
        public Vector2 velocity = new Vector2();
        int speed = 100;
        bool idle = true;
        void UpdateMovement()
        {
            CheckCollision();

            if (Vector2.Distance(GameScreen.Player.Position, Position) < Range)
            {
                Guarding = false;
            }
            else
            {
                Guarding = true;
            }

            if (!Guarding)
            {
                direction = Vector2.Normalize(GameScreen.Player.Position - Position);
                idle = false;
            }
            else if (Guarding)
            {
                if (Vector2.Distance(Position, guardingPosition) > 10)
                {
                    direction = Vector2.Normalize(guardingPosition - Position);
                    idle = false;
                }
                else
                {
                    direction = Vector2.Zero;
                    idle = true;
                }
                Guarding = true;
            }

            velocity = direction * speed * Game.DeltaTime;
            Position += velocity;
        }

        protected override void UpdateMyself()
        {
            UpdateMovement();
            if (!Guarding)
            {
                Sword.Update();
            }
            UpdateAnimations();
            CheckHealth();
        }

        protected override void DrawMyself()
        {
            DrawHealthBar();
            if (!Guarding)
            {
                Sword.Draw();
            }
            Game.SpriteBatch.Draw(currentAnimation, Position, null, Rotation, Scale, 0);
        }
    }
}
