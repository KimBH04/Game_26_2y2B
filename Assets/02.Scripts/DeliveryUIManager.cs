using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryUIManager : MonoBehaviour
{
    [Header("UI 요소")]
    [SerializeField] private Text statusText;
    [SerializeField] private Text messageText;
    [SerializeField] private Slider batterySlider;
    [SerializeField] private Image batteryFill;

    [SerializeField] private DeliveryDriver driver;

    private void Start()
    {
        driver.driverEvents.OnMoneyChanged.AddListener(UpdateMoney);
        driver.driverEvents.OnBatteryChanged.AddListener(UpdateBattery);
        driver.driverEvents.OnDeliveryCountChanged.AddListener(UpdateDeliveryCount);
        driver.driverEvents.OnMoveStarted.AddListener(OnMoveStarted);
        driver.driverEvents.OnMoveStopped.AddListener(OnMoveStopped);
        driver.driverEvents.OnLowBattery.AddListener(OnLowBattery);
        driver.driverEvents.OnLowBatteryEmpty.AddListener(OnBatteryEmpty);
        driver.driverEvents.OnDeliveryCompleted.AddListener(OnDeliveryCompleted);
    }

    private void Update()
    {
        UpdateUI();
    }

    private void OnDestroy()
    {
        driver.driverEvents.OnMoneyChanged.RemoveListener(UpdateMoney);
        driver.driverEvents.OnBatteryChanged.RemoveListener(UpdateBattery);
        driver.driverEvents.OnDeliveryCountChanged.RemoveListener(UpdateDeliveryCount);
        driver.driverEvents.OnMoveStarted.RemoveListener(OnMoveStarted);
        driver.driverEvents.OnMoveStopped.RemoveListener(OnMoveStopped);
        driver.driverEvents.OnLowBattery.RemoveListener(OnLowBattery);
        driver.driverEvents.OnLowBatteryEmpty.RemoveListener(OnBatteryEmpty);
        driver.driverEvents.OnDeliveryCompleted.RemoveListener(OnDeliveryCompleted);
    }

    private void ShowMessage(string message, Color color)
    {
        messageText.text = message;
        messageText.color = color;
        StartCoroutine(ClearMessageAfterDelay(2f));
    }

    private IEnumerator ClearMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        messageText.text = "";
    }

    private void UpdateMoney(float money)
    {
        ShowMessage($"돈 : {money} 원", Color.green);
    }

    private void UpdateBattery(float battery)
    {
        batterySlider.value = battery / 100f;

        if (battery > 50f)
        {
            batteryFill.color = Color.green;
        }
        else if (battery > 20f)
        {
            batteryFill.color = Color.yellow;
        }
        else
        {
            batteryFill.color = Color.red;
        }
    }

    private void UpdateDeliveryCount(int count)
    {
        ShowMessage($"배달 완료 : {count} 건", Color.blue);
    }

    private void OnMoveStarted()
    {
        ShowMessage("이동 시작", Color.cyan);
    }

    private void OnMoveStopped()
    {
        ShowMessage("이동 정시", Color.gray);
    }

    private void OnLowBattery()
    {
        ShowMessage("배터리 부족", Color.red);
    }

    private void OnBatteryEmpty()
    {
        ShowMessage("배터리 방전", Color.red);
    }

    private void OnDeliveryCompleted()
    {
        ShowMessage("배달 완료", Color.green);
    }

    private void UpdateUI()
    {
        statusText.text = driver.GetStatusText();
    }
}
