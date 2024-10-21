namespace AeroliteSharpEngine.Interfaces
{
    public interface IBody2D : IPhysicsObject
    {
        float Restitution { get; set; }
        float Friction { get; set; }
        float Angle { get; set; }
        float AngularVelocity { get; set; }
        float AngularAcceleration { get; set; }
        float Inertia { get; }
        float InverseInertia { get; }

        void ApplyTorque(float torque);
    }
}
