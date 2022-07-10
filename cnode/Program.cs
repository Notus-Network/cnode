using System;
using System.Runtime.ExceptionServices;
using System.Text.Json;
namespace cnode
{
    class Program
    {
        private const string Const_EncryptKey = "key-password-string";
        private const bool Const_EncryptionActivated = false;

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
            //AppDomain.CurrentDomain.MonitoringTotalAllocatedMemorySize
            //AppDomain.CurrentDomain.MonitoringSurvivedMemorySize
            //AppDomain.CurrentDomain.MonitoringTotalProcessorTime
            //AppDomain.CurrentDomain.FirstChanceException += FirstChanceExceptionEventHandler;

            NodeSettings.LocalNode = true;
            NodeSettings.InfoMode = true;
            NodeSettings.DebugMode = true;


            NodeSettings.EncryptMode = Const_EncryptionActivated;
            NodeSettings.HashSalt = Notus.Encryption.Toolbox.GenerateSalt();
            NodeSettings.EncryptKey = Const_EncryptKey;

            NodeSettings.SynchronousSocketIsActive = true;
            NodeSettings.Layer = Notus.Variable.Enum.NetworkLayer.Layer1;
            NodeSettings.Network = Notus.Variable.Enum.NetworkType.MainNet;
            NodeSettings.NodeType = Notus.Variable.Enum.NetworkNodeType.Suitable;

            NodeSettings.PrettyJson = true;
            NodeSettings.GenesisAssigned = false;

            NodeSettings.WaitTickCount = 4;

            NodeSettings.DevelopmentNode = false;
            NodeSettings.NodeWallet = new Notus.Variable.Struct.EccKeyPair()
            {
                CurveName = "",
                PrivateKey = "",
                PublicKey = "",
                WalletKey = "",
                Words = new string[] { },
            };
            NodeSettings.Port = new Notus.Variable.Struct.CommunicationPorts(){
                MainNet =0,
                TestNet=0,
                DevNet=0
            };

        CheckParameter(args);

            Notus.Toolbox.IO.NodeFolderControl(NodeSettings.Network, NodeSettings.Layer);

            if (NodeSettings.NodeType != Notus.Variable.Enum.NetworkNodeType.Replicant)
            {
                LightNodeActive = false;
            }
            CryptoTimerActive = true;
            Notus.Validator.Node.Start(NodeSettings, EmptyTimerActive, CryptoTimerActive, LightNodeActive);
        }
        static void CheckParameter(string[] args)
        {
            if (args.Length > 0)
            {
                //NodeSettings.DebugMode = false;
                for (int a = 0; a < args.Length; a++)
                {
                    if (string.Equals(args[a], "--testnet"))
                    {
                        NodeSettings.Network = Notus.Variable.Enum.NetworkType.TestNet;
                    }
                    if (string.Equals(args[a], "--mainnet"))
                    {
                        NodeSettings.Network = Notus.Variable.Enum.NetworkType.MainNet;
                    }
                    if (string.Equals(args[a], "--devnet"))
                    {
                        NodeSettings.Network = Notus.Variable.Enum.NetworkType.DevNet;
                    }


                    if (string.Equals(args[a], "--empty"))
                    {
                        EmptyTimerActive = true;
                    }
                    if (string.Equals(args[a], "--crypto"))
                    {
                        CryptoTimerActive = true;
                    }
                    if (string.Equals(args[a], "--light"))
                    {
                        LightNodeActive = true;
                    }


                    if (string.Equals(args[a], "--replicant"))
                    {
                        NodeSettings.NodeType = Notus.Variable.Enum.NetworkNodeType.Replicant;
                    }
                    if (string.Equals(args[a], "--main"))
                    {
                        NodeSettings.NodeType = Notus.Variable.Enum.NetworkNodeType.Main;
                    }
                    if (string.Equals(args[a], "--master"))
                    {
                        NodeSettings.NodeType = Notus.Variable.Enum.NetworkNodeType.Master;
                    }


                    if (string.Equals(args[a], "--debug"))
                    {
                        NodeSettings.DebugMode = true;
                    }
                    if (string.Equals(args[a], "--info"))
                    {
                        NodeSettings.InfoMode = true;
                    }


                    if (string.Equals(args[a], "--layer1"))
                    {
                        NodeSettings.Layer = Notus.Variable.Enum.NetworkLayer.Layer1;
                    }
                    if (string.Equals(args[a], "--layer2"))
                    {
                        NodeSettings.Layer = Notus.Variable.Enum.NetworkLayer.Layer2;
                    }
                    if (string.Equals(args[a], "--layer3"))
                    {
                        NodeSettings.Layer = Notus.Variable.Enum.NetworkLayer.Layer3;
                    }
                    if (string.Equals(args[a], "--layer4"))
                    {
                        NodeSettings.Layer = Notus.Variable.Enum.NetworkLayer.Layer4;
                    }
                    if (string.Equals(args[a], "--layer5"))
                    {
                        NodeSettings.Layer = Notus.Variable.Enum.NetworkLayer.Layer5;
                    }
                    if (string.Equals(args[a], "--layer6"))
                    {
                        NodeSettings.Layer = Notus.Variable.Enum.NetworkLayer.Layer6;
                    }
                    if (string.Equals(args[a], "--layer7"))
                    {
                        NodeSettings.Layer = Notus.Variable.Enum.NetworkLayer.Layer7;
                    }
                    if (string.Equals(args[a], "--layer8"))
                    {
                        NodeSettings.Layer = Notus.Variable.Enum.NetworkLayer.Layer8;
                    }
                    if (string.Equals(args[a], "--layer9"))
                    {
                        NodeSettings.Layer = Notus.Variable.Enum.NetworkLayer.Layer9;
                    }
                    if (string.Equals(args[a], "--layer10"))
                    {
                        NodeSettings.Layer = Notus.Variable.Enum.NetworkLayer.Layer10;
                    }
                }

                if (NodeSettings.Layer != Notus.Variable.Enum.NetworkLayer.Layer1)
                {
                    CryptoTimerActive = false;
                    EmptyTimerActive = false;
                }
            }
            else
            {
                Console.WriteLine(JsonSerializer.Serialize(NodeSettings, new JsonSerializerOptions() { WriteIndented = true }));
                Console.ReadLine();
                //Notus.Variable.Struct.NodeInfo Settings;
                using (Notus.Validator.Menu menuObj = new Notus.Validator.Menu())
                {
                    menuObj.Start();
                    NodeSettings=menuObj.DefineMySetting(NodeSettings);
                    //Settings = menuObj.Settings;
                }
                Console.WriteLine(JsonSerializer.Serialize(NodeSettings, new JsonSerializerOptions() { WriteIndented = true }));

                Console.ReadLine();
            }
        }
    }
}