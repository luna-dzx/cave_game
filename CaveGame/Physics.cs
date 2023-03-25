using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Threading;

namespace ThisIsTheActualProjectIPromise
{
    public class Physics
    {
        public Vector2 Velocity;
        public Vector2 Position;
        public Vector2 Size;
        
        public bool Grounded;
        public bool Gravity;

        public Physics(Vector2 pos, Vector2 size)
        {
            Size = size;
            Velocity = new Vector2(0, 0);
            Position = pos;

            Grounded = false;
            Gravity = false;
        }

        public void Update(GameTime gameTime, Physics[] testSubjects, Vector2 testRes)
        {
            TestCollide(testSubjects, testRes);
            
            //add pos to velocity
            Position += Velocity* (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (Gravity && !Grounded)
            {
                Velocity += new Vector2(0.0f,9.8f) * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

        }

        //linear interpolation, given 2 points you can find any point inbetween
        private float lerp(float v0, float v1, float t)
        {
            return (1 - t) * v0 + t * v1;
        }

        //tests if colliding with a point, if so, moves accordingly
        public void TestCollide(Physics[] testSubjects, Vector2 testRes)
        {
            int topCollisions = 0;
            int bottomCollisions = 0;
            int leftCollisions = 0;
            int rightCollisions = 0;

            //x value testing sweep
            for (int testStep = 0; testStep < (int) testRes.X; testStep++)
            {
                //interpolate num of points between pos and opp pos using res
                float xTestValue = lerp(Position.X, Position.X + Size.X, (float)testStep / testRes.X);
                foreach(Physics testSubject in testSubjects)
                {
                    if (testSubject.Contains(new Vector2 (xTestValue,Position.Y)))
                    {
                        topCollisions++;
                    }
                    
                    if (testSubject.Contains(new Vector2(xTestValue, Position.Y + Size.Y)))
                    {
                        bottomCollisions++;
                    }
                }
            }


            //y value (morbius) sweep 
            for (int testStep = 0; testStep < (int)testRes.Y; testStep++)
            {
                //interpolate num of points between pos and opp pos using res
                float yTestValue = lerp(Position.Y, Position.Y + Size.Y, (float)testStep / testRes.Y);
                foreach (Physics testSubject in testSubjects)
                {
                    if (testSubject.Contains(new Vector2(Position.X, yTestValue)))
                    {
                        leftCollisions++;
                    }

                    if (testSubject.Contains(new Vector2(Position.X + Size.X, yTestValue)))
                    {
                        rightCollisions++;
                    }
                }
            }
            if (Velocity.X < 0.0 && leftCollisions > 1)
            {
                Velocity.X = 0;
            }
            if (Velocity.X > 0.0 && rightCollisions > 1)
            {
                Velocity.X = 0;
            }
            if (Velocity.Y < 0.0 && topCollisions > 1)
            {
                Velocity.Y = 0;
            }
            if (Velocity.Y > 0.0 && bottomCollisions > 1)
            {
                Velocity.Y = 0;
            }
        }

        //checks if point is inside the bounds of the physics object
        public bool Contains(Vector2 pos)
        {
            Vector2 opposite = Position + Size;
            return (pos.X > Position.X && pos.X < opposite.X) && (pos.Y > Position.Y && pos.Y < opposite.Y);
        }
    }
}
