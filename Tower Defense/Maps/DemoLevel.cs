using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;
namespace Tower_Defense.Maps
{
    [ProtoContract]
    public class LevelDef
    {
        [ProtoMember(1)]
        public int WavesCount = 0;
        [ProtoMember(2)]
        public List<wave> Waves = new List<wave>();
        public void populate()
        {
            Type[] types = new Type[] { typeof(Monsters.Runner), typeof(Monsters.Tank), typeof(Monsters.Tank), typeof(Monsters.Runner), typeof(Monsters.Runner) };
            for (int i = 0; i < WavesCount; i++)
            {
                wave temp = new wave(types[i],10 + Helper.random.Next(10),i+1);
                Waves.Add(temp);
            }
        }

    }
    [ProtoContract]
    public class wave
    {
        [ProtoMember(1)]
        public Type MonsterType = typeof(Monsters.Runner);
        [ProtoMember(2)]
        public int NumberOfMobs = 20;
        [ProtoMember(3)]
        public int MobLevel = 1;
        public wave()
        { }
        public wave(Type type, int amount, int level)
        {
            MonsterType = type;
            NumberOfMobs = amount;
            MobLevel = level;
        }

        internal Monster makeMob(Level map,int level)
        {
            Monster m = (Monster)MonsterType.Assembly.CreateInstance(MonsterType.FullName);
            m.SetLevel(level);
            m.initMob(map);
            return m;
        }
    }
}
