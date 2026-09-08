using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class DeliveryDriver : MonoBehaviour
{
    [Header("배달원 설정")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("상태")]
    [SerializeField] private float currentMoney = 0;
    [SerializeField] private float batteryLevel = 100f;
    [SerializeField] private int deliveryCount = 0;

    public DriverEvents driverEvents;

    private bool isMoving = false;

    private void Start()
    {
        driverEvents.OnMoneyChanged.Invoke(currentMoney);
        driverEvents.OnBatteryChanged.Invoke(batteryLevel);
        driverEvents.OnDeliveryCountChanged.Invoke(deliveryCount);
    }

    private void Update()
    {
        HandleMovement();
    }

    private void ChangeBattery(float amount)
    {
        float oldBattery = batteryLevel;
        batteryLevel = Mathf.Clamp(batteryLevel + amount, 0f, 100f);

        driverEvents.OnBatteryChanged.Invoke(batteryLevel);

        if (oldBattery > 20f && batteryLevel <= 20f)
        {
            driverEvents.OnLowBattery.Invoke();
        }
        if (oldBattery > 0f && batteryLevel <= 0f)
        {
            driverEvents.OnLowBatteryEmpty.Invoke();
        }
    }

    private void HandleMovement()
    {
        if (batteryLevel <= 0)
        {
            if (isMoving)
            {
                StopMoving();
            }
        }

        Vector3 moveDirection = Vector3.zero;
        if (Keyboard.current != null)
        {
            moveDirection = new(
                (Keyboard.current.dKey.isPressed ? 1 : 0) -
                (Keyboard.current.aKey.isPressed ? 1 : 0),
                0f,
                (Keyboard.current.wKey.isPressed ? 1 : 0) -
                (Keyboard.current.sKey.isPressed ? 1 : 0)
            );
        }

        if (moveDirection != Vector3.zero)
        {
            if (!isMoving)
            {
                StartMoving();
            }

            moveDirection = moveDirection.normalized;
            transform.Translate(moveSpeed * Time.deltaTime * moveDirection, Space.World);

            Quaternion targetRoation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRoation, rotationSpeed * Time.deltaTime);

            ChangeBattery(-Time.deltaTime * 3f);
        }
        else
        {
            if (isMoving)
            {
                StopMoving();
            }
        }
    }

    private void StartMoving()
    {
        isMoving = true;
        driverEvents.OnMoveStarted.Invoke();
    }

    private void StopMoving()
    {
        isMoving = false;
        driverEvents.OnMoveStopped.Invoke();
    }

    public void AddMoney(float amount)
    {
        currentMoney += amount;
        driverEvents.OnMoneyChanged.Invoke(currentMoney);
    }

    public void CompleteDelivery()
    {
        deliveryCount++;
        float reward = Random.Range(3000, 8000);
        AddMoney(reward);
        driverEvents.OnDeliveryCountChanged.Invoke(deliveryCount);
        driverEvents.OnDeliveryCompleted.Invoke();
    }

    public void ChargeBattery()
    {
        ChangeBattery(batteryLevel - 100f);
    }

    public string GetStatusText()
    {
        return $"돈 : {currentMoney:f0} | 배터리 : {batteryLevel:f1}% | 배달 : {deliveryCount} 건";
    }

    public bool CanMove()
    {
        return batteryLevel > 0;
    }

    [System.Serializable]
    public class DriverEvents
    {
        [Header("이동")]
        public UnityEvent OnMoveStarted;
        public UnityEvent OnMoveStopped;

        [Header("상태 변화 Event")]
        public UnityEvent<float> OnMoneyChanged;
        public UnityEvent<float> OnBatteryChanged;
        public UnityEvent<int> OnDeliveryCountChanged;

        [Header("경고 Event")]
        public UnityEvent OnLowBattery;
        public UnityEvent OnLowBatteryEmpty;
        public UnityEvent OnDeliveryCompleted;
    }
}
