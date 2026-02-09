using Basalt.Framework.Networking.Serializers;
using Blasphemous.Multiplayer.Common.Packets;

namespace Blasphemous.Multiplayer.Common;

public class NetworkHelper
{
    public static IMessageSerializer CreateSerializer()
    {
        return new ClassicSerializer()
            .RegisterPacket<PositionPacket>(20, () => new PositionPacket(0, 0))
            .RegisterPacket<PositionResponsePacket>(21, () => new PositionResponsePacket(string.Empty, 0, 0))
            .RegisterPacket<AnimationPacket>(22, () => new PositionPacket(0, 0))
            .RegisterPacket<AnimationResponsePacket>(23, () => new PositionPacket(0, 0))
            .RegisterPacket<DirectionPacket>(24, () => new PositionPacket(0, 0))
            .RegisterPacket<DirectionResponsePacket>(25, () => new PositionPacket(0, 0))
            .RegisterPacket<ScenePacket>(26, () => new PositionPacket(0, 0))
            .RegisterPacket<SceneResponsePacket>(27, () => new PositionPacket(0, 0))

            .RegisterPacket<TeamPacket>(40, () => new PositionPacket(0, 0))
            .RegisterPacket<TeamResponsePacket>(41, () => new PositionPacket(0, 0))
            .RegisterPacket<SkinPacket>(42, () => new PositionPacket(0, 0))
            .RegisterPacket<SkinResponsePacket>(43, () => new PositionPacket(0, 0))

            .RegisterPacket<IntroPacket>(60, () => new PositionPacket(0, 0))
            .RegisterPacket<IntroResponsePacket>(61, () => new PositionPacket(0, 0))
            .RegisterPacket<JoinResponsePacket>(63, () => new PositionPacket(0, 0))
            .RegisterPacket<QuitResponsePacket>(65, () => new PositionPacket(0, 0))

            .RegisterPacket<AttackPacket>(80, () => new PositionPacket(0, 0))
            .RegisterPacket<AttackResponsePacket>(81, () => new PositionPacket(0, 0))
            .RegisterPacket<EffectPacket>(82, () => new PositionPacket(0, 0))
            .RegisterPacket<EffectResponsePacket>(83, () => new PositionPacket(0, 0))
            .RegisterPacket<ProgressPacket>(84, () => new PositionPacket(0, 0))
            .RegisterPacket<ProgressResponsePacket>(85, () => new PositionPacket(0, 0))
            .RegisterPacket<PingPacket>(86, () => new PositionPacket(0, 0))
            .RegisterPacket<PingResponsePacket>(87, () => new PositionPacket(0, 0));
    }
}
