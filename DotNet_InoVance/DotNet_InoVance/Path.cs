using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace modbus_inovance
{
    public class Path
    {
        public List<int> IdSequence { get; set; }

        public List<AgvState> MoveSequence { get; set; }

        public List<double> AngleSequence { get; set; }

        public List<TaskMode> TaskSequence { get; set; }
    }

    public enum AgvState
    {
        Left = -30,
        Backwark = -1,
        Forward = 1,
        Right = 30,
    }

    public enum TaskMode
    {
        None = 0,
    }
}
