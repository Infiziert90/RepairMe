using System;
using System.Text;
using Newtonsoft.Json;
using ProtoBuf;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace RepairMe;

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
public class SpawnPatternPacket
{
    public byte[] AsByte;
}

public class Core : ModSystem
{
    private const string ConfigName = "RepairMe.json";
    public static RepairConfig Config;

    public static ICoreAPI Api;
    public static ICoreServerAPI ServerApi;
    
    public override void Start(ICoreAPI api)
    {
        base.Start(api);
        
        api.Network.RegisterChannel("RepairMeChannel").RegisterMessageType<SpawnPatternPacket>();
        
        api.RegisterItemClass("repair", typeof(RepairItem));
    }

    public override void StartClientSide(ICoreClientAPI api)
    {
        base.StartClientSide(api);
        Api = api;

        api.Network.GetChannel("RepairMeChannel").SetMessageHandler<SpawnPatternPacket>(ReceivePacket);
    }

    public override void StartServerSide(ICoreServerAPI api)
    {
        base.StartServerSide(api);
        ServerApi = api;
        
        try
        {
            Config = api.LoadModConfig<RepairConfig>(ConfigName) ?? new RepairConfig();
        }
        catch (Exception ex)
        {
            api.Logger.Error("Unable to load config.");
            api.Logger.Error(ex);

            Config = new RepairConfig();
        }
        finally
        {
            api.StoreModConfig(Config, ConfigName);
        }

        api.Event.PlayerJoin += SendPacket;
    }

    /// <summary>
    /// Receives the config from the server
    /// </summary>
    /// <param name="packet">Packet filled with a byte array containing the json</param>
    private void ReceivePacket(SpawnPatternPacket packet)
    {
        Config = JsonConvert.DeserializeObject<RepairConfig>(Encoding.Default.GetString(packet.AsByte));
        Api.Logger.Debug($"Received Packet with Values: {Config.ToolRestoreAsPercent}");
    }

    /// <summary>
    /// Sends the config from server to client
    /// </summary>
    /// <param name="player">Specific player to receive this config message</param>
    private void SendPacket(IServerPlayer player)
    {
        var packet = new SpawnPatternPacket { AsByte = Encoding.Default.GetBytes(JsonConvert.SerializeObject(Config)) };
        ServerApi.Network.GetChannel("RepairMeChannel").SendPacket(packet, player);
        
        ServerApi.Logger.Debug($"Sending Packet with Values: {Config.ToolRestoreAsPercent}");
    }
}