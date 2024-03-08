namespace AGVSystem.Traffic
{
    public class AgvInfo(int ID)
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int AgvID { get; set; } = ID;
        /// <summary>
        /// 名称
        /// </summary>
        public string AgvName { get; set; }
        /// <summary>
        /// 任务名称 绑定
        /// </summary>
        public string TaskName { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public enum StateType
        {
            Free = 0,
            Occupy = 1,
            Charging = 2,
            TroubleShooting = 3
        }
        /// <summary>
        /// 类型
        /// </summary>
        public int AgvType { get; set; }
        /// <summary>
        /// IP地址
        /// </summary>
        public string IPAddress { get; set; }
        /// <summary>
        /// 端口号
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// 取货点
        /// </summary>
        public string Source { get; set; }
        /// <summary>
        /// 送货点
        /// </summary>
        public string Target { get; set; }
        /// <summary>
        /// 当前位置
        /// </summary>
        public int Position { get; set; }
        /// <summary>
        /// 电量
        /// </summary>
        public float ElectricQuantity { get; set; }
        /// <summary>
        /// 电压
        /// </summary>
        public float Voltage { get; set; }
        /// <summary>
        /// 速度
        /// </summary>
        public float Speed { get; set; }
    }

    public class CommunicationInfo
    {
        public int AgvID;
        public string IPAddress;
        public int Port;
    }
}
