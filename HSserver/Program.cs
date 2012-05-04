using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ProtoBuf;


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
            public List<SubmitScore> TopTen = new List<SubmitScore>();


        }
        public static TcpListener tcpListener;

        public static List<SubmitScore> MyScores;
        static void Main(string[] args)
        {
            MyScores = new List<SubmitScore>();
            Program p = new Program();
            tcpListener = new TcpListener(IPAddress.Any, 2593);
            tcpListener.Start();
            
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(p.AcceptClientConnection), tcpListener);
            while (true)
                Thread.Sleep(50);
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
            while (client.Connected)
            {
                Thread.Sleep(25);
                if(client.Available > 0)
                {
                    var score = Serializer.DeserializeWithLengthPrefix<SubmitScore>(client.GetStream(),PrefixStyle.Base128);
                    MyScores.Add(score);
                    MyScores.Sort();
                    HighScores hs = new HighScores();
                    hs.TopTen.AddRange(GetTopTen());
                    Serializer.SerializeWithLengthPrefix<HighScores>(client.GetStream(), hs, PrefixStyle.Base128);
                }
            }
        }

        private SubmitScore[] GetTopTen()
        {
            SubmitScore[] ar = new SubmitScore[Math.Min(MyScores.Count, 10)];
            MyScores.Sort();
            for (int i = 0; i < Math.Min(MyScores.Count,11); i++)
                ar[i] = MyScores[i];
            return ar;
        }
    }
}
