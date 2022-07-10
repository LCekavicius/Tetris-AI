using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrisThing
{
    public class Bag
    {
        private Random rand = new Random();
        public List<Piece> CurrentBag { get; private set; }
        public List<Piece> NextBag { get; private set; }

        private List<Piece> CurrentBagSave { get; set; }
        private List<Piece> NextBagSave { get; set; }
        private Random randSave { get; set; }


        public Bag()
        {
            CurrentBag = GetRandomBag();
            CurrentBag.AddRange(GetRandomBag());
            NextBag = GetRandomBag();
        }

        public void Save()
        {
            var temp = new Piece[CurrentBag.Count];
            CurrentBag.CopyTo(temp, 0);
            CurrentBagSave = temp.ToList();

            temp = new Piece[NextBag.Count];
            NextBag.CopyTo(temp, 0);
            NextBagSave = temp.ToList();

            randSave = rand;

        }
        public void Load()
        {
            var temp = new Piece[CurrentBagSave.Count];
            CurrentBagSave.CopyTo(temp, 0);
            CurrentBag = temp.ToList();

            temp = new Piece[NextBagSave.Count];
            NextBagSave.CopyTo(temp, 0);
            NextBag = temp.ToList();

            rand = randSave;
        }

        public Piece GetPiece()
        {
            if(CurrentBag.Count == 7)
                CycleBag();

            var ret = CurrentBag.First();
            CurrentBag.RemoveAt(0);
            return ret;
        }

        private void CycleBag()
        {
            CurrentBag.AddRange(NextBag);
            NextBag = GetRandomBag();
        }

        private List<Piece> GetRandomBag()
        {
            List<Piece> Bag = new List<Piece>()
            {
                new IPiece(),
                new JPiece(),
                new LPiece(),
                new OPiece(),
                new SPiece(),
                new TPiece(),
                new ZPiece()
            };

            return Bag.OrderBy(x => rand.Next()).ToList();
        }
    }
}
