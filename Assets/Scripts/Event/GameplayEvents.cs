using ARPG;

namespace ARPG
{
    public struct CharacterDeathEvent
    {
        public CharacterManager Character;
    }

    public struct GameDataLoadedEvent
    {
    }

    public struct NPCFollowPlayerEvent
    {
        public bool IsFollowing;
    }
}
