using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace MineFramework
{
    public class GeneralDictionary : Dictionary<string, object>
    {
        public GeneralDictionary()
        {
        }

        public GeneralDictionary(int capacity)
            : base(capacity)
        {
        }

        public GeneralDictionary(IEqualityComparer<string> comparer)
            : base(comparer)
        {
        }
        public GeneralDictionary(int capacity, IEqualityComparer<string> comparer)
            : base(capacity, comparer)
        {
        }
        public GeneralDictionary(IDictionary<string, object> dictionary)
            : base(dictionary)
        {
        }
        public GeneralDictionary(IDictionary<string, object> dictionary, IEqualityComparer<string> comparer)
            : base(dictionary, comparer)
        {
        }
        protected GeneralDictionary(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        //----------------------------------------------------------------------

        #region 기본 오퍼레이션 정의

        /// <summary>
        /// Object을 추가하거나 갱신합니다.
        /// </summary>
        /// <param name="szKey">키 이름</param>
        /// <param name="objValue">object 값</param>
        public void AddOrUpdate(string szKey, object objValue)
        {
            try
            {
                if (ContainsKey(szKey))
                {
                    // 키가 이미 있는 경우 값을 갱신한다.
                    base[szKey] = objValue;
                }
                else
                {
                    // 새로운 키를 추가한다.
                    Add(szKey, objValue);
                }
            }
            catch (ArgumentException)
            {
                // 키가 이미 있는 경우 값을 갱신한다.
                base[szKey] = objValue;
            }
            catch (Exception ex)
            {
                // 예외 처리
                Trace.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// 다른 GeneralDictionary 객체의 데이터를 추가합니다.
        /// </summary>
        /// <param name="inData">GeneralDictionary 객체</param>
        public void AddOrUpdate(GeneralDictionary inData)
        {
            try
            {
                if (inData == null)
                    return;

                foreach (string szKey in inData.Keys)
                {
                    // 입력된 모든 데이터를 추가/갱신 한다.
                    AddOrUpdate(szKey, inData[szKey]);
                }
            }
            catch (Exception ex)
            {
                // 예외 처리
                Trace.WriteLine(ex.ToString());
            }
        }

        #endregion

        //----------------------------------------------------------------------

        #region 인덱서 정의 및 처리

        /// <summary>
        /// 기본 인덱서를 정의합니다. 
        /// 기본적으로 데이터에 대한 설정/취득은 인덱서를 사용합니다.
        /// </summary>
        /// <param name="szKey">키 이름</param>
        public object this[string szKey]
        {
            get
            {
                object outValue = null;

                // Field 키가 존재하면 해당 값을 반환하거나 
                // 존재하지 않으면 빈 문자열을 반환한다.
                if (!TryGetValue(szKey, out outValue))
                    return null;

                return outValue;
            }
            set
            {
                // 추가 또는 갱신
                AddOrUpdate(szKey, value);
            }
        }

        /// <summary>
        /// 문자열 처리 인덱서를 정의합니다. 
        /// 입력된 키 값를 취득시에만 사용할 수 있습니다.
        /// </summary>
        /// <param name="szKey">키 이름</param>
        /// <param name="szDefaultObject">
        /// 키가 존재하지 않을 경우 사용되는 디폴트로 사용되는 object 입니다.
        /// 보통 object를 보통 문자열로 사용하는 경우가 많으며,
        /// 이 때 null 처리의 번거로움을 피하기위해서 사용됩니다.
        /// </param>
        public object this[string szKey, object szDefaultObject]
        {
            get
            {
                object outValue = null;

                // Field 키가 존재하면 해당 값을 반환하거나 
                // 존재하지 않으면 빈 문자열을 반환한다.
                if (!TryGetValue(szKey, out outValue))
                    return szDefaultObject;

                return outValue;
            }
        }

        #endregion
    }
}
