using Basalt.Framework.Networking.Serializers;
using Blasphemous.Multiplayer.Common.Packets;
using Blasphemous.Multiplayer.Common.Serializers;

namespace Blasphemous.Multiplayer.Common;

public class NetworkHelper
{
    public static IMessageSerializer CreateSerializer()
    {
        return new ClassicSerializer()
            .RegisterPacket<PositionPacket>(20, () => new PositionPacket(0, 0))
            .RegisterPacket<PositionResponsePacket>(21, () => new PositionResponsePacket(string.Empty, 0, 0))
            .RegisterPacket<AnimationPacket>(22, () => new AnimationPacket(0))
            .RegisterPacket<AnimationResponsePacket>(23, () => new AnimationResponsePacket(string.Empty, 0))
            .RegisterPacket<DirectionPacket>(24, () => new DirectionPacket(true))
            .RegisterPacket<DirectionResponsePacket>(25, () => new DirectionResponsePacket(string.Empty, true))
            .RegisterPacket<ScenePacket>(26, () => new ScenePacket(string.Empty))
            .RegisterPacket<SceneResponsePacket>(27, () => new SceneResponsePacket(string.Empty, string.Empty))

            .RegisterPacket<TeamPacket>(40, () => new TeamPacket(1))
            .RegisterPacket<TeamResponsePacket>(41, () => new TeamResponsePacket(string.Empty, 1))
            .RegisterPacket<SkinPacket>(42, () => new SkinPacket(string.Empty))
            .RegisterPacket<SkinResponsePacket>(43, () => new SkinResponsePacket(string.Empty, string.Empty))

            .RegisterPacket<IntroPacket>(60, () => new IntroPacket(0, string.Empty, string.Empty, string.Empty, 0))
            .RegisterPacket<IntroResponsePacket>(61, () => new IntroResponsePacket(0))
            .RegisterPacket<JoinResponsePacket>(63, () => new JoinResponsePacket(string.Empty, 0))
            .RegisterPacket<QuitResponsePacket>(65, () => new QuitResponsePacket(string.Empty))

            .RegisterPacket<AttackPacket>(80, () => new AttackPacket(0, 0, string.Empty))
            .RegisterPacket<AttackResponsePacket>(81, () => new AttackResponsePacket(string.Empty, 0, 0, string.Empty))
            .RegisterPacket<EffectPacket>(82, () => new EffectPacket(0))
            .RegisterPacket<EffectResponsePacket>(83, () => new EffectResponsePacket(string.Empty, 0))
            .RegisterPacket<ProgressPacket>(84, () => new ProgressPacket(0, string.Empty, 0))
            .RegisterPacket<ProgressResponsePacket>(85, () => new ProgressResponsePacket(string.Empty, 0, string.Empty, 0))
            .RegisterPacket<PingPacket>(86, () => new PingPacket(0, 0))
            .RegisterPacket<PingResponsePacket>(87, new PingResponseSerializer());
    }
}
