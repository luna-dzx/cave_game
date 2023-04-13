using Microsoft.Xna.Framework;

namespace CaveGame;

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
        TestCollide(testSubjects, testRes, Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds);
            
        //add pos to velocity
        Position += Velocity* (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (Gravity && !Grounded)
        {
            Velocity += new Vector2(0.0f,9.8f) * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

    }

    //linear interpolation, given 2 points you can find any point in between
    private static float Lerp(float v0, float v1, float t)
    {
        return (1 - t) * v0 + t * v1;
    }

    //tests if colliding with a point, if so, moves accordingly
    private void TestCollide(Physics[] testSubjects, Vector2 testRes, Vector2 velocity)
    {
        int topCollisions = 0;
        int bottomCollisions = 0;
        int leftCollisions = 0;
        int rightCollisions = 0;


        Vector2 position = Position + velocity;

        //x value testing sweep
        for (int testStep = 0; testStep < (int) testRes.X; testStep++)
        {
            //interpolate num of points between pos and opp pos using res
            float xTestValue = Lerp(position.X, position.X + Size.X, (float)testStep / testRes.X);
            foreach(Physics testSubject in testSubjects)
            {
                if (testSubject.Contains(new Vector2 (xTestValue, position.Y)))
                {
                    topCollisions++;
                }
                    
                if (testSubject.Contains(new Vector2(xTestValue, position.Y + Size.Y)))
                {
                    bottomCollisions++;
                }
            }
        }


        //y value (morbius) sweep 
        for (int testStep = 0; testStep < (int)testRes.Y; testStep++)
        {
            //interpolate num of points between pos and opp pos using res
            float yTestValue = Lerp(position.Y, position.Y + Size.Y, (float)testStep / testRes.Y);
            foreach (Physics testSubject in testSubjects)
            {
                if (testSubject.Contains(new Vector2(position.X, yTestValue)))
                {
                    leftCollisions++;
                }

                if (testSubject.Contains(new Vector2(position.X + Size.X, yTestValue)))
                {
                    rightCollisions++;
                }
            }
        }

        if (Velocity.X < 0.0 && leftCollisions > 0)
        {
            Velocity.X = 0;
        }
        if (Velocity.X > 0.0 && rightCollisions > 0)
        {
            Velocity.X = 0;
        }
        if (Velocity.Y < 0.0 && topCollisions > 0)
        {
            Velocity.Y = 0;
        }
        if (Velocity.Y > 0.0 && bottomCollisions > 0)
        {
            Velocity.Y = 0;
        }
    }

    //checks if point is inside the bounds of the physics object
    private bool Contains(Vector2 pos)
    {
        Vector2 opposite = Position + Size;
        return (pos.X > Position.X && pos.X < opposite.X) && (pos.Y > Position.Y && pos.Y < opposite.Y);
    }
}