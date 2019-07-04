using System;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using SFML.Audio;
using static SFML_NET_3D.Constants;

namespace SFML_NET_3D
{
    public class Utility
    {
        #region CALCULATIONS 

        /// <summary>
        /// (Vector2f)vector를 최소 (float)min에서 최대 (float)max만큼 한정한 값을 반환합니다.
        /// </summary>
        public static Vector2f Limit(Vector2f vector, float min, float max)
        {
            if (GetMagnitude(vector) < min)
                vector = SetMagnitude(vector, min);
            else if (GetMagnitude(vector) > max)
                vector = SetMagnitude(vector, max);
            return vector;
        }

        /// <summary>
        /// (Vector2f)vector를 최대 (float)max만큼 한정한 값을 반환합니다.
        /// </summary>
        public static Vector2f Limit(Vector2f vector, float max)
        {
            if (GetMagnitude(vector) > max)
                vector = SetMagnitude(vector, max);
            return vector;
        }

        /// <summary>
        /// (float)var을 최소 (float)min에서 최대 (float)max만큼 한정한 값을 반환합니다.
        /// </summary>
        public static float Limit(float var, float min, float max)
        {
            if (var < min) var = min;
            else if (var > max) var = max;
            return var;
        }

        /// <summary>
        /// (float)var을 최대 (float)max만큼 한정한 값을 반환합니다.
        /// </summary>
        public static float Limit(float var, float max)
        {
            if (var > max) var = max;
            return var;
        }

        /// <summary>
        /// (float)start1에서 (float)stop1 사이의 값인 (float)value를 (float)start2 에서 (float)stop2 사이의 비율로 치환한 값을 반환합니다.
        /// </summary>
        public static float Map(float value, float start1, float stop1, float start2, float stop2)
        {
            return ((value - start1) / (stop1 - start1)) * (stop2 - start2) + start2;
        }

        /// <summary>
        /// 라디안각 (float)radian 의 디그리각을 반환합니다.
        /// </summary>
        public static float ToDegree(float radian)
        {
            return radian * 180 / MathF.PI;
        }

        /// <summary>
        /// 디그리각 (float)degree 의 라디안각을 반환합니다.
        /// </summary>
        public static float ToRadian(float degree)
        {
            return degree * MathF.PI / 180;
        }

        /// <summary>
        /// (float[])array의 엘리먼츠중 최솟값을 반환합니다.
        /// </summary>
        public static float GetMin(float[] array)
        {
            float min = array[0];
            foreach (var element in array)
            {
                if (element <= min) min = element;
            }
            return min;
        }
        /// <summary>
        /// (float[])array의 엘리먼츠중 최댓값을 반환합니다.
        /// </summary>
        public static float GetMax(float[] array)
        {
            float max = array[0];
            foreach (var element in array)
            {
                if (element >= max) max = element;
            }
            return max;
        }
        /// <summary>
        /// (int[])array의 엘리먼츠중 최솟값을 반환합니다.
        /// </summary>
        public static int GetMin(int[] array)
        {
            int min = array[0];
            foreach (var element in array)
            {
                if (element <= min) min = element;
            }
            return min;
        }
        /// <summary>
        /// (int[])array의 엘리먼츠중 최댓값을 반환합니다.
        /// </summary>
        public static int GetMax(int[] array)
        {
            int max = array[0];
            foreach (var element in array)
            {
                if (element >= max) max = element;
            }
            return max;
        }
        #endregion

        #region VECTORS

        /// <summary>
        /// (Vector2f)vector의 크기를 반환합니다.
        /// </summary>
        public static float GetMagnitude(Vector2f vector)
        {
            return MathF.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
        }

        /// <summary>
        /// (Vector2f)vector의 크기를 (float)mag로 한정한 값을 반환합니다.
        /// </summary>
        public static Vector2f SetMagnitude(Vector2f vector, float mag)
        {
            vector = Normalize(vector);
            vector *= mag;
            return vector;
        }

        /// <summary>
        /// 정규화된 (Vector2f)vector를 반환합니다.
        /// </summary>
        public static Vector2f Normalize(Vector2f vector)
        {
            var magnitude = GetMagnitude(vector);
            return vector /= magnitude;
        }

        /// <summary>
        /// (Vector2f)pos1과 (Vector2f)pos2 사이의 거리를 반환합니다.
        /// </summary>
        public static float Distnace(Vector2f pos1, Vector2f pos2)
        {
            return MathF.Sqrt
            (
                MathF.Pow(pos1.X - pos2.X, 2) +
                MathF.Pow(pos1.Y - pos2.Y, 2)
            );
        }

        /// <summary>
        /// (Vector2)vector의 원점을 기준으로 한 라디안 각도를 반환합니다.
        /// </summary>
        public static float GetAngle(Vector2f vector)
        {
            return MathF.Atan2(vector.Y, vector.X);
        }

        /// <summary>
        /// 라디안각 (float)angle만큼 회전시킨 (Vector2f)vector를 반환합니다.
        /// </summary>
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
            /// <summary>
            /// (float[])Noise() 메소드에서 사용될 파라미터들을 설정합니다.
            /// </summary>
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

        /// <summary>
        /// (NoiseFactors)noiseFactors에 따른 펄린 노이즈 배열을 반환합니다.
        /// </summary>
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