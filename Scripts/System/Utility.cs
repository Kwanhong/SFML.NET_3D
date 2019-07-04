using System;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using SFML.Audio;
using static sfml.net_3d.Constants;

namespace sfml.net_3d
{
    public class Utility
    {
        #region CALCULATIONS 
        public static Vector2f Limit(Vector2f vector, float min, float max)
        {
            if (GetMagnitude(vector) < min)
                vector = SetMagnitude(vector, min);
            else if (GetMagnitude(vector) > max)
                vector = SetMagnitude(vector, max);
            return vector;
        }

        public static Vector2f Limit(Vector2f vector, float max)
        {
            if (GetMagnitude(vector) > max)
                vector = SetMagnitude(vector, max);
            return vector;
        }

        public static float Limit(float var, float min, float max)
        {
            if (var < min) var = min;
            else if (var > max) var = max;
            return var;
        }

        public static float Limit(float var, float max)
        {
            if (var > max) var = max;
            return var;
        }

        public static float Map(float value, float start1, float stop1, float start2, float stop2)
        {
            return ((value - start1) / (stop1 - start1)) * (stop2 - start2) + start2;
        }

        public static float ToDegree(float degree)
        {
            return degree * 180 / MathF.PI;
        }

        public static float ToRadian(float radian)
        {
            return radian * MathF.PI / 180;
        }

        public static float GetMin(float[] array)
        {
            float min = array[0];
            foreach (var element in array)
            {
                if (element <= min) min = element;
            }
            return min;
        }

        public static float GetMax(float[] array)
        {
            float max = array[0];
            foreach (var element in array)
            {
                if (element >= max) max = element;
            }
            return max;
        }
        #endregion

        #region VECTORS

        public static float GetMagnitude(Vector2f vector)
        {
            return MathF.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
        }

        public static Vector2f SetMagnitude(Vector2f vector, float mag)
        {
            vector = Normalize(vector);
            vector *= mag;
            return vector;
        }

        public static Vector2f Normalize(Vector2f vector)
        {
            var magnitude = GetMagnitude(vector);
            return vector /= magnitude;
        }

        public static float Distnace(Vector2f pos1, Vector2f pos2)
        {
            return MathF.Sqrt
            (
                MathF.Pow(pos1.X - pos2.X, 2) +
                MathF.Pow(pos1.Y - pos2.Y, 2)
            );
        }

        public static float GetAngle(Vector2f pos)
        {
            return MathF.Atan2(pos.Y, pos.X);
        }

        public static Vector2f RotateVector(Vector2f vector, float angle)
        {
            return new Vector2f
            (
                MathF.Cos(angle) * vector.X -
                MathF.Sin(angle) * vector.Y,
                MathF.Sin(angle) * vector.X +
                MathF.Cos(angle) * vector.Y
            );
        }
        #endregion

        #region PERLIN NOISE
        public class NoiseFactors
        {
            public NoiseFactors(
                int size = 800,
                int octave = 10,
                float softness = 8,
                float interval = 5,
                int randomSeed = 999)
            {
                Size = size;
                Octave = octave;
                Softness = softness;
                Interval = interval;
                RandomSeed = randomSeed;
            }
            public int Size { get; set; }
            public int Octave { get; set; }
            public float Softness { get; set; }
            public float Interval { get; set; }
            public int RandomSeed { get; set; }
        }

        public static float[] Noise(NoiseFactors noiseFactors)
        {
            float[] output = new float[noiseFactors.Size];
            float[] seed = new float[noiseFactors.Size];

            Random rand = new Random(noiseFactors.RandomSeed);
            for (int i = 0; i < noiseFactors.Size; i++)
                seed[i] = (float)rand.NextDouble();

            for (int x = 0; x < noiseFactors.Size; x++)
            {
                float noise = 0f;
                float scale = 1f;
                float scaleAcc = 0f;

                for (int o = 0; o < noiseFactors.Octave; o++)
                {
                    int pitch = noiseFactors.Size >> o;
                    int sample1 = (x / pitch) * pitch;
                    int sample2 = (sample1 + pitch) % noiseFactors.Size;
                    float blend = (float)(x - sample1) / (float)pitch;
                    float sample = (1f - blend) * seed[sample1] + blend * seed[sample2];

                    noise += sample * scale;
                    scaleAcc += scale;
                    scale = scale / noiseFactors.Softness;
                }
                output[x] = noise / scaleAcc;
            }
            return output;
        }
        #endregion

    }

}