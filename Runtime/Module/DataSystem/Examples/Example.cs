using UnityEngine;

public class Example : MonoBehaviour
{
    private void Awake()
    {
        var storages = Resources.LoadAll<BaseDataSO>("");
        Debug.Log("storages loaded: " + storages.Length);
        foreach (var storage in storages)
        {
            StorageService.Register(storage);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StorageService.Get<PlayerDataKeySO>().AddCurrency(new Currency()
            {
                Id = UnityEngine.Random.Range(0, int.MaxValue).ToString(),
                Qty = UnityEngine.Random.Range(0, int.MaxValue)
            });
        }
    }
}