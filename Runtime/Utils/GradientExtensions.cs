using Gilzoide.LottiePlayer.RLottie;
using Unity.Mathematics;
using UnityEngine;

namespace Gilzoide.LottiePlayer
{
    public static class GradientExtensions
    {
        public unsafe static Color32 ColorForPoint(this Node.Gradient gradient, Vector2 point)
        {
            float2 v = gradient.end - gradient.start;
            float2 projected = math.project(gradient.end - gradient.start, point - gradient.start);

            float t = math.length(projected) / math.length(v);
            if (t <= 0)
            {
                return gradient.stopPtr[0].color;
            }
            else if (t >= 1)
            {
                return gradient.stopPtr[(uint) gradient.stopCount - 1].color;
            }

            for (uint i = 1; i < (uint) gradient.stopCount; i++)
            {
                GradientStop nextStop = gradient.stopPtr[i];
                if (nextStop.pos >= t)
                {
                    GradientStop previousStop = gradient.stopPtr[i - 1];
                    float relativeT = math.unlerp(previousStop.pos, nextStop.pos, t);
                    return Color32.Lerp(previousStop.color, nextStop.color, relativeT);
                }
            }
            return gradient.stopPtr[0].color;
        }
    }
}
