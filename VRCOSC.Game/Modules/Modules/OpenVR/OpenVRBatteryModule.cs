﻿// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System.Linq;
using System.Threading.Tasks;

namespace VRCOSC.Game.Modules.Modules.OpenVR;

public class OpenVRBatteryModule : Module
{
    public override string Title => "OpenVR Battery";
    public override string Description => "Gets battery stats from your OpenVR (SteamVR) session";
    public override string Author => "VolcanicArts";
    public override ModuleType ModuleType => ModuleType.General;
    protected override int DeltaUpdate => 5000;
    protected override bool ExecuteUpdateImmediately => false;

    protected override void CreateAttributes()
    {
        CreateParameter<float>(OpenVRParameter.HMD_Battery, ParameterMode.Write, "VRCOSC/OpenVR/Battery/HMD", "The battery percentage normalised of your headset");

        CreateParameter<float>(OpenVRParameter.LeftController_Battery, ParameterMode.Write, "VRCOSC/OpenVR/Battery/LeftController", "The battery percentage normalised of your left controller");
        CreateParameter<float>(OpenVRParameter.RightController_Battery, ParameterMode.Write, "VRCOSC/OpenVR/Battery/RightController", "The battery percentage normalised of your right controller");

        CreateParameter<float>(OpenVRParameter.Tracker1_Battery, ParameterMode.Write, "VRCOSC/OpenVR/Battery/Trackers/1", "The battery percentage normalised of tracker 1");
        CreateParameter<float>(OpenVRParameter.Tracker2_Battery, ParameterMode.Write, "VRCOSC/OpenVR/Battery/Trackers/2", "The battery percentage normalised of tracker 2");
        CreateParameter<float>(OpenVRParameter.Tracker3_Battery, ParameterMode.Write, "VRCOSC/OpenVR/Battery/Trackers/3", "The battery percentage normalised of tracker 3");
        CreateParameter<float>(OpenVRParameter.Tracker4_Battery, ParameterMode.Write, "VRCOSC/OpenVR/Battery/Trackers/4", "The battery percentage normalised of tracker 4");
        CreateParameter<float>(OpenVRParameter.Tracker5_Battery, ParameterMode.Write, "VRCOSC/OpenVR/Battery/Trackers/5", "The battery percentage normalised of tracker 5");
        CreateParameter<float>(OpenVRParameter.Tracker6_Battery, ParameterMode.Write, "VRCOSC/OpenVR/Battery/Trackers/6", "The battery percentage normalised of tracker 6");
        CreateParameter<float>(OpenVRParameter.Tracker7_Battery, ParameterMode.Write, "VRCOSC/OpenVR/Battery/Trackers/7", "The battery percentage normalised of tracker 7");
        CreateParameter<float>(OpenVRParameter.Tracker8_Battery, ParameterMode.Write, "VRCOSC/OpenVR/Battery/Trackers/8", "The battery percentage normalised of tracker 8");
    }

    protected override Task OnUpdate()
    {
        var battery = OpenVrInterface.GetHMDBatteryPercentage();
        if (battery is not null) SendParameter(OpenVRParameter.HMD_Battery, (float)battery);

        var batteryLeft = OpenVrInterface.GetLeftControllerBatteryPercentage();
        SendParameter(OpenVRParameter.LeftController_Battery, batteryLeft);

        var batteryRight = OpenVrInterface.GetRightControllerBatteryPercentage();
        SendParameter(OpenVRParameter.RightController_Battery, batteryRight);

        var trackerBatteries = OpenVrInterface.GetTrackersBatteryPercentages().ToList();

        for (int i = 0; i < trackerBatteries.Count; i++)
        {
            SendParameter(OpenVRParameter.Tracker1_Battery + i, trackerBatteries[i]);
        }

        return Task.CompletedTask;
    }

    private enum OpenVRParameter
    {
        HMD_Battery,
        LeftController_Battery,
        RightController_Battery,
        Tracker1_Battery,
        Tracker2_Battery,
        Tracker3_Battery,
        Tracker4_Battery,
        Tracker5_Battery,
        Tracker6_Battery,
        Tracker7_Battery,
        Tracker8_Battery
    }
}