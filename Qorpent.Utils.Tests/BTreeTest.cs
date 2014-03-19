using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Qorpent.Utils.Collections.BTreeIndex;

namespace Qorpent.Utils.Tests
{
	[TestFixture]
	public class BTreeTest{
		private string[] strings = new[]{
			"ОКСАНА",		"АНАСТАСИЯ",
			"ДАНИЛА",		"СОФЬЯ",		"РЕНАТ",			"РАДИК",		"АЛЕНА",		"ВИКТОРИЯ",
			"ТАРАС",		"РИШАТ",		"НАИЛЬ",			"ЖАННА",		"ОЛЕГ",			"ИЛЬЯ",
			"РИФАТ",		"РАФИС",		"АЛИНА",			"АЛЕВТИНА",		"ДАРЬЯ",		"ГАЛИНА",
			"ИЛЬГИЗ",		"ИЛЬДУС",		"ВЕНЕРА",			"ЛЕВ",			"АНДРЕЙ",		"РОМАН",
			"АРСЕНИЙ",		"НЕЛЛИ",		"ФАРИТ",			"РАИСА",		"ЕЛЕНА",		"ВИТАЛИЙ",
			"КАМИЛЬ",		"ДИНА",			"УЛЬЯНА",			"ЯКОВ",			"ИГОРЬ",		"АРТЕМ",
			"МАРИАННА",		"МАТВЕЙ",		"РАШИД",			"ЗОЯ",			"ВЛАДИМИР",		"ВАСИЛИЙ",
			"НАИЛЯ",		"ШАМИЛЬ",		"РОБЕРТ",			"ИЛЬДАР",		"НИКОЛАЙ",		"АННА",
			"МАРС",			"ИНГА",			"АЛЬФИЯ",			"ЗИНАИДА",		"ЛИДИЯ",		"ЛЮДМИЛА",
			"ИЛДАР",		"ДАНИС",		"РАШИТ",			"ДАНИИЛ",		"АЛЕКСАНДРА",	"АНТОН",
			"РАИЛЬ",		"АНГЕЛИНА",		"ВЕНИАМИН",			"ДАМИР",		"ДМИТРИЙ",		"МАРИНА",
			"ИЛЬФАТ",		"КЛАВДИЯ",		"РУДОЛЬФ",			"АЛЬБИНА",		"ВЕРА",			"ЮЛИЯ",
			"ТАХИР",		"ИЛЬНУР",		"РАФАИЛ",			"ПОЛИНА",		"НИНА",			"ЕКАТЕРИНА",
			"ВСЕВОЛОД",		"ЗУЛЬФИЯ",		"РОЗА",				"ТИМОФЕЙ",		"ЮРИЙ",			"КОНСТАНТИН",
			"ФАРИД",		"СНЕЖАНА",		"АЗАТ",				"АНТОНИНА",		"МИХАИЛ",		"ПАВЕЛ",
			"АЛИСА",		"АРМЕН",		"МАРСЕЛЬ",			"ВЕРОНИКА",		"ЕВГЕНИЙ",		"ВЯЧЕСЛАВ",
			"МАЙЯ",			"МАНСУР",		"САЛАВАТ",			"ТИМУР",		"ОЛЕСЯ",		"ДЕНИС",
			"ВАРВАРА",		"АЛИК",			"ГЛЕБ",				"ЕЛИЗАВЕТА",	"АНАТОЛИЙ",		"ТАТЬЯНА",
			"АРСЕН",		"РАСИМ",		"ДИАНА",			"РАМИЛЬ",		"ВИКТОР",		"ИРИНА",
			"РЕГИНА",		"АЙДАР",		"ЯН",				"РИММА",		"СЕРГЕЙ",		"НАДЕЖДА",
			"СВЯТОСЛАВ",	"РОДИОН",		"АНЖЕЛА",			"ЯРОСЛАВ",		"АЛЕКСАНДР",	"СВЕТЛАНА",
			"ЛИАНА",		"МАРК",			"РАИС",				"ГЕРМАН",		"МАРГАРИТА",	"ВАДИМ",
			"НЭЛЛИ",		"ФАИНА",		"ЭЛЬДАР",			"ИРАИДА",		"ЭЛЬВИРА",		"МАКСИМ",
			"САМВЕЛ",		"ФАНИС",		"АЙРАТ",			"АНЖЕЛИКА",		"АЛЛА",			"НАТАЛЬЯ",
			"ЕФИМ",			"АРТЕМИЙ",		"ИЛЬЯС",			"РАВИЛЬ",		"ИННА",			"ЕВГЕНИЯ",
			"ТАГИР",		"РАФАЭЛЬ",		"РАФИК",			"ГУЛЬНАРА",		"АЛЕКСЕЙ",		"ВАЛЕРИЙ",
		};

		[Test]
		public void CanStore(){
			var index = new BTreeIndex<string, bool, bool>(10);
			foreach(var s in strings){
				index.EnsureValue(s);
			}
			Console.WriteLine(index.PlainIndex.Count);
		}

		[Test]
		public void CanRead()
		{
			var index = new BTreeIndex<string, bool, bool>(10);
			foreach (var s in strings)
			{
				index.EnsureValue(s);
			}
			foreach (var s in strings)
			{
				Assert.NotNull(index.Find(s));
			}
		}

		[Test]
		public void CanWriteAndRestoreFromBinary(){
			var index = new BTreeIndex<string, bool, bool>(10);
			foreach (var s in strings)
			{
				index.EnsureValue(s);
			}
			using (var stream = new MemoryStream()){
				using (var sw = new BinaryWriter(stream,Encoding.UTF8,true)){
					index.Write(sw);
					sw.Flush();
				}
				index = new BTreeIndex<string, bool, bool>(10);
				stream.Position = 0;
				using (var sr = new BinaryReader(stream, Encoding.UTF8, true))
				{
					index.Read(sr);
				}
			};
			
			
			foreach (var s in strings)
			{
				Assert.NotNull(index.Find(s));
			}
		}

		private void Write(BTreeIndex<string, bool, bool> index){
			Console.WriteLine();

			foreach (var b in index.PlainIndex.Values){
				Console.WriteLine(b.Id);
				foreach (var value in b.Values){
					Console.Write(value.ToString());
				}
				Console.WriteLine();
				Console.WriteLine("------------------------------------------");
			}

			Console.WriteLine("===================================================");
		}
	}
}
