using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Resources;

namespace TheGameOfGovernment
{
    class Program
    {
	///<summary>
	/// These colors aren't work well on consoles with white background
	/// </summary>
	public static bool IsOutputColorized = false;

        static void Main(string[] args)
        {
	    /// Uncomment one of the following lines to redefine system settings:
	    //System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo ("en");
	    //System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo ("ru");
            Game g = Input.Read();
            while(true)
            {
                g.Next();
                Console.ReadKey();
            }
        }
    }

    internal class Input
    {
        public static Game Read()
        {
	    Console.Write(GoG_Resources.GOODS_COUNT);
            int count = Convert.ToInt32(Console.ReadLine());
            List<Goods> goods = new List<Goods>();
            List<Country> countries = new List<Country>();
            for (int i = 0; i < count; i++)
            {
		if (Program.IsOutputColorized)
		    Console.BackgroundColor = ConsoleColor.DarkGray; 
		string message = string.Format (GoG_Resources.GOOD_WITH_NUM, i + 1);
		Console.WriteLine(message);
		if (Program.IsOutputColorized)
                    Console.BackgroundColor = ConsoleColor.Black;
                goods.Add(Goods.Input());
                countries.Add(Country.Input());
            }

            return new Game(countries) { Goods = goods};
        }
    }

    public class Goods
    {
        public string Name { get; set; }
        public int Coast { get; set; }
        public int Actuality { get; set; }

        public static Goods Input()
        {
            
            Goods g = new Goods();
	    Console.Write(GoG_Resources.GOOD_NAME);
            g.Name = Console.ReadLine();

	    Console.Write(GoG_Resources.GOOD_IMPORTANCE);
            g.Actuality = Convert.ToInt32(Console.ReadLine());

	    Console.Write(GoG_Resources.GOOD_PRICE);
            g.Coast = Convert.ToInt32(Console.ReadLine());

            return g;
            
        }
    }

    public class Country
    {
        public int Money { get; set; }
        public Game Game { get; set; }
        public int[] GoodsAmount { get; set; }
        public int Earned { get; set; }
        public int Spent { get; set; }
        public static Country Input()
        {
	    Console.Write(GoG_Resources.BUDGET_INITIAL);
            
            return new Country() { Money = Convert.ToInt32(Console.ReadLine()) };
        }

        internal void Next()
        {
            if (GoodsAmount == null)
                GoodsAmount = new int[Game.Goods.Count];
            for (int i = 0; i < GoodsAmount.Length; i++)
            {
                if (i == Game.GetId(this))
                    continue;
                if(GoodsAmount[i]==0)
                {
                    Game.Buy(this, i);
                }
                GoodsAmount[i]--;
            }
        }

        public void EndOfDay()
        {
            Earned = 0;
            Spent = 0;
        }

        public string DayString()
        {
	    return Game.SWL(string.Format("{0}", Earned), 4) + Game.SWL(string.Format("{0}", Spent), 4) + Game.SWL(string.Format("{0}", Money), 4);
        }

        public void Buy(Goods g)
        {

        }

        internal void Sell(int i)
        {
            Earned += Game.Goods[i].Coast;
            Money += Game.Goods[i].Coast;
        }
    }

       public class Game
    {
        public List<Country> Countries { get; set; }
        public List<Goods> Goods { get; set; }

        public Game(List<Country> c)
        {
            Countries = c;
            foreach (var cc in c)
                cc.Game = this;

        }

        public void Next()
        {
            foreach (var c in Countries)
                c.Next();

            if(Day%15==0)
            {
		if (Program.IsOutputColorized)
		    Console.BackgroundColor = ConsoleColor.DarkYellow;
		Console.Write(SWL(String.Empty, 4));
                foreach (var g in Goods)
                {
                    Console.Write(SWL(g.Name, 12));
                }
                Console.WriteLine();
            }
            Day++;

	    if (Program.IsOutputColorized)
                Console.BackgroundColor = ConsoleColor.DarkRed;
	    Console.Write(SWL(String.Format("{0}", Day), 4));
	    if (Program.IsOutputColorized)
                Console.BackgroundColor = ConsoleColor.Black;
            foreach(var c in Countries)
                Console.Write(c.DayString());

            foreach (var c in Countries)
                c.EndOfDay();
            Console.WriteLine();
        }

        public string SWL(string s, int l) // string with length
        {
            if (s.Length < l) s =  new string(' ', l - s.Length ) + s ;
            return s;
        }

        public int Day { get; set; }

        public int Buy(Country c, int i)
        {
            c.Money -= Goods[i].Coast;
            c.Spent += Goods[i].Coast;
            c.GoodsAmount[i] += Goods[i].Actuality;
            Countries[i].Sell(i);

            return Goods[i].Coast;
        }

        public int GetId(Country c)
        {
            return Countries.IndexOf(c);
        }
    }
}
