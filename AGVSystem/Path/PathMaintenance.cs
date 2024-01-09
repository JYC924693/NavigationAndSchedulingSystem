using AGVSystem.Models;

namespace AGVSystem.Path
{
    public class QRCode
    {
        public int ID { get; set; }
        public double angle { get; set; }
        public int mode { get; set; }
        public int task { get; set; }
        public QRCode() { } 
        public QRCode(QRCode other)
        {
            ID = other.ID;
            angle = other.angle;
            mode = other.mode;
            task = other.task;
        }  
    }
    public class PathMaintenance
    {
        private static readonly ConcreteMap map = MapContext.GetInstance();
        private readonly List<QRCode> _path = [];
        public PathMaintenance() { }

        /// <summary>
        /// 前端会调用方法来调用GetQRCode获取二维码数据
        /// 根据二维码ID从图中获取二维码数据
        /// </summary>
        public void GetQRCode(int QR_ID)
        {
            QRCode qRCode = new QRCode();
            // 通过map获取图中二维码数据 赋给qRCode

            _path.Add(qRCode);
        }

        /// <summary>
        /// 前端选择二维码后 点击生成路径
        /// GeneratePath中会判断是否能生成路径 最后返回排好序的路径
        /// </summary>
        public void GenenratePath()
        {
            JudgeQRCodesIsPath();
        }

        /// <summary>
        /// 在图中查询两个二维码之间是否有边 确定两个节点是否是邻接点
        /// </summary>
        /// <param name="previous"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public static bool CheckEdgeBetweenQRs(int previous, int next)
        {
            return true;
        } 

        /// <summary>
        /// 判断一串二维码是否能构成一条路径
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public List<QRCode> JudgeQRCodesIsPath()
        {
            int path_length = _path.Count;                                  // 路径的长度
            List<QRCode > temp = new List<QRCode>();                        // 二维码排好序的路径
            QRCode start = new QRCode(_path[0]);                            // 初始化起点 并赋值
            QRCode end = new QRCode(_path[path_length - 1]);                // 初始化终点 并赋值
            temp.Add(start);                                                // temp中插入起点
            _path.RemoveAt(0);                                              // 删除_path的起点

            while (_path.Count > 0)
            {
                if (temp.Count == 0)
                {
                    throw new InvalidOperationException("temp is empty!");
                }

                for (int i = 0; i < _path.Count; i++)
                {
                    int previous_QR = temp[temp.Count - 1].ID;              // 前一个二维码id
                    int next_QR = _path[i].ID;                              // 后一个二维码id
                    if (CheckEdgeBetweenQRs(previous_QR, next_QR))
                    {
                        temp.Insert(temp.Count - 1, _path[i]);
                        _path.RemoveAt(i);
                    }
                    else
                    {
                        if(i == _path.Count -1)
                        {
                            throw new InvalidOperationException("二维码不能组成一条路径!");
                        }
                    }  
                }
            }
            if (temp[temp.Count-1] == end)
            {
                return temp;
            }
            else
            {
                return new List<QRCode>();
            }
        }

        /// <summary>
        /// 将_path保存至SQL中
        /// </summary>
        public void SavePath()
        {

        }

    }
}
