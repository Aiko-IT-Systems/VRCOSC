﻿// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

namespace VRCOSC.Game.Modules.Modules.Heartrate.Pulsoid;

public class PulsoidModule : HeartRateModule
{
    private const string pulsoid_access_token_url = "https://pulsoid.net/oauth2/authorize?response_type=token&client_id=a31caa68-b6ac-4680-976a-9761b915a1e3&redirect_uri=&scope=data:heart_rate:read&state=a52beaeb-c491-4cd3-b915-16fed71e17a8&response_mode=web_page";

    public override string Title => "Pulsoid";
    public override string Description => "Connects to Pulsoid and sends your heartrate to VRChat";

    protected override HeartRateProvider CreateHeartRateProvider() => new PulsoidProvider(GetSetting<string>(PulsoidSetting.AccessToken));

    protected override void CreateAttributes()
    {
        base.CreateAttributes();
        CreateSetting(PulsoidSetting.AccessToken, "Access Token", "Your Pulsoid access token", string.Empty, () => OpenUrlExternally(pulsoid_access_token_url));
    }

    protected override void OnStart()
    {
        var accessToken = GetSetting<string>(PulsoidSetting.AccessToken);

        if (string.IsNullOrEmpty(accessToken))
        {
            Terminal.Log("Cannot connect to Pulsoid. Please obtain an access token");
            return;
        }

        base.OnStart();
    }

    private enum PulsoidSetting
    {
        AccessToken
    }
}