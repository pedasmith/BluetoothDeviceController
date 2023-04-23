using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothDeviceController.SpecialtyPages
{
    public interface IHandleStatus
    {
        void SetStatusText(string text);
        void SetStatusActive(bool isActive);
    }

    public interface ISetHandleStatus
    {
        void SetHandleStatus(IHandleStatus handleStatus);
    }
}
