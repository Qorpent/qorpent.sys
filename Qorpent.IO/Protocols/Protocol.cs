// Copyright 2007-2014  Qorpent Team - http://github.com/Qorpent
// Supported by Media Technology LTD 
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
// Created : 2014-09-04

using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Qorpent.Log;

namespace Qorpent.IO.Protocols{
	/// <summary>
	///     ����������� ����� ���������
	/// </summary>
	public abstract class Protocol{
		/// <summary>
		/// ������
		/// </summary>
		public IUserLog Log;
		/// <summary>
		///     ������� �������� �� ���������
		/// </summary>
		protected ConcurrentQueue<ProtocolBufferPage> PageQueue = new ConcurrentQueue<ProtocolBufferPage>();

		/// <summary>
		///     ������� ������� ���������
		/// </summary>
		protected Task Worker = null;

		private bool _isAlive = true;

		/// <summary>
		///     ������ ��� ���������
		/// </summary>
		public Exception Error { get; set; }

		/// <summary>
		///     ������� ������ ���������
		/// </summary>
		public bool IsAlive{
			get{
				if (null != Error) return false;
				if (Success) return false;
				return _isAlive;
			}
			protected set { _isAlive = value; }
		}

		/// <summary>
		///     ������� ��������� ������ ���������
		/// </summary>
		public bool Success { get; protected set; }


		/// <summary>
		///     ��������� �������� ���������
		/// </summary>
		/// <param name="page"></param>
		public void Process(ProtocolBufferPage page){
			if(null!=Log)Log.Debug("Protocol: enque page");
			PageQueue.Enqueue(page);
			if (null == Worker || Worker.IsCompleted){
				Worker = Task.Run((Action) InternalProcess);
			}
		}

		/// <summary>
		///     ��������� ��������� ������� � ��������� ����� ���������
		/// </summary>
		public void Join(){
			if (null != Log) Log.Debug("Protocol: start join");
			if (null != Worker) Worker.Wait();
			if (null == Error && 0 != PageQueue.Count){
				InternalProcess();
			}
			if (null != Log) Log.Debug("Protocol: end join");
		}


		private void InternalProcess(){
			if (null != Log) Log.Debug("Protocol: start process");
			ProtocolBufferPage page;
			while (IsAlive && PageQueue.TryDequeue(out page)){
				page.State = ProtocolBufferPage.Read;
				try{
					if (null != Log) Log.Debug("Protocol: start page");
					ProcessPage(page);
					if (null != Log) Log.Debug("Protocol: end page");
				}
				catch (IOException ioException){
					Error = ioException;
				}
				catch (Exception error){
					if (null != Log) Log.Error("Protocol: error detected - "+error.Message);
					Error = new IOException(error.Message, error);
				}
				finally{
					page.State = ProtocolBufferPage.Free;
				}
			}
			if (!IsAlive){
				CleanupQueue();
			}
			if (null != Log) Log.Debug("Protocol: end process");
		}

		/// <summary>
		///     ���������� ��� ��������� �������� ����������� ������, ������ ���� �������� � �������� ���������
		/// </summary>
		/// <param name="page"></param>
		protected abstract void ProcessPage(ProtocolBufferPage page);

		private void CleanupQueue(){
			ProtocolBufferPage page;
			while (!PageQueue.IsEmpty) PageQueue.TryDequeue(out page);
		}
		/// <summary>
		/// ������� ���������� ��������� ���������
		/// </summary>
		public virtual void Finish(){
			Success = null == Error;
		}
		/// <summary>
		/// ������� ������ ������ ���������
		/// </summary>
		public virtual void Start(){
			Error = null;
			Success = false;
		}
	}
}