using NetworkObj.Utils;
using System.Text;

namespace NetworkObj.Packets
{
    class GQuickMatch : ServerPacket
    {
        public class RoomInfo
        {
            public uint m_iRoomId;

            public uint m_iMapId;

            public string? m_strCreaterNickname;

            public uint m_iOnlineNum;

            public uint m_iMaxUserNum;

            public uint m_room_status;

            public uint m_Creater_level;

            public string? m_password;
        }

        public uint m_iCurpage;

        public uint m_pagesum;

        public uint m_values;

        public RoomInfo? room;

        public Writer Pack()
        {
            Writer p = new Writer();
            p.wuint(m_iCurpage);
            p.wuint(m_pagesum);
            p.wuint(m_values);

            if (room == null) return Packet.Pack(Protocols.CG_LEAVE_ROOM, p);
            p.wuint(room.m_iRoomId);
            p.wuint(room.m_iMapId);

            if (room.m_strCreaterNickname == null) return Packet.Pack(Protocols.CG_LEAVE_ROOM, p);
            byte[] nick = new byte[16];
            byte[] unick = Encoding.ASCII.GetBytes(room.m_strCreaterNickname);
            Array.Copy(unick, 0, nick, 0, (uint)Math.Min(16, unick.Length));
            p.wuint((uint)Math.Min(16, unick.Length));
            p.wbytes(nick);

            p.wuint(room.m_iOnlineNum);
            p.wuint(room.m_iMaxUserNum);
            p.wuint(room.m_room_status);
            p.wuint(room.m_Creater_level);

            if (room.m_password == null) return Packet.Pack(Protocols.CG_LEAVE_ROOM, p);
            byte[] pass = new byte[7];
            byte[] upass = Encoding.ASCII.GetBytes(room.m_password);
            Array.Copy(upass, 0, pass, 0, (uint)Math.Min(7, upass.Length));
            p.wuint((uint)Math.Min(7, upass.Length));
            p.wbytes(pass);

            return Packet.Pack(Protocols.GC_QUICK_ROOM_LIST, p);
        }
    }
}