using System;
using System.IO;

namespace Qorpent.IO.Tests{
	public class StubFileRecord:WebFileRecord{
		public StubFileRecord(string match){
			this.Name = match;
		}

		protected override DateTime GetVersion(){
			throw new NotImplementedException();
		}

		public override long Write(Stream output){
			throw new NotImplementedException();
		}
	}
}