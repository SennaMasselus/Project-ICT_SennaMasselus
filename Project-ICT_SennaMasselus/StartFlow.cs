using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_ICT_SennaMasselus
{
    internal class StartFlow
    {
        public void Start_Flow(SerialPort port)
        {
            // Check if the serial port is open
            if (port.IsOpen)
            {
                //zet D3 van de Nucleo aan, dit is verbonden aan een ledje.
                port.Write("\u0002");
            }
        }
    }
}