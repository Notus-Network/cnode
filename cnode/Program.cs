﻿using System;
using System.Runtime.ExceptionServices;
using System.Text.Json;
namespace cnode
{
    class Program
    {
        private static bool LightNodeActive = false;
        private static bool EmptyTimerActive = false;
        private static bool CryptoTimerActive = false;
        private static Notus.Variable.Common.ClassSetting NodeSettings = new Notus.Variable.Common.ClassSetting();

        private static void FirstChanceExceptionEventHandler(object sender, FirstChanceExceptionEventArgs e)
        {
            Console.WriteLine(e.Exception.Message, "Unhandled FirstChanceExceptionEventArgs Exception");
            Console.WriteLine(sender.ToString());
            Console.WriteLine("press enter to continue");
            Console.ReadLine();
        }
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine((e.ExceptionObject as Exception).Message, "Unhandled UnhandledExceptionEventArgs Exception");
            Console.WriteLine(sender.ToString());
            Console.WriteLine("press enter to continue");
            Console.ReadLine();
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            /*
                sunucu durumunu veriyor
                http://94.101.87.42:5000/online    

                son blok içeriğini veriyor
                http://94.101.87.42:5000/block/last
                
                son blok hakkında bilgi veriyor
                http://94.101.87.42:5000/block/summary    

                row değeri verilen blok içeriğini veriyor
                http://94.101.87.42:5000/metrics/node
                http://94.101.87.42:5000/metrics/master
                http://94.101.87.42:5000/metrics/main
                http://94.101.87.42:5000/metrics/replicant
                http://94.101.87.42:5000/metrics/block
            
                row değeri verilen blok hash değerini veriyor
                http://94.101.87.42:5000/block/hash/1
                
                UID değerinin verildiği blok hash değerini veriyor
                http://94.101.87.42:5000/block/hash/100000000000000000000000000000000000000000000000000000000000000000000000000000000000000000

                row değeri verilen blok içeriğini veriyor
                http://94.101.87.42:5000/block/1
                
                UID değerinin verildiği blok içeriğini veriyor
                http://94.101.87.42:5000/block/100000000000000000000000000000000000000000000000000000000000000000000000000000000000000000

                //tüm nodeların listelerini veriyor
                http://94.101.87.42:5000/node

                cüzdan bakiyesini veriyor
                http://94.101.87.42:5000/balance/NRCqcJDQEy8Y5FewBvJjn2UV2AqsaMxGDzksBd
                http://94.101.87.42:5000/balance/_Cüzdan_Adresi

                transfer işleminin durumunu kontrol ediyor...
                http://94.101.87.42:5000/transaction/status/13489f62ef340cb3edfaa162fc0a5ab65c45b89e320574646584ca04d4cfd0e866f4e3bd836079cc46b762b8f5
                http://94.101.87.42:5000/transaction/status/_Kayıt_Esnasında_Verilen_Uid_Değeri
            */

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            using (Notus.Validator.Menu menuObj = new Notus.Validator.Menu())
            {
                NodeSettings = menuObj.PreStart(args);

                menuObj.Start();
                NodeSettings = menuObj.DefineMySetting(NodeSettings);
                Notus.Toolbox.IO.NodeFolderControl(NodeSettings.Network, NodeSettings.Layer);
            }
            if (NodeSettings.NodeType != Notus.Variable.Enum.NetworkNodeType.Replicant)
            {
                LightNodeActive = false;
            }
            CryptoTimerActive = true;

            Console.WriteLine(JsonSerializer.Serialize(NodeSettings, new JsonSerializerOptions() { WriteIndented = true }));
            Console.ReadLine();

            if (NodeSettings.DevelopmentNode == true)
            {
                NodeSettings.Network = Notus.Variable.Enum.NetworkType.DevNet;

                Notus.Validator.Node.Start(NodeSettings, EmptyTimerActive, CryptoTimerActive, LightNodeActive);
            }
        }
    }
}