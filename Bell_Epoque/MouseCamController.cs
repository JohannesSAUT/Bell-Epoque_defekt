using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Input;
using System;

namespace Bell_Epoque
{
    public class MouseCamController : SyncScript
    {
        /// <summary>
        /// Dieses Script benutzt die middlere Maustaste und das Mausrad zur Bewegungskontrolle,
        /// sowie die rechte Maustaste zum schwenken der Kammera.
        /// </summary>

        private const float MaximumPitch = MathUtil.PiOverTwo * 0.99f;

        private Vector3 upVector;
        private Vector3 translation;
        private float yaw;
        private float pitch;
        private String meldung = "Keine Taste gedrueckt";
        
        public Vector2 RotationSpeed { get; set; } = new Vector2(1.0f, 1.0f);
        public float SpeedFactor { get; set; } = 50.0f;
        public float ZoomFactor { get; set; } = 5.0f;
        //public CameraComponent MyCamera { get; init; }

        public override void Start()
        {
            base.Start();
            // Default up-direction
            upVector = Vector3.UnitY;
            translation = Vector3.UnitZ;
        }

        public override void Update()
        {
            translation = Vector3.Zero;
            yaw = 0f;
            pitch = 0f;
            Vector3 dir = Vector3.Zero;
            DebugText.Print(meldung, new Int2(20, 20));

            if (Input.HasMouse)
            {
                // Start Rotate with right Mouse Button
                if (Input.HasPressedMouseButtons && !Input.IsMouseButtonDown(MouseButton.Middle) && !Input.IsMouseButtonDown(MouseButton.Left))
                {
                    Input.LockMousePosition();
                    var centerPoint = "Hallo";
                    //var centerPoint = MyCamera.RaycastMouse(this);
                    meldung = "HitResult = " + centerPoint;
                }
                // Rotate with right Mouse Button
                if (Input.IsMouseButtonDown(MouseButton.Right) && !Input.IsMouseButtonDown(MouseButton.Middle))
                {
                    yaw -= Input.MouseDelta.X * RotationSpeed.X;
                    pitch -= Input.MouseDelta.Y * RotationSpeed.Y;
                }
                // Finish Rotate with right Mouse Button
                if (Input.HasReleasedMouseButtons && Input.IsMousePositionLocked)
                {
                    Input.UnlockMousePosition();
                    meldung = "Rechte Maustaste losgelassen";
                }


                // Move with mouse
                if (Input.IsMouseButtonDown(MouseButton.Middle) && !Input.IsMouseButtonDown(MouseButton.Right))
                {
                    dir.X -= Input.MouseDelta.X * SpeedFactor;
                    dir.Z += Input.MouseDelta.Y * SpeedFactor;
                }
                // Übersetzungsvariable für UpdateTransform() verwendet wird
                translation += dir;
            }
            // Get the local coordinate system
            var rotation = Matrix.RotationQuaternion(Entity.Transform.Rotation);

            // Enforce the global up-vector by adjusting the local x-axis
            var right = Vector3.Cross(rotation.Forward, upVector);
            var up = Vector3.Cross(right, rotation.Forward);

            // Stabilize
            right.Normalize();
            up.Normalize();

            // Neigungswinkel anpassen. Vermeiden Sie, dass es nach oben und unten zeigt. Randfälle stabilisieren.
            var currentPitch = MathUtil.PiOverTwo - MathF.Acos(Vector3.Dot(rotation.Forward, upVector));
            pitch = MathUtil.Clamp(currentPitch + pitch, -MaximumPitch, MaximumPitch) - currentPitch;

            Vector3 finalTranslation = translation;
            finalTranslation.Z = -finalTranslation.Z;
            finalTranslation = Vector3.TransformCoordinate(finalTranslation, rotation);

            // Move in local coordinates
            Entity.Transform.Position += finalTranslation;

            // Gieren um den globalen Aufwärtsvektor, Nicken und Rollen im lokalen Raum
            Entity.Transform.Rotation *= Quaternion.RotationAxis(right, pitch) * Quaternion.RotationAxis(upVector, yaw);
        }
    }
}