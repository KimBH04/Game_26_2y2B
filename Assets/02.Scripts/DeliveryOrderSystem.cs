using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DeliveryOrderSystem : MonoBehaviour
{
    [Header("Order Settings")]
    public float orderGenerateInterval = 15f;
    public int maxActiveOrders = 8;

    [Header("Game State")]
    public int totalOrdersGenerated = 0;
    public int completedOrders = 0;
    public int expiredOrdrs = 0;

    private List<DeliveryOrder> currentOrders = new();

    private List<Building> restaurants = new();
    private List<Building> customers = new();

    public OrderSystemEvents orderEvents;
    public DeliveryDriver driver;

    private void Start()
    {
        driver = FindFirstObjectByType<DeliveryDriver>();
        FindAllBuilding();

        StartCoroutine(GenerateInitialOrders());
        StartCoroutine(OrderGenerator());
        StartCoroutine(ExpiredOrderChecker());
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(new(10, 10, 400, 1300));

        GUILayout.Label("=== 배달 주문 ===");
        GUILayout.Label($"활성 주문: {currentOrders.Count} 개");
        GUILayout.Label($"픽업 대기: {GetPickWaitingCount()} 개");
        GUILayout.Label($"배달 대기: {GetDeliveryWaitingCunt()} 개");
        GUILayout.Label($"완료 : {completedOrders} 개 | 만료 : {expiredOrdrs}");

        GUILayout.Space(10);

        foreach (var order in currentOrders)
        {
            string status = order.state == OrderState.WaitingPickup ? "픽업 대기" : "배달 대기";
            float timeLeft = order.GetRemainingTime();

            GUILayout.Label($"#{order.orderID} : {order.restaurantName} -> {order.customerName}");
            GUILayout.Label($"{status} | {timeLeft:F0} 초 남음");
        }

        GUILayout.EndArea();
    }

    private void FindAllBuilding()
    {
        Building[] allBuildings = FindObjectsByType<Building>(FindObjectsSortMode.None);

        foreach (var building in allBuildings)
        {
            if (building.buildingType == BuildingType.Restaurant)
            {
                restaurants.Add(building);
            }
            else if (building.buildingType == BuildingType.Customer)
            {
                customers.Add(building);
            }
        }

        Debug.Log($"음식점: {restaurants}점, 고객 {customers.Count}명 발견.");
    }

    private void CreateNewOrder()
    {
        if (restaurants.Count == 0 || customers.Count == 0)
        {
            return;
        }

        Building randRestaurant = restaurants[Random.Range(0, restaurants.Count)];
        Building randCustomer = customers[Random.Range(0, customers.Count)];

        if (randRestaurant == randCustomer)
        {
            randCustomer = customers[Random.Range(0, customers.Count)];
        }

        float reward = Random.Range(3000f, 8000f);

        DeliveryOrder newOrder = new(++totalOrdersGenerated, randRestaurant, randCustomer, reward);

        currentOrders.Add(newOrder);
        orderEvents.OnNewOrderAdded.Invoke(newOrder);
    }

    private void PickupOrder(DeliveryOrder order)
    {
        order.state = OrderState.PickedUp;
        orderEvents.OnOrderPickedUp.Invoke(order);
    }

    private void CompleteOrder(DeliveryOrder order)
    {
        order.state = OrderState.Completed;
        completedOrders++;

        if (driver != null)
        {
            driver.AddMoney(order.reward);
        }

        currentOrders.Remove(order);
        orderEvents.OnOrderCompleted.Invoke(order);
    }

    private void ExpireOrder(DeliveryOrder order)
    {
        order.state = OrderState.Epired;
        expiredOrdrs++;

        currentOrders.Remove(order);
        orderEvents.OnOrderExpired.Invoke(order);
    }

    public List<DeliveryOrder> GetcurrentOrders()
    {
        return new List<DeliveryOrder>(currentOrders);
    }

    public int GetPickWaitingCount()
    {
        int count = 0;
        foreach (var order in currentOrders)
        {
            if (order.state == OrderState.WaitingPickup)
            {
                count++;
            }
        }
        return count;
    }

    public int GetDeliveryWaitingCunt()
    {
        int count = 0;
        foreach (var order in currentOrders)
        {
            if (order.state == OrderState.PickedUp)
            {
                count++;
            }
        }
        return count;
    }

    private DeliveryOrder FindOrderForPickUp(Building restaurant)
    {
        foreach (var order in currentOrders)
        {
            if (order.restaurantBuilding == restaurant && order.state == OrderState.WaitingPickup)
            {
                return order;
            }
        }

        return null;
    }

    private DeliveryOrder FindOrderForDelivery(Building customer)
    {
        foreach (var order in currentOrders)
        {
            if (order.customerBuilding == customer && order.state == OrderState.PickedUp)
            {
                return order;
            }
        }

        return null;

    }

    public void OnDriverEnteredRestaurant(Building restaurant)
    {
        DeliveryOrder orderToPickUp = FindOrderForPickUp(restaurant);

        if (orderToPickUp != null)
        {
            PickupOrder(orderToPickUp);
        }
    }

    public void OnDirverEnteredCustorm(Building customer)
    {
        DeliveryOrder orderToDeliver = FindOrderForDelivery(customer);

        if (orderToDeliver != null)
        {
            CompleteOrder(orderToDeliver);
        }
    }

    private IEnumerator GenerateInitialOrders()
    {
        yield return new WaitForSeconds(1f);

        for (int i = 0; i < 3; i++)
        {
            CreateNewOrder();
            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator OrderGenerator()
    {
        for (; ; )
        {
            yield return new WaitForSeconds(orderGenerateInterval);

            if (currentOrders.Count < maxActiveOrders)
            {
                CreateNewOrder();
            }
        }
    }

    private IEnumerator ExpiredOrderChecker()
    {
        for (; ; )
        {
            yield return new WaitForSeconds(5f);
            List<DeliveryOrder> expiredOrders = new();

            foreach (var order in currentOrders)
            {
                if (order.IsExpired() && order.state != OrderState.Completed)
                {
                    expiredOrders.Add(order);
                }
            }

            foreach (var expired in expiredOrders)
            {
                ExpireOrder(expired);
            }
        }

    }

    [System.Serializable]
    public class OrderSystemEvents
    {
        public UnityEvent<DeliveryOrder> OnNewOrderAdded;
        public UnityEvent<DeliveryOrder> OnOrderPickedUp;
        public UnityEvent<DeliveryOrder> OnOrderCompleted;
        public UnityEvent<DeliveryOrder> OnOrderExpired;
    }
}
