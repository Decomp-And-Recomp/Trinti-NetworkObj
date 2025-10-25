using NetworkObj.Utils;
using System.Buffers;
using System.Net.Sockets;

namespace NetworkObj
{
    public class User
    {
        public int UserId = -1;
        public int RoomId = -1;
        public bool RoomMaster = false;
        public int Avatar = 1;
        public int Level = 1;
        public string Name = "Player";
        public int Index = 0;
        public string IP = "";
        // private int Kills; (survival)
    }

    public class Room
    {
        public int MapId = -1;
        public int Online = 1;
        public string Password = string.Empty;
        public int Max = 4; // TODO: change to 3 if client is in survival (detect using 1u packet)
        public List<TcpClient> Players = new List<TcpClient>(4);
        public int Dead = 0;
        public bool Started = false;
        public bool Survival = false;
    }

    public struct Vector3
    {
        public float x;
        public float y;
        public float z;

        public bool FromReader(Reader reader)
        {
            uint xRaw = reader.ruint();
            uint yRaw = reader.ruint();
            uint zRaw = reader.ruint();

            x = (float)(int)xRaw / 100f;
            y = (float)(int)yRaw / 100f;
            z = (float)(int)zRaw / 100f;

            return true;
        }

        public void ToWriter(Writer writer)
        {
            writer.wuint(unchecked((uint)(int)(x * 100)));
            writer.wuint(unchecked((uint)(int)(y * 100)));
            writer.wuint(unchecked((uint)(int)(z * 100)));
        }

        public override string ToString()
        {
            return $"{{{x}, {y}, {z}}}";
        }
    }

    public enum Avatars : int
    {
        JoeBlo = 0,
        Worker = 1,
        Nerd = 2,
        Doctor = 3,
        Cowboy = 4,
        Swat = 5,
        Marine = 6,
        BEAF = 7,
        Pirate = 8,
        Ninja = 9,
        Pastor = 10,
        Eskimo = 11,
        Evil = 12,
        Hunter = 13,
        PixelJoeBlo = 14,
        PixelMercenary = 15,
        Charlemagne = 16,
        Constantine = 17,
        Mike = 18
    }

    public enum Weapons : int
    {
        Empty = 0,
        AR = 1,
        Shotgun = 2,
        RocketLauncher = 3,
        Gatling = 4,
        Laser = 5,
        Sniper = 6,
        Chainsaw = 7,
        M32 = 8,
        Saber = 10,
        Flamethrower = 11,
        Electric = 12,
        Crossbow = 14
    }

    public class Get
    {
        public string Avatar(int Character)
        {
            switch ((Avatars)Character)
            {
                case Avatars.JoeBlo:
                    return "Human";
                case Avatars.BEAF:
                    return "B.E.A.F";
                case Avatars.PixelJoeBlo:
                    return "Pixel Human";
                case Avatars.PixelMercenary:
                    return "Pixel Mercenary";
                default:
                    return Enum.GetName<Avatars>((Avatars)Character);
            }

            return "Unidentified";
        }
    }
}