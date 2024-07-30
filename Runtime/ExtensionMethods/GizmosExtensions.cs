using UnityEngine;

namespace Z3.Utils.ExtensionMethods
{
    public static class GizmosExtensions
    {
        public static void DrawLineSecant(this Transform transform, float angle, float distance)
        {
            Vector2 center = transform.position;

            float secant = MathUtils.Secant(angle);
            Vector2 to = MathUtils.AngleToDirection(angle, secant * distance);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(center, center + to);
        }

        public static void DrawPointsAlongRadius(this Transform transform, float openingAngle, float distance, int count, Color color, float sphereRadius = .25f)
        {
            Vector2 center = transform.position;

            float angle = transform.eulerAngles.z - openingAngle / 2;
            float angleStep = openingAngle / (count - 1);

            Gizmos.color = color;
            for (int i = 0; i < count; i++)
            {

                Vector2 offset = MathUtils.AngleToDirection(angle, distance);
                Gizmos.DrawWireSphere(center + offset, sphereRadius);
                angle += angleStep;
            }
        }

        public static void SequenceCenteredPoints(this Transform transform, Vector3 direction, float spacing, int count, Color color, float gizmosRadius = .1f)
        {
            SequenceCenteredPoints(transform.position, direction, spacing, count, color, gizmosRadius);
        }

        public static void SequenceCenteredPoints(Vector3 pivotPosition, Vector3 direction, float spacing, int count, Color color, float gizmosRadius = .1f)
        {
            direction = direction.normalized;

            Gizmos.color = color;
            Vector3 position = pivotPosition - direction * (spacing * (count - 1) / 2);
            for (int i = 0; i < count; i++)
            {

                Vector3 offset = direction * (spacing * i);
                Gizmos.DrawWireSphere(position + offset, gizmosRadius);
            }
        }

        public static void SequencePoints(this Transform transform, Vector2 direction, float distance, float spacing, int count, Color color)
        {
            direction = direction.normalized;

            Gizmos.color = color;
            Vector3 position = transform.position;
            for (int i = 0; i < count; i++)
            {

                Vector3 offset = direction * (distance + spacing * i);
                Gizmos.DrawWireSphere(position + offset, .2f);
            }
        }

        public static void MirroredSequencePoints(this Transform transform, Vector2 direction, float distance, float spacing, int count, Color color)
        {
            direction = direction.normalized;

            Gizmos.color = color;
            Vector3 position = transform.position;
            for (int i = 0; i < count; i++)
            {

                Vector3 offset = direction * (distance + spacing * i);
                Gizmos.DrawWireSphere(position + offset, .2f);
                Gizmos.DrawWireSphere(position - offset, .2f);
            }
        }

        public static void DrawDistanceLine(this Transform transform, float distance)
        {
            transform.DrawDistanceLine(distance, Color.cyan);
        }

        public static void DrawDistanceLine(this Transform transform, float distance, Color firstLineColor)
        {
            Vector2 pointA = transform.position;
            Vector2 pointB = new Vector2(pointA.x + distance, pointA.y);

            Gizmos.color = firstLineColor;
            Gizmos.DrawLine(pointA, pointB);
            DrawVerticalLine(pointB);
        }

        public static void DrawLineBetween(this Transform transform, Vector2 avarageDistance)
        {
            transform.DrawLineBetween(avarageDistance, Color.cyan);
        }

        public static void DrawLineBetween(this Transform transform, Vector2 avarageDistance, Color color)
        {
            Vector2 pointA = new Vector2()
            {
                x = transform.position.x + avarageDistance.x,
                y = transform.position.y
            };
            Vector2 pointB = new Vector2()
            {
                x = transform.position.x + avarageDistance.y,
                y = transform.position.y
            };

            Gizmos.color = color;
            Gizmos.DrawLine(pointA, pointB);
            DrawVerticalLine(pointA);
            DrawVerticalLine(pointB);
        }

        public static void DrawAverageLine(this Transform transform, Vector2 avarageDistance)
        {
            transform.DrawAverageLine(avarageDistance, Color.cyan, Color.green);
        }

        public static void DrawAverageLine(this Transform transform, Vector2 avarageDistance, Color firstLineColor, Color secondLineColor)
        {
            Vector2 pointA = transform.position;
            Vector2 pointB = new Vector2(pointA.x + avarageDistance.x, pointA.y);
            Vector2 pointC = new Vector2(pointA.x + avarageDistance.y, pointA.y);

            Gizmos.color = firstLineColor;
            Gizmos.DrawLine(pointA, pointB);
            DrawVerticalLine(pointB);

            Gizmos.color = secondLineColor;
            Gizmos.DrawLine(pointB, pointC);
            DrawVerticalLine(pointC);
        }

        public static void DrawVerticalLine(Vector2 point, float size = 1f)
        {
            float half = size / 2f;
            Gizmos.DrawLine(new Vector2(point.x, point.y + half), new Vector2(point.x, point.y - half)); // Vertical
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transform">Center</param>
        /// <param name="radius">Area</param>
        /// <param name="initialAngle">Cos angle</param>
        /// <param name="count">Points around center</param>
        public static void DrawPointsAroundRadius(this Transform transform, float radius, float initialAngle, int count, Color color, float pointRadius = .25f)
        {
            Vector2 center = transform.position;

            float angleStep = 360f / count;
            float currentAngle = initialAngle;
            Gizmos.color = color;

            Gizmos.DrawWireSphere(center, radius);
            for (int i = 0; i < count; i++)
            {

                Vector2 point = MathUtils.AngleToDirection(currentAngle, radius);
                Gizmos.DrawWireSphere(center + point, pointRadius);

                currentAngle += angleStep;
            }
        }

        /// <summary>
        /// Draws a biquadratic parabola, for projectile trajectories.
        /// </summary>
        /// <param name="yRotation">y Rotation of the transform. Use this to change direction.</param>
        /// <param name="shrink">How much should the parabola shrink. By default is set to 100.</param>
        public static void DrawBiquadraticParabola(this Transform transform, float a, float b, Color color, float yEulerRotation = 0, float shrink = 100)
        {
            float spinFactor = yEulerRotation < 1 ? 1 : -1; //Change direction. Don't use  "== 0" because Unity likes broken numbers.

            a *= -1;   //Since most projectiles move downwards, reverse the A value.
            Gizmos.color = color;
            Vector2 currentPosition;

            for (int xPosition = 0; xPosition < 20; xPosition++)
            {
                currentPosition.x = transform.position.x + xPosition * spinFactor;
                currentPosition.y = transform.position.y + (a * xPosition * xPosition / shrink + b * xPosition / shrink);
                Gizmos.DrawSphere(currentPosition, 0.05f);
            }
        }


        public static void DrawObliqueThrow(this Transform transform, float gravity, float xSpeed, Vector2 ySpeedLimit)
        {
            float g = gravity * -Physics2D.gravity.y;
            float minTime = ySpeedLimit.x / (g / 2);
            float maxTime = ySpeedLimit.y / (g / 2);

            float minHeight = ySpeedLimit.x * ySpeedLimit.x / g / 2;
            float maxHeight = ySpeedLimit.y * ySpeedLimit.y / g / 2;

            float minFinalX = minTime * xSpeed;
            float maxFinalX = maxTime * xSpeed;

            Vector2 minY = new Vector2(minFinalX / 2, minHeight);
            Vector2 maxY = new Vector2(maxFinalX / 2, maxHeight);

            Vector2 minX = new Vector2(minFinalX, 0);
            Vector2 maxX = new Vector2(maxFinalX, 0);

            Vector2 currentPos = transform.position;
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(minX + currentPos, 0.1f);
            Gizmos.DrawSphere(minY + currentPos, 0.1f);

            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(maxX + currentPos, 0.1f);
            Gizmos.DrawSphere(maxY + currentPos, 0.1f);
        }
    }
}