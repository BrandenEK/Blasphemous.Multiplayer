using Blasphemous.ModdingAPI.Persistence;
using System.Collections.Generic;

namespace Blasphemous.Multiplayer.Client;

/// <summary>
/// The slot save data for the multiplayer mod
/// </summary>
public class MultiplayerSlotData : SlotSaveData
{
    /// <summary>
    /// The list of IDs of interacted objects
    /// </summary>
    public List<string> InteractedPersistenceObjects { get; set; }
}
