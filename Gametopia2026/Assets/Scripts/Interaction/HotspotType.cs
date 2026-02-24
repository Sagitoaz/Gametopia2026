namespace CoderGoHappy.Interaction
{
    /// <summary>
    /// Types of hotspot interactions
    /// </summary>
    public enum HotspotType
    {
        /// <summary>
        /// Pickup item - adds item to inventory
        /// </summary>
        Pickup,
        
        /// <summary>
        /// Use item - requires specific item from inventory
        /// </summary>
        ItemUse,
        
        /// <summary>
        /// Navigation - transitions to another scene
        /// </summary>
        Navigation,
        
        /// <summary>
        /// Puzzle - shows puzzle UI
        /// </summary>
        Puzzle,
        
        /// <summary>
        /// Examine - shows description/dialogue (optional feature)
        /// </summary>
        Examine
    }
}
