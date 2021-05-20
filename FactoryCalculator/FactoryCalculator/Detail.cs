using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace FactoryCalculator
{
    [DataContract]
    public class Detail :ICloneable
    {
        [DataMember]
        public Item item;
        [DataMember]
        public float count;
        private Detail()
        {

        }
        public Detail(Item item, float count)
        {
            this.item = item;
            this.count = count;
        }

        public object Clone()
        {
            

            return new Detail((Item)this.item.Clone(), this.count);
        }
    }
}
