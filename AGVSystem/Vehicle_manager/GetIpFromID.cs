namespace AGVSystem.vehicle_manager
{
    public class GetIpFromID
    {
        int AgvID;
        GetIpFromID(int agvID)
        {
            AgvID = agvID;
        }

        string GetIp(List<AgvInfo> connectedClients)
        {
            AgvInfo targetAGV = new AgvInfo();
            targetAGV = connectedClients.Find(agv => agv.AgvId == AgvID);

            return targetAGV.IpAddress;
        }
    }
}
