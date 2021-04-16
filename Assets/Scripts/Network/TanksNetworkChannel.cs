using MLAPI.Transports;

namespace Tanks.Networking
{
    public enum TanksNetworkChannel : byte
    {
        TerrainUpdate = NetworkChannel.ChannelUnused + 1
    }
}