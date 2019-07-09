using SFML.Window;
using SFML.System;
using SFML.Audio;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using static SFML_NET_3D.Data;
using static SFML_NET_3D.Constants;
using static SFML_NET_3D.Utility;

namespace SFML_NET_3D
{
    class Renderer
    {
        private static List<PlaneVertexArray> PlaneList { get; set; } = new List<PlaneVertexArray>();

        public static void Clear()
        {
            PlaneList.Clear();
        }

        public static void Add(PlaneVertexArray plane)
        {
            PlaneList.Add(plane);
        }

        public static void Render()
        {
            if (PlaneList == null)
                return;

            //Sort Plane List By Z-Order
            PlaneList = SortByZOrder(PlaneList, 0, PlaneList.Count - 1);

            //Display Planes One By One;
            for (int i = 0; i < PlaneList.Count; i++)
                PlaneList[i].Display();

            //PlaneList.Clear();
        }

        private static List<PlaneVertexArray> SortByZOrder(List<PlaneVertexArray> list, int start, int end)
        {
            int i;
            if (start < end)
            {
                i = Partition(list, start, end);

                SortByZOrder(list, start, i - 1);
                SortByZOrder(list, i + 1, end);
            }

            return list;
        }

        private static int Partition(List<PlaneVertexArray> list, int start, int end)
        {
            PlaneVertexArray temp;
            PlaneVertexArray p = list[end];
            int i = start - 1;

            for (int j = start; j <= end - 1; j++)
            {
                if (list[j].GetDepth() <= p.GetDepth())
                {
                    i++;
                    temp = list[i];
                    list[i] = list[j];
                    list[j] = temp;
                }
            }

            temp = list[i + 1];
            list[i + 1] = list[end];
            list[end] = temp;
            return i + 1;
        }

    }
}