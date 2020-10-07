using System;

public class RgbPoint3
    {
        public float x;
        public float y;
        public float z;
        public int[] rgb;

        public RgbPoint3(byte[] bytes, RosSharp.RosBridgeClient.Messages.Sensor.PointField[] fields)
        {
            foreach (var field in fields)
            {
                byte[] slice = new byte[field.count * 4];
                Array.Copy(bytes, field.offset, slice, 0, field.count * 4);
                switch (field.name)
                {
                    case "x":
                        x = GetValue(slice);
                        break;
                    case "y":
                        y = GetValue(slice);
                        break;
                    case "z":
                        z = GetValue(slice);
                        break;
                    case "rgb":
                        rgb = GetRGB(slice);
                        break;
                }
            }
        }

        public override string ToString()
        {
            return "xyz=(" + x.ToString() + ", " + y.ToString() + ", " + z.ToString() + ")"
                + "  rgb=(" + rgb[0].ToString() + ", " + rgb[1].ToString() + ", " + rgb[2].ToString() + ")";
        }
        private static float GetValue(byte[] bytes)
        {
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(bytes);

            float result = BitConverter.ToSingle(bytes, 0);
            return result;
        }
        private static int[] GetRGB(byte[] bytes)
        {
            int[] rgb = new int[3];
            rgb[0] = Convert.ToInt16(bytes[0]);
            rgb[1] = Convert.ToInt16(bytes[1]);
            rgb[2] = Convert.ToInt16(bytes[2]);

			return rgb;
        }
    }
