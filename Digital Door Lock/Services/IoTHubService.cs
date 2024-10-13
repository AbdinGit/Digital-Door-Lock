using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using System;
using System.Text;
using System.Threading.Tasks;

public class IoTHubService
{
    private DeviceClient deviceClient;
    private string deviceConnectionString = "HostName=AbdinIOThub.azure-devices.net;DeviceId=cec95419-748a-423a-a9ce-bc9f6c9e6e9a;SharedAccessKey=SeFps8MV4pTBV/5hrcsW28Nlz7/j934q0IiFTBU9XZw="; // Replace with your connection string

    public IoTHubService(string deviceConnectionString)
    {
        deviceClient = DeviceClient.CreateFromConnectionString(deviceConnectionString, TransportType.Mqtt);
    }


    public async Task<Twin> GetDeviceTwinAsync()
    {
        try
        {
            return await deviceClient.GetTwinAsync();
        }
        catch (Exception ex)
        {

            throw new Exception("Could not retrieve device twin", ex);
        }
    }


    public async Task<bool> ComparePinAsync(string enteredPin)
    {
        var twin = await GetDeviceTwinAsync();


        if (twin != null && twin.Properties.Desired.Contains("pinCode"))
        {
            string storedPin = twin.Properties.Desired["pinCode"].ToString();
            return enteredPin == storedPin;
        }
        return false;
    }


    public async Task ReportDeviceStateAsync(string pinCode)
    {
        var twinCollection = new TwinCollection();
        twinCollection["pinCode"] = pinCode;
        await deviceClient.UpdateReportedPropertiesAsync(twinCollection);
    }
}