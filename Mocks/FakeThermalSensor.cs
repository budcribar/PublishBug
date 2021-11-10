using System;
using System.Threading;
using System.Threading.Tasks;
using ToolFrameworkPackage;

namespace Mocks
{
    public class ThermalSensorWrapper : IThermalZone
    {
        Task? simTask;
        CancellationTokenSource tokenSource = new();

        public void FireInitialEvent()
        {
            //Event?.Invoke(this, new() );
        }

        public TimeSpan MinimumEventPeriod { get; set; } = TimeSpan.FromSeconds(1);
        private readonly DateTime startTime = DateTime.Now;

        private EventHandler<ThermalZoneEventArgs>? eventHandler;

        public event EventHandler<ThermalZoneEventArgs> Event
        {
            add
            {
                StartSimulation();
                eventHandler += value;
            }
            remove
            {
                StopSimulation();
                eventHandler -= value;
            }
        }

        public void StartSimulation()
        {
            string[] zones = { "TZ00_0", "TZ01_0" };
            if (simTask == null)
            {
                simTask = Task.Run(async () =>
                {
                    while (true)
                    {
                        var zone = zones[new Random().Next(0, zones.Length)];
                        eventHandler?.Invoke(this, new ThermalZoneEventArgs { CurrentTemperature = new Random().Next(80, 101), Zone = zone });
                        try
                        {
                            await Task.Delay(MinimumEventPeriod, tokenSource.Token);
                        }
                        catch (Exception)
                        {
                            break;
                        }
                    }
                }, tokenSource.Token);
            }
        }

        public async void StopSimulation()
        {
            tokenSource.Cancel();
            try
            {
                if (simTask != null)
                    await simTask;
            }
            finally
            {
                simTask = null;
                tokenSource.Dispose();
                tokenSource = new();
            }
        }

        public ThermalSensorWrapper()
        {

        }
    }
    public class FakeThermalZoneManager : IHardwareManager<ThermalZoneEventArgs>
    {
        public FakeThermalZoneManager()
        {

        }
        public IHardware<ThermalZoneEventArgs> GetDefault()
        {
            return new ThermalSensorWrapper();
        }
    }
}
