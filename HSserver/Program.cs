using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ProtoBuf;
using System.Linq;

namespace HSserver
{
    public class Program
    {
        [ProtoContract]
        public class SubmitScore : IComparable
        {
            [ProtoMember(1)]
            public string Name { get; set; }
            [ProtoMember(2)]
            public Int64 Score { get; set; }
            [ProtoMember(3)]
            public Int32 LevelHashCode { get; set; }

            public override bool Equals(object obj)
            {
                SubmitScore s = (SubmitScore)obj;
                return this.Score == s.Score;
            }


            public int CompareTo(object obj)
            {
                SubmitScore s = (SubmitScore)obj;
                return s.Score.CompareTo(this.Score);
            }
        }
        [ProtoContract]
        public class HighScores
        {
            [ProtoMember(1)]
            public List<SubmitScore> TopTen = new List<SubmitScore>();//scores


        }
        public static TcpListener tcpListener;

        public static HighScores MyScores;
        static DateTime lastCommit;

        static void Main(string[] args)
        {
            MyScores = LoadScores();

            Program p = new Program();
            tcpListener = new TcpListener(IPAddress.Any, 2593);
            tcpListener.Start();
            
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(p.AcceptClientConnection), tcpListener);
            while (true)
            {
                Thread.Sleep(5000);
                SaveScores();
            }
        }

        private static string data = "Data.dat";

        private static void SaveScores()
        {

            if (MyScores.TopTen.Count == 0)
                return;
            if ((DateTime.Now - lastCommit).TotalHours < 10)
                return;
            
            try
            {
                lock (MyScores.TopTen)
                {
                    using (var fs = File.Create(data))
                        Serializer.SerializeWithLengthPrefix<HighScores>(fs, MyScores, PrefixStyle.Base128);

                    using (var fs = File.Open(data, FileMode.Open))
                    {
                        var x = Serializer.DeserializeWithLengthPrefix<HighScores>(fs, PrefixStyle.Base128);
                        if (x.TopTen.Count > 0)
                            using (var ws = File.Open(data + ".bak", FileMode.Create))
                                Serializer.SerializeWithLengthPrefix<HighScores>(ws, MyScores, PrefixStyle.Base128);
                    }
                }
            }
            catch(Exception e){
                Console.WriteLine(e.Message);
            }
            lastCommit = DateTime.Now;

        }
        private static HighScores LoadScores()
        {
            lastCommit = DateTime.Now;
            if (File.Exists(data))
            {
                try
                {
                    using(var fs = File.OpenRead(data))
                    return Serializer.DeserializeWithLengthPrefix<HighScores>(fs, PrefixStyle.Base128);
                }
                catch {
                    try
                    {
                        File.Delete(data);
                        File.Copy(data + ".bak", data);
                        using (var fs = File.OpenRead(data))
                            return Serializer.DeserializeWithLengthPrefix<HighScores>(fs, PrefixStyle.Base128);
                    }
                    catch { return new HighScores(); }
                    
                }
            }
            else {
                return new HighScores();
            }

        }


        private void AcceptClientConnection(IAsyncResult ar)
        {
            TcpListener client = (TcpListener)ar.AsyncState;
            var ClientComThread = new Thread(new ParameterizedThreadStart(HandleClientCom));
            TcpClient UOClient = client.EndAcceptTcpClient(ar);
            ClientComThread.Start(UOClient);
            client.BeginAcceptTcpClient(new AsyncCallback(this.AcceptClientConnection), tcpListener);
        }

        private void HandleClientCom(object obj)
        {
            TcpClient client = (TcpClient)obj;
            NetworkStream ClientStream = client.GetStream();
            System.Diagnostics.Stopwatch st = new System.Diagnostics.Stopwatch();
            Console.WriteLine("Client Connected, Awaiting Data");
            while (client.Connected)
            {
                Thread.Sleep(100);
                if(client.Available > 0)
                {
                    var score = Serializer.DeserializeWithLengthPrefix<SubmitScore>(client.GetStream(),PrefixStyle.Base128);
                    Console.WriteLine(score.Name + " Submitted " + score.Score);
                    MyScores.TopTen.Add(score);
                    
                    HighScores hs = new HighScores();
                    hs.TopTen.AddRange(GetTopTen(score.LevelHashCode));
                    Serializer.SerializeWithLengthPrefix<HighScores>(client.GetStream(), hs, PrefixStyle.Base128);
                }
                if (st.ElapsedMilliseconds > 25000)
                    break;
            }
        }

        private IEnumerable<SubmitScore> GetTopTen(int p)
        {
            SubmitScore[] ar = new SubmitScore[Math.Min(MyScores.TopTen.Count, 10)];
            var x = MyScores.TopTen.Where(a => a.LevelHashCode == p).ToList();
            x.Sort();
            for (int i = 0; i < Math.Min(x.Count(), 10); i++)
                ar[i] = x[i];//ar[i] = MyScores.TopTen[i];
            return ar;
        }

        
    }
}
