using System;
using System.Text.Json;
using System.Threading;
namespace cnode
{
    class Program
    {
        private const string Const_EncryptKey = "key-password-string";
        private const bool Const_EncryptionActivated = false;

        private static bool LightNodeActive = false;
        private static bool EmptyTimerActive = false;
        private static bool CryptoTimerActive = false;
        private static Notus.Kernel.Common.ClassSetting NodeSettings = new Notus.Kernel.Common.ClassSetting();

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine((e.ExceptionObject as Exception).Message, "Unhandled UI Exception");
            Console.WriteLine(sender.ToString());
            Console.WriteLine("press enter to continue");
            Console.ReadLine();
            // here you can log the exception ...
        }
        static void LoadOrGenerateNodeWallet()
        {
            try
            {
                Notus.Kernel.Function.NodeFolderControl();
                
                using (
                    Notus.Kernel.Mempool ObjMp_Node = new Notus.Kernel.Mempool(
                        "local_db" + 
                        System.IO.Path.DirectorySeparatorChar + 
                        Notus.Core.Function.NetworkTypeStr(NodeSettings.Network) + "node_settings"
                    )
                )
                {
                    ObjMp_Node.AsyncActive = false;
                    
                    using (Notus.Kernel.Encryption.Cipher Obj_Cipher = new Notus.Kernel.Encryption.Cipher())
                    {
                        string NodeKeyStr = ObjMp_Node.Get("node_key", "");
                        if (NodeKeyStr.Length == 0)
                        {

                            NodeSettings.NodeWallet = Notus.Core.Wallet.ID.GenerateKeyPair(NodeSettings.Network);
                            if (Const_EncryptionActivated == true)
                            {
                                ObjMp_Node.Set("node_key",
                                    Obj_Cipher.Encrypt(
                                        JsonSerializer.Serialize(NodeSettings.NodeWallet), "", NodeSettings.EncryptKey, NodeSettings.EncryptKey
                                    ),
                                    true
                                );
                            }
                            else
                            {
                                ObjMp_Node.Set("node_key", JsonSerializer.Serialize(NodeSettings.NodeWallet), true);
                            }
                        }
                        else
                        {
                            if (Const_EncryptionActivated == true)
                            {
                                NodeSettings.NodeWallet = JsonSerializer.Deserialize<Notus.Core.Variable.EccKeyPair>(
                                    Obj_Cipher.Decrypt(NodeKeyStr, "", NodeSettings.EncryptKey, NodeSettings.EncryptKey)
                                );
                            }
                            else
                            {
                                NodeSettings.NodeWallet = JsonSerializer.Deserialize<Notus.Core.Variable.EccKeyPair>(NodeKeyStr);
                            }
                        }
                    }
                }
            }
            catch (Exception err)
            {
                Console.WriteLine("hata olustu");
                Console.WriteLine(err.Message);
                Console.WriteLine("hata olustu");
                Console.WriteLine();
                Console.ReadLine();
            }
        }
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
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            NodeSettings.InfoMode = true;
            NodeSettings.DebugMode = true;

            NodeSettings.EncryptMode = Const_EncryptionActivated;
            NodeSettings.HashSalt = Notus.Core.Function.GenerateSalt();
            NodeSettings.EncryptKey = Const_EncryptKey;

            NodeSettings.Network = Notus.Core.Variable.NetworkType.Const_MainNetwork;
            NodeSettings.NodeType = Notus.Kernel.Variable.Constant.NetworkNodeType.Suitable;

            NodeSettings.PrettyJson = true;
            NodeSettings.GenesisAssigned = false;

            NodeSettings.WaitTickCount = 4;
            //AppDomain.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);

            if (args.Length > 0)
            {
                NodeSettings.DebugMode = false;
                for (int a = 0; a < args.Length; a++)
                {
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
                        NodeSettings.NodeType = Notus.Kernel.Variable.Constant.NetworkNodeType.Replicant;
                    }
                    if (string.Equals(args[a], "--test-network"))
                    {
                        NodeSettings.Network = Notus.Core.Variable.NetworkType.Const_TestNetwork;
                    }
                    if (string.Equals(args[a], "--main"))
                    {
                        NodeSettings.NodeType = Notus.Kernel.Variable.Constant.NetworkNodeType.Main;
                    }
                    if (string.Equals(args[a], "--master"))
                    {
                        NodeSettings.NodeType = Notus.Kernel.Variable.Constant.NetworkNodeType.Master;
                    }
                    if (string.Equals(args[a], "--debug"))
                    {
                        NodeSettings.DebugMode = true;
                    }
                    if (string.Equals(args[a], "--info"))
                    {
                        NodeSettings.InfoMode = true;
                    }
                }
            }

            LoadOrGenerateNodeWallet();

            if (NodeSettings.NodeType != Notus.Kernel.Variable.Constant.NetworkNodeType.Replicant)
            {
                LightNodeActive = false;
            }
            Notus.Network.Node.Start(NodeSettings, EmptyTimerActive, CryptoTimerActive, LightNodeActive);
        }
    }
}
