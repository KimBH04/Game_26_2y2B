using UnityEngine;
using UnityEngine.Events;

public class Building : MonoBehaviour
{
    [Header("건물 정보")]
    [SerializeField] private BuildingType type;
    [SerializeField] private string buildingName = "건물";

    [SerializeField] private BuildingEvents buildingEvents;

    private void Start()
    {
        SetupBuilding();
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
        switch (type)
        {
            case BuildingType.Restaurant:
                Debug.Log(buildingName + "에서 음식을 픽업했습니다.");
                break;

            case BuildingType.Customer:
                Debug.Log(buildingName + " 배달 완료.");
                break;

            case BuildingType.ChargingStation:
                Debug.Log(buildingName + "에서 배터리를 충전했습니다.");
                break;

            default:
                break;
        }
    }

    private void SetupBuilding()
    {
        if (TryGetComponent<Renderer>(out var renderer))
        {
            Material mat = renderer.material;

            switch (type)
            {
                case BuildingType.Restaurant:
                    mat.color = Color.red;
                    buildingName = "음식점";
                    break;

                case BuildingType.Customer:
                    mat.color = Color.green;
                    buildingName = "고객 집";
                    break;

                case BuildingType.ChargingStation:
                    mat.color = Color.yellow;
                    buildingName = "충전소";
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

    [System.Serializable]
    public class BuildingEvents
    {
        public UnityEvent<string> OnDriverEntered;
        public UnityEvent<string> OnDriverExtied;
        public UnityEvent<BuildingType> OnServiceUsed;
    }
}
