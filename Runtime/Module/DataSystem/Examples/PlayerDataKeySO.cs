using UnityEngine;

[CreateAssetMenu(fileName = "PlayerDataKey", menuName = "PlayerDataKey")]
public class PlayerDataKeySO : BaseDataKeySO<PlayerInventory>
{
    public void AddCurrency(Currency currency)
    {
        if (!this._Data.Currencies.Contains(currency))
        {
            this._Data.Currencies.Add(currency);
            this.Save();
            // Raise event...
        }
    }
}