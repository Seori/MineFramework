using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace MineFramework
{
    public class DictionaryCls : Dictionary<string, object>
    {
        /// <summary>
        /// 기본생성자 정의
        /// </summary>
        public DictionaryCls()
        {

        }

        public DictionaryCls(int capacity) : base(capacity)
        {

        }

        public DictionaryCls(IEqualityComparer<string> comparer) : base(comparer)
        {

        }

        public DictionaryCls(int capacity, IEqualityComparer<string> comparer) : base(capacity, comparer)
        {

        }

        public DictionaryCls(IDictionary<string, object> dictionary) : base(dictionary)
        {

        }

        public DictionaryCls(IDictionary<string, object> dictionary, IEqualityComparer<string> comparer) : base(dictionary, comparer)
        {
            
        }

        protected DictionaryCls(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }
        
        public void addOrUpdate(string szKey, object szValue)
        {
            try
            {
                if (ContainsKey(szKey))
                {
                    base[szKey] = szValue;
                }
                else
                {
                    Add(szKey, szValue);
                }

            }
            catch (ArgumentException)
            {
                base[szKey] = szValue;
            }
            catch (Exception ex)
            {

            }
        }

        public void addOrUpdate(DictionaryCls inData)
        {
            try
            {
                if (inData == null)
                {
                    return;
                }

                foreach(string szKey in inData.Keys)
                {
                    addOrUpdate(szKey, inData[szKey]);
                }
            }
            catch(Exception ex)
            {

            }
        }

        public object this[string szKey]
        {
            get
            {
                object outValue = null;

                if (!TryGetValue(szKey, out outValue))
                {
                    return null;
                }

                return outValue;
            }
            set
            {
                addOrUpdate(szKey, value);
            }
        }

        public object this[string szKey, object szDefaultObject]
        {
            get
            {
                object outValue = null;

                if (!TryGetValue(szKey, out outValue))
                {
                    return szDefaultObject;
                }

                return outValue;
            }
        }
    }
}
