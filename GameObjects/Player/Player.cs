using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Linq;
using System.Collections.Generic;

using Spritesheet;

using SlashySam.Particles;

namespace SlashySam.GameObjects.Player
{
    public class Player : GameObject
    {
        public PlayerSword Sword { get; private set; }
        enum Anim
        {
            IdleRight,
            IdleLeft,

            WalkRight,
            WalkLeft
        };
        Animation currentAnim;
        Dictionary<Anim, Animation> animations = new Dictionary<Anim, Animation>();

        Animation headAnimRight;
        Animation headAnimLeft;
        Animation currentHeadAnim;

        ParticleSystem dashEffect;
        ParticleSystem smokeBombEffect;
        ParticleSystemSettings dashEffectSettings;
        ParticleSystemSettings smokeBombEffectSettings;

        Vector2 smokeBombSize = new Vector2(125);

        public int MaxHealth { get; } = 100;
        int maxEnergy = 100;

        int initialShurikens = 10;
        public int CurrentShurikens { get; set; }
        int initialSmokeBombs = 3;
        public int CurrentSmokeBombs { get; set; }

        public int Health { get; set; }
        public int Energy { get; set; }

        public Player()
        {
            Sword = new PlayerSword(this);

            Texture = Game.Assets.Player.Sam01;
            Scale = new Vector2(1.7f);
            Game.Camera.Position = Position;
            sourceRectangle = new Rectangle(0, 0, 16, 29);

            Spritesheet.Spritesheet sheet = new Spritesheet.Spritesheet(Texture).WithGrid((16, 29));

            animations.Add(Anim.IdleRight, sheet.CreateAnimation((0, 0), (1, 0), (2, 0)));
            animations.Add(Anim.WalkRight, sheet.CreateAnimation((0, 1), (1, 1)));

            animations[Anim.IdleLeft] = animations[Anim.IdleRight].FlipX();
            animations[Anim.WalkLeft] = animations[Anim.WalkRight].FlipX();

            animations[Anim.IdleRight].Start(Repeat.Mode.Loop);
            animations[Anim.IdleLeft].Start(Repeat.Mode.Loop);

            animations[Anim.WalkRight].Start(Repeat.Mode.Loop);
            animations[Anim.WalkLeft].Start(Repeat.Mode.Loop);

            currentAnim = animations[Anim.IdleRight];

            Spritesheet.Spritesheet headSheet = new Spritesheet.Spritesheet(Game.Assets.Player.SamHead).WithGrid((18, 14));

            headAnimRight = headSheet.CreateAnimation((0, 0), (1, 0));
            headAnimLeft = headAnimRight.FlipX();

            headAnimRight.Start(Repeat.Mode.Loop);
            headAnimLeft.Start(Repeat.Mode.Loop);

            currentHeadAnim = headAnimLeft;

            var dashSize = Helper.SquareTexture.CreateScale(Rectangle.Width, Rectangle.Height);
            dashEffectSettings = new ParticleSystemSettings()
            {
                Density = 1f,
                Lifetime = new Size(0.2f, 0.5f),
                Size = new Size(dashSize.X, dashSize.Y),
                Speed = new Size(0, 0),
                Texture = Helper.SquareTexture,
                OneTime = false,
                DecreaseSize = true,
                DecreaseAlpha = false,
                Color = Color.Black
            };
            smokeBombEffectSettings = new ParticleSystemSettings()
            {
                Density = 0.3f,
                Lifetime = new Size(0.7f, 1.6f),
                Size = new Size(3f, 6f),
                Speed = new Size(100, 200),
                Texture = Game.Assets.PixelatedCircle,
                OneTime = false,
                DecreaseSize = true,
                DecreaseAlpha = true,
                Colors = new List<Color>()
                {
                    new Color(38, 38, 38),
                    new Color(69, 69, 69)
                },
                SpawnInCircle = false,
                CircleRadius = (int)smokeBombSize.X,
            };

            dashEffect = new ParticleSystem(Position, new Vector2(1, 1), dashEffectSettings);
            smokeBombEffect = new ParticleSystem(Position, smokeBombSize, smokeBombEffectSettings);

            HasCollider = true;
        }

        public void Spawn(Vector2 position)
        {
            Position = position;

            Health = MaxHealth;
            Energy = maxEnergy;

            CurrentShurikens = initialShurikens;
            CurrentSmokeBombs = initialSmokeBombs;

            Dead = false;
        }

        float speed = 230;
        Vector2 direction = new Vector2();
        Vector2 velocity;

        void CheckCollisions()
        {
            foreach (var obs in GameScreen.CurrentLevel.Objects)
            {
                bool hasCollider = obs.HasCollider;

                if (hasCollider && Rectangle.Intersects(obs.Rectangle))
                {
                    var collision = Helper.RectangleCollision(Rectangle, obs.Rectangle);
                    if (collision == Helper.Collision.Top)
                    {
                        Position = new Vector2(Position.X, obs.Rectangle.Bottom + Rectangle.Height / 2);
                    }
                    if (collision == Helper.Collision.Bottom)
                    {
                        Position = new Vector2(Position.X, obs.Rectangle.Top - Rectangle.Height / 2);
                    }
                    if (collision == Helper.Collision.Right)
                    {
                        Position = new Vector2(obs.Rectangle.Left - Rectangle.Width / 2, Position.Y);
                    }
                    if (collision == Helper.Collision.Left)
                    {
                        Position = new Vector2(obs.Rectangle.Right + Rectangle.Width / 2, Position.Y);
                    }

                    if (dashing)
                    {
                        dashing = false;
                    }
                }
            }
        }

        bool spaceKeyReleased = true;
        void PerformMovement()
        {
            // Changes the direction according to the keys that are currently pressed
            var keys = Keyboard.GetState().GetPressedKeys();

            if ((smokeBombAvailable && keys.Contains(Keys.F) && Energy > smokeBombCost && CurrentSmokeBombs > 0) || smoking)
            {
                SmokeBomb();
            }

            if (Keyboard.GetState().IsKeyUp(Keys.Space))
            {
                spaceKeyReleased = true;
            }

            if ((dashAvailable && keys.Contains(Keys.Space) && Energy > dashCost && spaceKeyReleased) || dashing)
            {
                Dash();
                spaceKeyReleased = false;
            }
            else
            {
                if (keys.Contains(Keys.W))
                {
                    direction.Y = -1;
                }
                if (keys.Contains(Keys.S))
                {
                    direction.Y = 1;
                }
                if (!keys.Contains(Keys.W) && !keys.Contains(Keys.S))
                {
                    direction.Y = 0;
                }

                if (keys.Contains(Keys.A))
                {
                    direction.X = -1;
                }
                if (keys.Contains(Keys.D))
                {
                    direction.X = 1;
                }
                if (!keys.Contains(Keys.A) && !keys.Contains(Keys.D))
                {
                    direction.X = 0;
                }
            }

            // normalize the direction if both it's components are not 0
            if (direction.X != 0 && direction.Y != 0)
            {
                direction.Normalize();
            }
            // Update position based on velocity
            velocity = direction * speed;
            Position += velocity * Game.DeltaTime;

            CheckCollisions();

            // Make the camera smoothly follow the player
            if (!dashing)
            {
                Game.Camera.Position = Vector2.Lerp(Game.Camera.Position, Position, camSpeed * Game.DeltaTime);
            }

            if (!dashAvailable)
            {
                c_dashCooldown -= Game.DeltaTime;
                if (c_dashCooldown < 0)
                {
                    dashAvailable = true;
                    c_dashCooldown = dashCooldown;
                }
            }

            if (dashing && dashEffect.Enabled == false)
            {
                dashEffect.Enabled = true;
            }
            else if (!dashing && dashEffect.Enabled == true)
            {
                dashEffect.Enabled = false;
            }
        }
        float camSpeed = 10f;

        float dashCooldown = 0.75f;
        float c_dashCooldown = 0;
        bool dashAvailable = true;
        bool dashing = false;
        Vector2 dashTarget = new Vector2();
        int dashLength = 150;
        float dashSpeed = 30f;
        void Dash()
        {
            if (!dashing)
            {
                Energy -= dashCost;
                var mouseInWorld = Helper.MouseInWorld;
                var dir = Vector2.Normalize(new Vector2(mouseInWorld.X - Position.X, mouseInWorld.Y - Position.Y));
                float a = MathF.Atan2(dir.Y, dir.X);
                dashTarget.X = Position.X + MathF.Cos(a) * dashLength;
                dashTarget.Y = Position.Y + MathF.Sin(a) * dashLength;
                dashing = true;
                dashAvailable = false;
                direction = dir;
            }
            else
            {
                Position = Vector2.Lerp(Position, dashTarget, dashSpeed * Game.DeltaTime);

                if (Vector2.Distance(Position, dashTarget) < 10)
                {
                    dashing = false;
                }
            }
        }

        float smokeBombDuration = 5;
        float smokeBombCooldown = 1;
        bool smokeBombAvailable = true;
        bool smoking = false;
        public bool InSmoke
        {
            get
            {
                return Vector2.Distance(smokeBombEffect.Position, Position) < smokeBombSize.X;
            }
        }

        void SmokeBomb()
        {
            if (!smoking)
            {
                Energy -= smokeBombCost;
                smoking = true;
                smokeBombAvailable = false;
                smokeBombEffect.Position = Position;
                smokeBombCooldown = smokeBombDuration;
                smokeBombEffect.Enabled = true;
                CurrentSmokeBombs--;
            }
            else
            {
                smokeBombEffect.Update();
                smokeBombCooldown -= Game.DeltaTime;
                if (smokeBombCooldown < 0)
                {
                    smoking = false;
                    smokeBombAvailable = true;
                    smokeBombEffect.Enabled = false;
                    smokeBombEffect.Particles.Clear();
                }
            }
        }

        public enum Weapon
        {
            Sword,
            Shuriken
        }

        public Weapon CurrentWeapon
        {
            get;
            private set;
        } = Weapon.Sword;

        public int ShurikenDamage => 20;
        public int SwordDamage => 15;

        public List<Shuriken> Shurikens { get; } = new List<Shuriken>();
        bool shurikenAvailable = false;
        Vector2 shurikenPos;

        public int swordAttackCost = 7;
        int shurikenCost = 13;
        int dashCost = 30;
        int smokeBombCost = 25;

        public void ApplySwordHitCost()
        {
            Energy -= swordAttackCost;
        }

        void UpdateWeapons()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.D1) && CurrentWeapon != Weapon.Sword)
            {
                CurrentWeapon = Weapon.Sword;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.D2) && CurrentWeapon != Weapon.Shuriken)
            {
                CurrentWeapon = Weapon.Shuriken;
            }

            if (CurrentWeapon == Weapon.Sword)
            {
                Sword.Update();
            }
            else if (CurrentWeapon == Weapon.Shuriken)
            {
                var mouseInWorld = Helper.MouseInWorld;

                var dir = Vector2.Normalize(new Vector2(mouseInWorld.X - Position.X, mouseInWorld.Y - Position.Y));
                float a = MathF.Atan2(dir.Y, dir.X);
                int radius = 40;
                int x = (int)(Position.X + MathF.Cos(a) * radius);
                int y = (int)(Position.Y + MathF.Sin(a) * radius);
                shurikenPos = new Vector2(x, y);
                var mouse = Mouse.GetState();
                if (mouse.LeftButton == ButtonState.Pressed && GameScreen.ActualScreenRectangle.Includes(mouse.Position) && shurikenAvailable && Energy > shurikenCost && CurrentShurikens > 0)
                {
                    Energy -= shurikenCost;
                    Shurikens.Add(new Shuriken(shurikenPos, dir));
                    shurikenAvailable = false;
                    CurrentShurikens--;
                }
                if (Mouse.GetState().LeftButton == ButtonState.Released)
                {
                    shurikenAvailable = true;
                }
                shurikenRotation += (MathF.PI / 180) * Game.DeltaTime * 400;
            }

            foreach (var s in Shurikens.ToArray())
            {
                if (s.Dead)
                {
                    Shurikens.Remove(s);
                }
                else
                {
                    s.Update();
                }
            }

            if (Energy < maxEnergy)
            {
                if (energyRestoreCD < 0)
                {
                    Energy += 3;
                    energyRestoreCD = 0.1f;
                    if (Energy > maxEnergy)
                    {
                        Energy = maxEnergy;
                    }
                }
                else
                {
                    energyRestoreCD -= Game.DeltaTime;
                }
            }
        }
        float energyRestoreCD = 0;

        int animDirection = 1;
        void UpdateAnimations()
        {
            if (direction.X > 0)
                animDirection = 1;
            else if (direction.X < 0)
                animDirection = -1;

            if (direction.X == 0)
            {
                if (animDirection == 1 && currentAnim != animations[Anim.IdleRight])
                    currentAnim = animations[Anim.IdleRight];
                else if (animDirection == -1 && currentAnim != animations[Anim.IdleLeft])
                    currentAnim = animations[Anim.IdleLeft];
            }
            else
            {
                if (animDirection == 1 && currentAnim != animations[Anim.WalkRight])
                    currentAnim = animations[Anim.WalkRight];
                else if (animDirection == -1 && currentAnim != animations[Anim.WalkLeft])
                    currentAnim = animations[Anim.WalkLeft];
            }

            if (direction.Y != 0 && (currentAnim != animations[Anim.WalkRight] || currentAnim != animations[Anim.WalkLeft]))
                if (animDirection == 1)
                    currentAnim = animations[Anim.WalkRight];
                else if (animDirection == -1)
                    currentAnim = animations[Anim.WalkLeft];

            currentAnim.Update(Game.GameTime);

            if (animDirection == 1 && currentHeadAnim != headAnimRight)
                currentHeadAnim = headAnimRight;
            else if (animDirection == -1 && currentHeadAnim != headAnimLeft)
                currentHeadAnim = headAnimLeft;

            if (direction == Vector2.Zero && headAnimRight.IsStarted)
                currentHeadAnim.Pause();
            else if (direction != Vector2.Zero && !headAnimRight.IsStarted)
                currentHeadAnim.Resume();

            currentHeadAnim.Update(Game.GameTime);
        }

        protected override void UpdateMyself()
        {
            PerformMovement();
            dashEffect.Position = Position;
            dashEffect.Update();
            UpdateWeapons();
            UpdateAnimations();
            if (Health <= 0)
            {
                Dead = true;
            }
        }

        float shurikenRotation = 0;
        void DrawWeapons()
        {
            if (CurrentWeapon == Weapon.Sword)
            {
                Sword.Draw();
            }
            else if (CurrentWeapon == Weapon.Shuriken)
            {
                var texture = Game.Assets.Player.Shuriken;
                var origin = new Vector2(texture.Width / 2);
                var scale = new Vector2(1.7f);
                if (shurikenRotation > Math.PI / 2)
                    shurikenRotation = 0;
                Game.SpriteBatch.Draw(texture, shurikenPos, null, Color.White, shurikenRotation, origin, scale, SpriteEffects.None, 0);
            }

            foreach (var shuriken in Shurikens)
            {
                shuriken.Draw();
            }
        }

        protected override void DrawMyself()
        {
            dashEffect.Draw();
            Game.SpriteBatch.Draw(currentAnim, Position, null, Rotation, Scale, 0);
            DrawWeapons();
            smokeBombEffect.Draw();
        }

        public void DrawHUD()
        {
            DrawHead();
            DrawBars();
            DrawConsumables();
        }

        void DrawHead()
        {
            float headScale = 4;
            var headWith = (Game.Assets.Player.SamHead.Width / 2) * headScale;
            Game.SpriteBatch.Draw(currentHeadAnim, new Vector2(headWith, GameScreen.BlackBarHeight / 2), null, 0, new Vector2(headScale), 0);
        }


        Rectangle barsRect;
        void DrawBars()
        {
            int x = 140;
            int h = 20;
            int healthBarY = GameScreen.BlackBarHeight / 2 - h;
            int width = 200;
            if (barsRect.IsEmpty)
            {
                barsRect = new Rectangle(x - 2, healthBarY - 2, width + 4, h * 2 + 6);
            }
            int healthBarWidthWidth = (int)Helper.MapValue(Health, MaxHealth, 0, width, 0);

            var healthBarRect = new Rectangle(x, healthBarY, healthBarWidthWidth, h);

            int energyBarWidth = (int)Helper.MapValue(Energy, maxEnergy, 0, width, 0);
            int energyBarY = healthBarY + h + 2;
            var energyBarRect = new Rectangle(x, energyBarY, energyBarWidth, h);

            Game.SpriteBatch.Draw(Helper.SquareTexture, barsRect, Color.Violet);
            
            Game.SpriteBatch.Draw(Helper.SquareTexture, healthBarRect, Color.Green);
            Game.SpriteBatch.Draw(Helper.SquareTexture, energyBarRect, Color.Purple);
        }

        void DrawConsumables()
        {
            var shurikenContainer = new Rectangle(barsRect.Right + 30, barsRect.Y, (int)(barsRect.Width * 0.65f), barsRect.Height);
            var shurikenRect = new Rectangle(shurikenContainer.X + 2, barsRect.Y + 2, barsRect.Height - 4, barsRect.Height - 4);
            var shurikenTextPos = new Vector2(shurikenRect.Right, shurikenRect.Y - shurikenRect.Height / 3);

            var smokeBombContainer = new Rectangle(shurikenContainer.Right + 30, shurikenContainer.Y, shurikenContainer.Width, shurikenContainer.Height);
            var smokeBombRect = new Rectangle(smokeBombContainer.X + 2, barsRect.Y + 2, barsRect.Height - 4, barsRect.Height - 4);
            var smokeBombTextPos = new Vector2(smokeBombRect.Right, smokeBombRect.Y - smokeBombRect.Height / 3);

            Game.SpriteBatch.Draw(Helper.SquareTexture, shurikenContainer, Color.Violet);
            Game.SpriteBatch.Draw(Game.Assets.Player.Shuriken, shurikenRect, Color.White);
            Game.SpriteBatch.DrawString(Game.Assets.ThaleahFont, ":" + CurrentShurikens.ToString(), shurikenTextPos, new Color(26, 26, 26), 0, new Vector2(), 0.5f, SpriteEffects.None, 0);

            Game.SpriteBatch.Draw(Helper.SquareTexture, smokeBombContainer, Color.Violet);
            Game.SpriteBatch.Draw(Game.Assets.Player.SmokeBombIcon, smokeBombRect, Color.White);
            Game.SpriteBatch.DrawString(Game.Assets.ThaleahFont, ":" + CurrentSmokeBombs.ToString(), smokeBombTextPos, new Color(26, 26, 26), 0, new Vector2(), 0.5f, SpriteEffects.None, 0);

            var enemiesLeftRect = new Rectangle(smokeBombContainer.Right + 30, smokeBombContainer.Y, 350, smokeBombContainer.Height);
            Game.SpriteBatch.Draw(Helper.SquareTexture, enemiesLeftRect, Color.Violet);
            Game.SpriteBatch.DrawString(Game.Assets.ThaleahFont, "Enemies left: " + GameScreen.CurrentLevel.Enemies.Count.ToString(), new Color(26, 26, 26), enemiesLeftRect);
        }
    }
}
