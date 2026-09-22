using UnityEngine;
using UnityEngine.Events;

public class Building : MonoBehaviour
{
    [Header("건물 정보")]
    public BuildingType buildingType;
    public string buildingName = "건물";

    public BuildingEvents buildingEvents;

    private DeliveryOrderSystem orderSystem;

    private void Start()
    {
        SetupBuilding();
        orderSystem = FindFirstObjectByType<DeliveryOrderSystem>();
        CreateNameTag();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<DeliveryDriver>(out var driver))
        {
            buildingEvents.OnDriverEntered.Invoke(buildingName);
            HandleDriverService(driver);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<DeliveryDriver>(out _))
        {
            buildingEvents.OnDriverExtied.Invoke(buildingName);
            Debug.Log(buildingName + "을/를 떠났습니다.");
        }
    }

    private void HandleDriverService(DeliveryDriver driver)
    {
        switch (buildingType)
        {
            case BuildingType.Restaurant:
                if (orderSystem != null)
                {
                    orderSystem.OnDriverEnteredRestaurant(this);
                }
                break;

            case BuildingType.Customer:
                if (orderSystem != null)
                {
                    orderSystem.OnDirverEnteredCustorm(this);
                }
                else
                {
                    driver.CompleteDelivery();
                }
                break;

            case BuildingType.ChargingStation:
                driver.ChargeBattery();
                break;

            default:
                break;
        }

        buildingEvents.OnServiceUsed.Invoke(buildingType);
    }

    private void SetupBuilding()
    {
        if (TryGetComponent<Renderer>(out var renderer))
        {
            Material mat = renderer.material;

            switch (buildingType)
            {
                case BuildingType.Restaurant:
                    mat.color = Color.red;
                    break;

                case BuildingType.Customer:
                    mat.color = Color.green;
                    break;

                case BuildingType.ChargingStation:
                    mat.color = Color.yellow;
                    break;

                default:
                    break;
            }
        }

        if (TryGetComponent<Collider>(out var collider))
        {
            collider.isTrigger = true;
        }
    }

    private void CreateNameTag()
    {
        GameObject nameTag = new("NameTag");
        nameTag.transform.SetParent(transform);
        nameTag.transform.localPosition = Vector3.up * 1.5f;

        TextMesh textMesh = nameTag.AddComponent<TextMesh>();
        textMesh.text = buildingName;
        textMesh.characterSize = 0.2f;
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.color = Color.white;
        textMesh.fontSize = 20;

        nameTag.AddComponent<BillBoard>();
    }

    [System.Serializable]
    public class BuildingEvents
    {
        public UnityEvent<string> OnDriverEntered;
        public UnityEvent<string> OnDriverExtied;
        public UnityEvent<BuildingType> OnServiceUsed;
    }
}
