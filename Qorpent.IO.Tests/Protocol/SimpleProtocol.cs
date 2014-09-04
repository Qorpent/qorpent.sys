using System;
using System.Text;
using Qorpent.IO.Protocols;

namespace Qorpent.IO.Tests.Protocol{
	/// <summary>
	/// Простой протокол, собирающий строку из переданных чанков
	/// </summary>
	public class SimpleProtocol:Protocols.Protocol{
		readonly StringBuilder _builder = new StringBuilder();
		public string Result;
		public int ActualCount;
		public int MaxCount = int.MaxValue;
		public bool ThrowOnOverMax;
		protected override void ProcessPage(ProtocolBufferPage page){
			ActualCount++;
			if (ActualCount > MaxCount)
			{
				if (ThrowOnOverMax){
					Error = new Exception("exceed max");
				}
				else{
					Finish();
				}
				return;
			}
			_builder.Append(page.ReadAscii());
		}
		public override void Start(){
			base.Start();
			Result = null;
			_builder.Clear();
			ActualCount = 0;
		}
		public override void Finish(){
			base.Finish();
			if (null!=Error) return;
			Result = _builder.ToString();
		}
	}
}