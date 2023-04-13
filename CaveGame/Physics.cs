using System;
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

    public void Update(GameTime gameTime, Physics[] testSubjects, int mapWidth)
    {
        // velocity needs to be calculated twice here as TestCollide changes Velocity
        TestCollide(testSubjects, Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds);
        
        // add pos to velocity
        Position += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        // keep inside map
        if (Position.X < 0f) Position.X = 0f;
        if (Position.X + Size.X > mapWidth) Position.X = mapWidth - Size.X;

        if (Gravity && !Grounded)
        {
            Velocity += new Vector2(0.0f,14f) * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

    }

    //linear interpolation, given 2 points you can find any point in between
    private static float Lerp(float v0, float v1, float t)
    {
        return (1 - t) * v0 + t * v1;
    }

    //tests if colliding with a point, if so, moves accordingly
    private void TestCollide(Physics[] testSubjects, Vector2 velocity)
    {
        Grounded = false;

        Vector2 p0 = Position + new Vector2(velocity.X, 0f);
        Vector2 p1 = p0 + Size;

        foreach (Physics testSubject in testSubjects)
        {
            Vector2 t0 = testSubject.Position;
            Vector2 t1 = testSubject.Position + testSubject.Size;

            // if player's rectangle within bounds of test subject's rectangle
            if (p1.X < t0.X || p0.X > t1.X || (p1.Y < t0.Y || p0.Y > t1.Y)) continue;
            
            if (Velocity.X > 0f) Velocity.X = 0;
            if (Velocity.X < 0f) Velocity.X = 0;
        }
        
        p0 = Position + new Vector2(0f, velocity.Y);
        p1 = p0 + Size;
        
        foreach (Physics testSubject in testSubjects)
        {
            Vector2 t0 = testSubject.Position;
            Vector2 t1 = testSubject.Position + testSubject.Size;

            // if player's rectangle within bounds of test subject's rectangle
            if (p1.X < t0.X || p0.X > t1.X || (p1.Y < t0.Y || p0.Y > t1.Y)) continue;

            if (Velocity.Y > 0f) {Velocity.Y = 0; Grounded = true; }
            if (Velocity.Y < 0f) Velocity.Y = 0;
        }

    }
}