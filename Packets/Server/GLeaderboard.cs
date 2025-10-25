using NetworkObj.Utils;

namespace NetworkObj.Packets
{
    class GLeaderboard : ServerPacket
    {
        public uint m_iUserId;

        public uint mKill_count;

        public uint mDeath_count;

        public uint mCash_loot;

        public long mDamage_val;

        public Writer Pack()
        {
            Writer p = new Writer();
            p.wuint(m_iUserId);
            p.wuint(mKill_count);
            p.wuint(mDeath_count);
            p.wuint(mCash_loot);
            p.wulong((ulong)mDamage_val);

            return Packet.Pack(Protocols.GC_COOP_WINNER, p);
        }
    }
}