using UnityEngine;

namespace Z3.Utils
{
    public static class MathUtils
    {
        /// <summary>
        /// Forces angle to always be between 0 and 360 degrees range
        /// </summary>
        /// <param name="degrees">Angle in Degrees</param>
        /// <returns>Normalized Angle in Degrees</returns>
        public static float NormalizeAngle(float degrees)
        {
            return (degrees + 360f) % 360f;
        }

        public static Vector2 GetCirclePosition(float angle, float radius)
        {
            return GetCirclePosition(Vector2.down, Vector2.zero, angle, radius);
        }

        public static Vector2 GetCirclePosition(Vector2 direction, Vector2 center, float angle, float radius)
        {
            Quaternion offset = Quaternion.AngleAxis(angle, Vector3.forward);
            return center + (Vector2)(offset * (direction * radius));
        }

        public static float Rebase(float value, Vector2 rangeInput, Vector2 rangeOutput)
        {
            float t = Mathf.InverseLerp(rangeInput.x, rangeInput.y, value);
            return Mathf.Lerp(rangeOutput.x, rangeOutput.y, t);
        }

        /// <param name="multiplier">Distance or Velocity</param>
        public static Vector2 AngleToDirection(float angle, float multiplier = 1)
        {
            float degrees = angle * Mathf.Deg2Rad;
            float x = Mathf.Cos(degrees);
            float y = Mathf.Sin(degrees);
            return new Vector2(x, y) * multiplier;
        }

        public static float DirectionToAngle(Vector3 direction) => Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        public static float AngleDiference(float currentAngle, float targetAngle)
        {
            float phi = Mathf.Abs(targetAngle - currentAngle) % 360;
            float distance = phi > 180 ? 360 - phi : phi;
            return distance;
        }

        /// <summary>
        /// Angle based from center to target
        /// </summary>
        /// <returns>Return a value between -180 and 180</returns>
        public static float TargetAngle(Vector3 center, Vector3 target)
        {
            Vector3 lookDir = target - center;
            return DirectionToAngle(lookDir);
        }

        /// <summary>
        /// Angle based from center to target
        /// </summary>
        /// <returns>Return a value between 0 and 360</returns>
        public static float TargetAngle360(Vector3 center, Vector3 target)
        {
            Vector2 lookDir = target - center;
            float angle = DirectionToAngle(lookDir);
            return NormalizeAngle(angle);
        }

        public static float Cosecant(float angle)
        {
            float cosecant = 1 / Mathf.Cos(angle * Mathf.Deg2Rad);
            return Mathf.Abs(cosecant);
        }

        public static float Secant(float angle)
        {
            float secant = 1 / Mathf.Sin(angle * Mathf.Deg2Rad);
            return Mathf.Abs(secant);
        }

        public static float Hypotenuse(float sideALength, float sideBLength)
        {
            return Mathf.Sqrt(sideALength * sideALength + sideBLength * sideBLength);
        }

        #region Oblique Throw
        /// <summary>
        /// V0y = (Yp - Yi + g * TotalTime² / 2) / TotalTime
        /// </summary> 
        public static Vector3 ObliqueThrowX(Vector2 targetDistance, float mass, float xSpeed)
        {
            float timeTotal = Mathf.Abs(targetDistance.x) / xSpeed;
            //float speedY = (targetDistance.y + rigidbody2D.gravityScale * timeTotal * timeTotal * 5) / timeTotal;
            float speedY = (targetDistance.y + mass * -Physics.gravity.y * timeTotal * timeTotal / 2) / timeTotal;
            float speedX = targetDistance.x > 0 ? xSpeed : -xSpeed;
            return new Vector2(speedX, speedY);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetDistance">Final position - start potion</param>
        /// <param name="ySpeedLimit">Min and max</param>
        public static Vector2 ObliqueThrowX(Vector2 targetDistance, float gravity, float xSpeed, Vector2 ySpeedLimit)
        {
            Vector2 velocity = ObliqueThrowX(targetDistance, gravity, xSpeed);

            if (velocity.y < ySpeedLimit.x)
            {
                velocity.y = ySpeedLimit.x;
            }
            else if (velocity.y > ySpeedLimit.y)
            {
                velocity.y = ySpeedLimit.y;
            }

            return velocity;
        }

        /// <summary>
        /// Yp = Yi + V0y * Ttotal - g * Ttotal² / 2 
        /// </summary>
        public static Vector2 ObliqueThrowTime(Vector2 targetDistance, float gravity, float duration)
        {
            float velocityX = targetDistance.x / duration;
            float velocityY = targetDistance.y + gravity * -Physics2D.gravity.y / 2 * duration;
            return new Vector2(velocityX, velocityY);
        }
        #endregion


        public static Vector3 ClosestPointInRadius(Vector3 fromPoint, Vector3 targetPoint, float radiusDistance)
        {
            Vector3 direction = fromPoint - targetPoint;

            // Check if is inside of radius
            float distance = direction.magnitude;
            if (distance <= radiusDistance)
            {
                return fromPoint;
            }

            return targetPoint + direction.normalized * radiusDistance;
        }
    }
}