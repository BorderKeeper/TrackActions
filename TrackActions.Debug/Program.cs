using System;
using System.Collections.Generic;
using System.Threading;
using TrackActions.Core.Logitech;
using TrackActions.Core.TrackIR;

namespace TrackActions.Debug
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Point> keyboardPoints = new List<Point>();

            TrackIrClient client = new TrackIrClient();

            client.TrackIR_Enhanced_Init();

            bool looking = false;
            Console.WriteLine($"Init is: {LogitechGSDK.LogiLedInit()}");
            LogitechGSDK.LogiLedSaveCurrentLighting();

            while (true)
            {
                var output = client.client_HandleTrackIRData();

                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey().Key;

                    switch (key)
                    {
                        case ConsoleKey.N:
                            if (keyboardPoints.Count > 1)
                            {
                                Console.WriteLine("2 points added");
                                break;
                            }

                            Console.WriteLine($"Point added: [{output.fNPPitch}, {output.fNPYaw}]");
                            keyboardPoints.Add(new Point
                            {
                                Pitch = output.fNPPitch,
                                Yaw = output.fNPYaw
                            });
                            break;
                        case ConsoleKey.Q:
                            client.TrackIR_Shutdown();
                            LogitechGSDK.LogiLedShutdown();
                            return;
                    }
                }

                if (keyboardPoints.Count > 1)
                {
                    var topLeft = keyboardPoints[0];
                    var bottomRight = keyboardPoints[1];

                    var pitch = output.fNPPitch;
                    var yaw = output.fNPYaw;

                    if (pitch > topLeft.Pitch && pitch < bottomRight.Pitch &&
                        yaw > bottomRight.Yaw && yaw < topLeft.Yaw)
                    {
                        Console.WriteLine("Looking at keyboard");

                        if (!looking)
                        {
                            Console.WriteLine($"Turning kb on: {LogitechGSDK.LogiLedSetLighting(1, 1, 1)}");
                            //Console.WriteLine($"Restoring: {LogitechController.LogiLedRestoreLighting()}");

                            looking = true;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Not looking at keyboard");

                        if (looking)
                        {
                            Console.WriteLine($"Turning kb off: {LogitechGSDK.LogiLedSetLighting(0, 0, 0)}");
                            //Console.WriteLine($"Saving color: {LogitechController.LogiLedSaveCurrentLighting()} | Turning kb off: {LogitechController.LogiLedSetLighting(0, 0, 0)}");

                            looking = false;
                        }
                    }
                }

                Thread.Sleep(100);
            }
        }
    }

    class Point
    {
        public float Pitch { get; set; }

        public float Yaw { get; set; }
    }
}
