using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "SO/Data/PlayerData")]
public class PlayerDataSO : BaseDataSO
{
    public PlayerProfile Profile;
    public PlayerInventory Inventory;

    public override void Save()
    {
        DataSystem.Save("player-profile", Profile, Encrypt);
        DataSystem.Save("player-inventory", Inventory, Encrypt);
    }

    public override void Load()
    {
        Profile = DataSystem.Load<PlayerProfile>("player-profile", Encrypt);
        Inventory = DataSystem.Load<PlayerInventory>("player-inventory", Encrypt);
    }

    public override void Delete()
    {
        DataSystem.Delete("player-profile");
        DataSystem.Delete("player-inventory");
    }
}