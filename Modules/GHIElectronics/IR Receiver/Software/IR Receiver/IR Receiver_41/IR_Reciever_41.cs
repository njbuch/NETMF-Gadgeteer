﻿using System;
using Microsoft.SPOT;

using GTM = Gadgeteer.Modules;

using Microsoft.SPOT.Hardware;

namespace Gadgeteer.Modules.GHIElectronics
{

    /// <summary>
    /// A IR_Receiver module for Microsoft .NET Gadgeteer
    /// </summary>
    public class IR_Receiver : GTM.Module
    {
        private static long last_tick;
        private static long bit_time;
        private static uint pattern;
        private static bool streaming;
        private static uint shiftBit;
        private static bool new_press = false;

        /// <summary>
        /// The protocol used for communication.
        /// </summary>
        public enum ReceiverType : byte
        {
            /// <summary>
            /// RC-5 protocol. Only one implemented.
            /// </summary>
            RC5 = 1,

            /// <summary>
            /// RC-6 protocol. Not yet implemented.
            /// </summary>
            RC6 = 2,
        }
        ReceiverType IRReceiverType;

        // Note: A constructor summary is auto-generated by the doc builder.
        /// <summary></summary>
        /// <param name="socketNumber">The socket that this module is plugged in to.</param>
        public IR_Receiver(int socketNumber)
        {
            // This finds the Socket instance from the user-specified socket number.  
            // This will generate user-friendly error messages if the socket is invalid.
            // If there is more than one socket on this module, then instead of "null" for the last parameter, 
            // put text that identifies the socket to the user (e.g. "S" if there is a socket type S)
            Socket socket = Socket.GetSocket(socketNumber, true, this, null);

            last_tick = DateTime.Now.Ticks;
            IRReceiverType = ReceiverType.RC5; // Only one supported at this time

            // This creates an GTI.InterruptInput interface. The interfaces under the GTI namespace provide easy ways to build common modules.
            // This also generates user-friendly error messages automatically, e.g. if the user chooses a socket incompatible with an interrupt input.
            this.input = new InterruptPort(socket.CpuPins[3], false, Port.ResistorMode.Disabled, Port.InterruptMode.InterruptEdgeBoth);//new GTI.InterruptInput(socket, GT.Socket.Pin.Three, GTI.GlitchFilterMode.On, GTI.ResistorMode.Disabled, GTI.InterruptMode.RisingAndFallingEdge, this);

            // This registers a handler for the interrupt event of the interrupt input (which is below)
            this.input.OnInterrupt += new NativeEventHandler(input_OnInterrupt); //new GTI.InterruptInput.InterruptEventHandler(this._input_Interrupt);
        }

        private InterruptPort input;

        void input_OnInterrupt(uint data1, uint data2, DateTime time)
        {
            bit_time = time.Ticks - last_tick;
            last_tick = time.Ticks;
            switch (IRReceiverType)
            {
                case ReceiverType.RC5:
                    /* When testing revealed that the togglebit not work reliably with fast key action,
                     * hence this is resolved by testing for a bit timeout of 100 ms. This gives better results.*/
                    if (bit_time > 1000000) // 100 ms
                        new_press = true;
                    if (bit_time > 26670)  // 3 * halftime (half_bittime = 889 us)
                    {
                        bit_time = 0;
                        pattern = 0;
                        if (data2 == 0)
                        {
                            streaming = true;
                            shiftBit = 1;
                            pattern |= shiftBit;
                        }
                        else
                            streaming = false;
                        return;
                    }
                    if (streaming)
                    {
                        if (bit_time > 10668)  // = half_bittime * 1.2 (half_bittime = 889 us)
                        {
                            if (data2 == 0)
                                shiftBit = 1;
                            else
                                shiftBit = 0;
                            pattern <<= 1;
                            pattern |= shiftBit;
                        }
                        else
                        {
                            if (data2 == 0)
                            {
                                pattern <<= 1;
                                pattern |= shiftBit;
                            }
                        }
                        if ((pattern & 0x2000) > 0)  // 14 bits
                        {
                            if (new_press)
                            {
                                IREventArgs _args = new IREventArgs();
                                _args.Button = pattern & 0x3F;
                                _args.ReadTime = DateTime.Now;
                                OnIREvent(_args);
                                new_press = false;
                            }
                            pattern = 0;
                            bit_time = 0;
                            streaming = false;
                        }
                    }
                    break;
                case ReceiverType.RC6:
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void IREventDelegate(object sender, IREventArgs e);

        /// <summary>
        /// 
        /// </summary>
        public event IREventDelegate IREvent;

        /// <summary>
        /// 
        /// </summary>
        public class IREventArgs : EventArgs
        {
            /// <summary>
            /// The button what was pressed.
            /// </summary>
            public uint Button { get; set; }

            /// <summary>
            /// The time that the button was read.
            /// </summary>
            public DateTime ReadTime { get; set; }
        }

        private void OnIREvent(IREventArgs e)
        {
            if (IREvent != null)
            {
                IREvent(this, e);
            }
        }
    }
}