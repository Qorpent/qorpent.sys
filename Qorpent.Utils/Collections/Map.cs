#region LICENSE
// Copyright 2007-2013 Qorpent Team - http://github.com/Qorpent
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
// PROJECT ORIGIN: Qorpent.Utils/Map.cs
#endregion
using System;
using System.Collections.Generic;
using System.Linq;

namespace Qorpent.Utils.Collections{
    /// <summary>
    /// ��������������� ����� ���� ��������->���� c ������������ �������
    /// � ��������� ������
    /// �������� ������ ����������� ����������� ���������,
    /// ����� ����� ��������� ��������� ���������� ��������
    /// </summary>
    /// <typeparam name="F">��� ��������� ��������</typeparam>
    /// <typeparam name="T">��� �������� ��������</typeparam>
    public class Map<F, T> : List<IMapItem<F, T>>, IMap<F, T>{
        #region IMap<F,T> Members

        /// <summary>
        /// ���������� ������ ����� � ����� ����������
        /// </summary>
        /// <param name="from">��������</param>
        /// <returns>������ �����</returns>
        public T[] this[F from]{
            get{
                return Enumerable.Select(this
                                .Where(GetMainPredicate(from)), GetMainConverter()
                    ).Distinct().ToArray();
            }
            set{
                foreach (var v in value){
                    this.Add(new MapItem<F, T> { From = from, To = v });    
                }
                
            }
           
        }

        /// <summary>
        /// ������� � ��������� ����� ������� �����
        /// </summary>
        /// <param name="from">��������</param>
        /// <param name="to">����</param>
        /// <returns>������� �����</returns>
        public IMapItem<F, T> Add(F from, T to){
            var result = new MapItem<F, T>{From = from, To = to};
            Add(result);
            return result;
        }

        /// <summary>
        /// ���������� ������ ���������� ��� ����
        /// </summary>
        /// <param name="to">����</param>
        /// <returns>������ ����������</returns>
        public F[] Reverse(T to){
            return Enumerable.Select(this.Where(GetreversePredicate(to)), GetReverseConverter()).Distinct().ToArray();
        }

        #endregion

        /// <summary>
        /// �������� ��������� - ����������� ����
        /// </summary>
        /// <returns>������ ����������� �������� ����� � �� ����, �� ��������� m=>m.To</returns>
        protected virtual Func<IMapItem<F, T>, T> GetMainConverter(){
            return m =>
                   {
                       //Contract.Assume(m != null);
                       return m.To;
                   };
        }

        /// <summary>
        /// �������� �������� (��������� ��������)
        /// </summary>
        /// <param name="from">���� ���������</param>
        /// <returns>������ �������� ���������, �� ��������� m => m.From.Equals(from)</returns>
        protected virtual Func<IMapItem<F, T>, bool> GetMainPredicate(F from){
            return m =>
                   {
                       //Contract.Assume(m != null);
                       return m.From.Equals(from);
                   };
        }

        /// <summary>
        /// ����������� ��������� - ����������� ���������
        /// </summary>
        /// <returns>������ ����������� �������� ����� � ��� ��������, �� ��������� m=>m.From</returns>
        protected virtual Func<IMapItem<F, T>, F> GetReverseConverter(){
            return m =>
                   {
                       //Contract.Assume(m != null);
                       return m.From;
                   };
        }

        /// <summary>
        /// ����������� �������� (��������� ����)
        /// </summary>
        /// <param name="to">���� ����</param>
        /// <returns>������ �������� ����, �� ��������� m => m.To.Equals(ConvertTo)</returns>
        protected virtual Func<IMapItem<F, T>, bool> GetreversePredicate(T to){
            return m =>
                   {
                       //Contract.Assume(m != null);
                       return m.To.Equals(to);
                   };
        }

        
    }
}